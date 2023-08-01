using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Humanizer;
using NSolaris.Helpers;
using NSolaris.Models;
using NSolaris.Util;
using SolarisDIB.Cli.Util;

namespace NSolaris.Client;

public class SolarisClient {
    public string BaseUrl { get; }
    private readonly HttpClient _client;
    private readonly ILogger _log;
    private SolarisClientCache _cache = new();

    public UserInfo? User { get; private set; }

    public SolarisClient(Logger log, string baseUrl) {
        BaseUrl = baseUrl;
        _log = log.For<SolarisClient>();
        _client = new HttpClient(
            new HttpClientHandler {
                UseCookies = true,
                AllowAutoRedirect = true,
                CookieContainer = new CookieContainer(),
            }
        ) {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromSeconds(300),
        };
    }

    public void SaveCache(string cacheDir) {
        Directory.CreateDirectory(cacheDir);
        _log.Trace($"saving cache to {cacheDir}");
        foreach (var gameId in _cache.Games.Keys) {
            _log.Trace($"  saving cache for game {gameId}");
            var gameCache = _cache.Games[gameId];
            var gameCachePath = Path.Join(cacheDir, $"{gameId}.json");
            var jsonDump = JsonSerializer.Serialize(gameCache, GameModelHelpers.CreateJsonSerializerOptions());
            File.WriteAllText(gameCachePath, jsonDump);
        }
    }

    public bool LoadCache(string cacheDir) {
        if (!Directory.Exists(cacheDir)) {
            _log.Trace($"no client cache found at {cacheDir}");
            return false;
        }

        _log.Trace($"loading cache from {cacheDir}");
        var loadedCache = new SolarisClientCache();
        var gameCacheFiles = Directory.GetFiles(cacheDir, "*.json");
        foreach (var gameCacheFile in gameCacheFiles) {
            var gameId = Path.GetFileNameWithoutExtension(gameCacheFile);
            var cacheFileSize = new FileInfo(gameCacheFile).Length;
            _log.Trace($"  loading cache for game {gameId} ({cacheFileSize.Bytes()})");
            var jsonDump = File.ReadAllText(gameCacheFile);
            var gameCache =
                JsonSerializer.Deserialize<SolarisClientCache.GameCache>(jsonDump,
                    GameModelHelpers.CreateJsonSerializerOptions());
            if (gameCache == null) continue;
            loadedCache.Games[gameId] = gameCache;
        }

        _cache = loadedCache;
        return true;
    }

    public async Task<bool> Login(string email, string password) {
        _log.Info($"logging in to {BaseUrl} as {email}");
        var resp = await _client.PostAsJsonAsync("/api/auth/login", new {
            email,
            password,
        });
        resp.EnsureSuccessStatusCode();

        User = await resp.Content.ReadFromJsonAsync<UserInfo>();
        if (User == null) throw new ArgumentNullException(nameof(User));

        _log.Info($"logged in as {User.username} ({User._id})");

        return true;
    }

    public async Task<List<GameOverviewResponse>> GetActiveGames() {
        _log.Trace("getting active games");

        var resp = await _client.GetAsync("/api/game/list/active");
        resp.EnsureSuccessStatusCode();

        var resData = await resp.Content.ReadFromJsonAsync<List<GameOverviewResponse>>();
        if (resData == null) throw new ArgumentNullException(nameof(resData));

        _log.Trace($"  got {resData.Count} active games");
        return resData;
    }

    public async Task<List<GameOverviewResponse>> GetPastGames() {
        _log.Trace("getting past games");

        var resp = await _client.GetAsync("/api/game/list/completed/user");
        resp.EnsureSuccessStatusCode();

        var resData = await resp.Content.ReadFromJsonAsync<List<GameOverviewResponse>>();
        if (resData == null) throw new ArgumentNullException(nameof(resData));

        _log.Trace($"  got {resData.Count} past games");
        return resData;
    }

    public async Task<GameInfoResponse?> GetGameInfo(string gameId) {
        _log.Trace($"getting info for game {gameId}");

        var resp = await _client.GetAsync($"/api/game/{gameId}/info");
        resp.EnsureSuccessStatusCode();

        var resData = await resp.Content.ReadFromJsonAsync<GameInfoResponse>();
        if (resData == null) return null;

        _log.Trace($"  succesfully parsed info for game {gameId}");

        return resData;
    }

    public async Task<GameSyncResponse> GetGameSync(string gameId, int? tick = null) {
        _log.Trace($"getting sync data for game {gameId}@{tick}");

        var query = new Dictionary<string, string>();
        SolarisClientCache.GameCache? gameCache;
        if (tick != null) {
            // see if we have it cached
            if (_cache.ForGame(gameId).SyncHistory.TryGetValue(tick.Value, out var cachedSync)) {
                _log.Trace($"  found cached sync data for game {gameId}@{tick}");
                return cachedSync;
            }

            query["tick"] = tick.Value.ToString();
        }

        var resp = await _client.GetAsync(QueryHelper.BuildUrl($"/api/game/{gameId}/galaxy", query));
        resp.EnsureSuccessStatusCode();

        var syncResData = await resp.Content.ReadFromJsonAsync<GameSyncResponse>();
        if (syncResData == null) throw new ArgumentNullException(nameof(syncResData));

        // store in cache
        _cache.ForGame(gameId).SyncHistory[syncResData.state.tick] = syncResData;

        _log.Trace($"  succesfully parsed sync data for game {gameId}");

        return syncResData;
    }

    public async Task<Dictionary<int, GameSyncResponse>> GetGameSyncHistory(string gameId, int? startTick = null,
        int? endTick = null) {
        _log.Trace($"getting sync history for game {gameId}@[{startTick},{endTick}]");

        if (startTick == null || endTick == null) {
            var currentTickSync = await GetGameSync(gameId);

            if (startTick == null) startTick = 1;
            if (endTick == null) endTick = currentTickSync.state.tick;
        }

        var syncHistory = new Dictionary<int, GameSyncResponse>();
        for (var tick = startTick.Value; tick <= endTick.Value; tick++) {
            syncHistory[tick] = await GetGameSync(gameId, tick);
        }

        return syncHistory;
    }

    public async Task<GameIntelTick[]> GetGameIntel(string gameId, int? startTick = null, int? endTick = null) {
        _log.Trace($"getting intel data for game {gameId}@[{startTick},{endTick}]");

        // see if we have it cached
        if (startTick != null && endTick != null) {
            if (_cache.ForGame(gameId).IntelTickHistory != null) {
                var cachedTickHistory = _cache.ForGame(gameId).IntelTickHistory!;
                if (cachedTickHistory.Last().tick >= endTick.Value) {
                    _log.Trace($"  found cached intel data for game {gameId}@{startTick},{endTick}");
                    return cachedTickHistory.Where(t => t.tick >= startTick.Value && t.tick <= endTick.Value).ToArray();
                }
            }
        }

        var query = new Dictionary<string, string>();
        if (startTick != null) query["startTick"] = startTick.Value.ToString();
        if (endTick != null) query["endTick"] = endTick.Value.ToString();

        var resp = await _client.GetAsync(QueryHelper.BuildUrl($"/api/game/{gameId}/intel", query));
        resp.EnsureSuccessStatusCode();

        var ticksResData = await resp.Content.ReadFromJsonAsync<GameIntelTick[]>();
        if (ticksResData == null) throw new ArgumentNullException(nameof(ticksResData));

        // store in cache
        _cache.ForGame(gameId).IntelTickHistory = ticksResData;

        _log.Trace($"  succesfully parsed intel data for game {gameId}");

        return ticksResData;
    }

    public async Task<GameEventsResponse> GetGameEvents(string gameId) {
        _log.Trace($"getting events for game {gameId}");

        var resp = await _client.GetAsync($"/api/game/{gameId}/events");
        resp.EnsureSuccessStatusCode();

        // var resData = await resp.Content.ReadFromJsonAsync<GameEventsResponse>();
        var eventsResData =
            GameModelHelpers.GameEventsParserHelper.ParseGameEventsResponse(await resp.Content.ReadAsStringAsync());
        if (eventsResData == null) throw new ArgumentNullException(nameof(eventsResData));

        // store in cache
        _cache.ForGame(gameId).EventsHistory = eventsResData;

        _log.Trace($"  succesfully parsed events for game {gameId}");

        return eventsResData;
    }
}
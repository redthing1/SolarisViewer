using System.Text.Json;
using NSolaris.Client;
using NSolaris.Models;
using SolarisDIB.Cli.Util;

namespace SolarisDIB.Cli.Helpers;

public static class GameLoadHelper {
    public record LoadedGame(string MePlayerId, GameSyncResponse GameSync, GameIntelTick[] GameIntel,
        GameEventsResponse GameEvents,
        Dictionary<int, GameSyncResponse> SyncHistory);

    public static Task<LoadedGame> LoadOfflineGameDataInteractive(ILogger log, string dataPath) {
        log.Info($"loading game data from {dataPath}");

        // load intel
        var intelPath = Path.Combine(dataPath, "intel.json");
        log.Info($"  loading intel data from {intelPath}");
        var intel = JsonSerializer.Deserialize<GameIntelTick[]>(File.ReadAllText(intelPath))!;

        // load sync history
        var syncHistory = new Dictionary<int, GameSyncResponse>();
        log.Info($"  loading sync history from {dataPath}");
        var gameSyncFiles = Directory.GetFiles(dataPath, "tick_*.json")
            .OrderBy(x => x)
            .ToList();
        foreach (var gameSyncFile in gameSyncFiles) {
            var tick = int.Parse(Path.GetFileNameWithoutExtension(gameSyncFile).Split('_')[1]);
            log.Trace($"    loading sync data for tick#{tick} from {gameSyncFile}");
            var jsonDump = File.ReadAllText(gameSyncFile);
            var gameSync = JsonSerializer.Deserialize<GameSyncResponse>(jsonDump)!;
            syncHistory[tick] = gameSync;
        }

        var mostRecentSync = syncHistory.Values.MaxBy(x => x.state.tick);

        // we don't have events, so we'll just make an empty list
        var events = new GameEventsResponse(0, new List<GameEvent>());

        var mePlayerId = mostRecentSync!.galaxy.players.Single(x => x.hasPerspective)._id;

        var ret = new LoadedGame(mePlayerId, mostRecentSync, intel, events, syncHistory);
        return Task.FromResult(ret);
    }

    public static async Task<LoadedGame> LoadOnlineGameDataInteractive(ILogger log, SolarisClient client,
        GameInfoResponse gameInfo) {
        log.Info($"syncing game {gameInfo.settings.general.name} ({gameInfo._id})");
        var gameSync = await client.GetGameSync(gameInfo._id, tick: gameInfo.state.tick);
        log.Info($"  successfully synced game {gameInfo.settings.general.name} ({gameInfo._id})");
        log.Info($"getting intel data for game {gameInfo._id}");
        var gameIntel = await client.GetGameIntel(gameInfo._id, startTick: 1, endTick: gameInfo.state.tick);
        log.Info($"  succesfully got intel data for game {gameInfo._id}");
        log.Info($"getting events for game {gameInfo._id}");
        var gameEvents = await client.GetGameEvents(gameInfo._id);
        log.Info($"  succesfully got events for game {gameInfo._id}");

        log.Info($"getting game sync history for game {gameInfo._id}");
        var gameSyncHistory =
            await client.GetGameSyncHistory(gameInfo._id, startTick: 1, endTick: gameInfo.state.tick);
        log.Info($"  succesfully got game sync history for game {gameInfo._id}");

        var mePlayerId = gameSync.galaxy.players.Single(x => x.hasPerspective)._id;

        return new LoadedGame(mePlayerId, gameSync, gameIntel, gameEvents, gameSyncHistory);
    }
}
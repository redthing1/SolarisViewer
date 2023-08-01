using NSolaris.Models;

namespace NSolaris.Client;

public class SolarisClientCache {
    public class GameCache {
        public Dictionary<int, GameSyncResponse> SyncHistory { get; set; } = new();
        public GameIntelTick[]? IntelTickHistory { get; set; } = null;
        public GameEventsResponse? EventsHistory { get; set; } = null;
    }

    public Dictionary<string, GameCache> Games { get; set; } = new();
    public GameCache ForGame(string gameId) {
        if (!Games.TryGetValue(gameId, out var gameCache)) {
            gameCache = new GameCache();
            Games[gameId] = gameCache;
        }

        return gameCache;
    }
}
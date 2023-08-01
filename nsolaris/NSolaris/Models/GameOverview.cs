// ReSharper disable InconsistentNaming
namespace NSolaris.Models;

public record GameStateOverview(
    bool locked,
    int tick,
    bool paused,
    int productionTick,
    int players,
    bool cleaned,
    int stars,
    int starsForVictory
);

public record GameAfkOverview(
    int lastSeenTimeout,
    int cycleTimeout,
    int turnTimeout
);

public record GameTime(
    GameAfkOverview afk,
    string gameType,
    int speed,
    int startDelay,
    int turnJumps,
    int maxTurnWait,
    string isTickLimited
);

public record UserNotifications(
    int? unreadConversations,
    int? unreadEvents,
    int? unread,
    bool? turnWaiting
);

public record GameSettingsOverviewGeneral(
    string type,
    int playerLimit,
    string name
);

public record GameSettingsOverviewGalaxy(
    int productionTicks
);

public record GameSettingsOverview(
    GameSettingsOverviewGeneral general,
    GameSettingsOverviewGalaxy galaxy,
    GameTime gameTime
);

public record GameInfoResponse(
    string _id,
    GameSettingsOverview settings,
    GameStateOverview state
) {
    public string Name => settings.general.name;
}


public record GameOverviewResponse(
    string _id,
    GameSettingsOverview settings,
    GameStateOverview state,
    UserNotifications userNotifications
) : GameInfoResponse(_id, settings, state);
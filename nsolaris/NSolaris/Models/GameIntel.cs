// ReSharper disable InconsistentNaming
namespace NSolaris.Models;

public record PlayerIntelTickStatistics(
    int totalStars,
    int totalHomeStars,
    int totalEconomy,
    int totalIndustry,
    int totalScience,
    int totalShips,
    int totalCarriers,
    int totalSpecialists,
    int totalStarSpecialists,
    int totalCarrierSpecialists,
    float newShips,
    int warpgates
);

public record PlayerIntelTickResearchItem(
    int level
);

public record PlayerIntelTickResearch(
    PlayerIntelTickResearchItem scanning,
    PlayerIntelTickResearchItem hyperspace,
    PlayerIntelTickResearchItem terraforming,
    PlayerIntelTickResearchItem experimentation,
    PlayerIntelTickResearchItem weapons,
    PlayerIntelTickResearchItem banking,
    PlayerIntelTickResearchItem manufacturing,
    PlayerIntelTickResearchItem specialists
) {

    public override string ToString() {
        return $"{scanning.level} S / {hyperspace.level} H / {terraforming.level} T / {experimentation.level} E / {weapons.level} W / {banking.level} B / {manufacturing.level} M / {specialists.level} P";
    }
}

public record PlayerIntelTick(
    PlayerIntelTickStatistics statistics,
    PlayerIntelTickResearch research,
    string playerId
);

public record GameIntelTick(
    string _id,
    string gameId,
    int tick,
    List<PlayerIntelTick> players
) {
    public PlayerIntelTick? GetPlayerIntel(Player player) => GetPlayerIntel(player._id);
    public PlayerIntelTick? GetPlayerIntel(string playerId) {
        return players.SingleOrDefault(x => x.playerId == playerId);
    }
}
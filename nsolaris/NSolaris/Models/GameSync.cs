// ReSharper disable InconsistentNaming

namespace NSolaris.Models;

public record PlayerColor(
    string alias,
    string value
);

public record LedgerCredit(
    int debt,
    string _id,
    string playerId
);

public record Reputation(
    int score,
    string playerId,
    string? _id
);

public record Location(
    float x,
    float y
) {
    override public string ToString() {
        return $"({x:0.00}, {y:0.00})";
    }
}

public record CarrierToStarCombatModifier(
    int? weaponsPerAlly
);

public record CarrierToStarCombat(
    CarrierToStarCombatModifier? attacker,
    CarrierToStarCombatModifier? defender
);

public record SpecialistLocalModifiers(
    int? scanning,
    int? manufacturing,
    int? terraforming,
    int? weapons,
    int? hyperspace,
    float? speed,
    CarrierToStarCombat? carrierToStarCombat
);

public record SpecialistSpecialModifiers(
    bool? hideShips,
    bool? lockWarpGates,
    int? autoCarrierSpecialistAssign,
    int? addNaturalResourcesOnTick,
    int? deductEnemyWeapons,
    bool? avoidcombatCarrierToCarrier,
    bool? unlock_warpGates,
    int? starCaptureReward_multiplier
);

public record Ledger(
    List<LedgerCredit> credits,
    List<LedgerCredit> creditsSpecialists
);

public record PlayerDiplomaticStatus(
    string _id,
    string playerId,
    string status
);

public record StarUpgradeCost(
    int? economy,
    int? industry,
    int? science,
    int? warpGate,
    int? carriers
);

public record StarIgnoreBulkUpgrade(
    bool? economy,
    bool? industry,
    bool? science
);

public record Waypoint(
    string _id,
    string source,
    string destination,
    string? action,
    int? actionShips,
    int? delayTicks,
    int? ticks,
    int? ticksEta
);

public record EffectiveTechnologyLevels(
    int scanning,
    int hyperspace,
    int terraforming,
    int experimentation,
    int weapons,
    int banking,
    int manufacturing,
    int specialists
);

public record ResearchCostTiers(
    string scanning,
    string hyperspace,
    string terraforming,
    string experimentation,
    string weapons,
    string banking,
    string manufacturing,
    string specialists
);

public record ResearchItemState(
    int level,
    int? progress
);

public record TechnologyLevels(
    ResearchItemState scanning,
    ResearchItemState hyperspace,
    ResearchItemState terraforming,
    ResearchItemState experimentation,
    ResearchItemState weapons,
    ResearchItemState banking,
    ResearchItemState manufacturing,
    ResearchItemState specialists
) {
    public EffectiveTechnologyLevels ToEffectiveLevels() {
        return new EffectiveTechnologyLevels(
            scanning: scanning.level,
            hyperspace: hyperspace.level,
            terraforming: terraforming.level,
            experimentation: experimentation.level,
            weapons: weapons.level,
            banking: banking.level,
            manufacturing: manufacturing.level,
            specialists: specialists.level
        );
    }
}

public record ExpenseMultipliers(
    int standard,
    int expensive,
    int veryExpensive,
    int crazyExpensive,
    int? cheap
);

public record GameState(
    bool locked,
    int tick,
    bool paused,
    int productionTick,
    string startDate,
    string? endDate,
    string lastTickDate,
    int? ticksToEnd,
    int players,
    string? winner,
    bool cleaned,
    List<string> leaderboard,
    int stars,
    int starsForVictory
);

public record ExpenseModifiers(
    float? none,
    float? cheap,
    float? standard,
    float? expensive,
    float? crazyExpensive
);

public record GameSettingsFlux(
    int id,
    string name,
    string month,
    string description,
    string tooltip
);

public record GameSettingsGeneral(
    int fluxId,
    string description,
    string type,
    string mode,
    bool featured,
    bool passwordRequired,
    int playerLimit,
    string playerType,
    string anonymity,
    string playerOnlineStatus,
    string timeMachine,
    string awardRankTo,
    string fluxEnabled,
    string advancedAI,
    string spectators,
    string readyToQuit,
    string name,
    GameSettingsFlux flux
);

public record GameSettingsGalaxy(
    string galaxyType,
    int starsPerPlayer,
    int productionTicks
);

public record SpecialistBan(
    List<int> star,
    List<int> carrier
);

public record GameSettingsSpecialGalaxy(
    SpecialistBan specialistBans,
    string carrierCost,
    string carrier_upkeepCost,
    string warpgateCost,
    string specialistCost,
    string specialistsCurrency,
    int random_warpGates,
    int randomWormHoles,
    int randomNebulas,
    int randomAsteroidFields,
    int randomBinaryStars,
    int randomBlackHoles,
    int randomPulsars,
    string darkGalaxy,
    string giftCarriers,
    string defenderBonus,
    string carrierToCarrierCombat,
    string splitResources,
    string resourceDistribution,
    string playerDistribution,
    int carrierSpeed,
    string starCaptureReward
);

public record GameSettingsConquest(
    string victoryCondition,
    int victoryPercentage,
    string capitalStarElimination
);

public record GameSettingsKingOfTheHill(
    int productionCycles
);

public record GameSettingsOrbitalMechanics(
    string enabled,
    int orbitSpeed
);

public record StarInfrastructure(
    int economy,
    int industry,
    int science
) {
    override public string ToString() {
        return $"{economy} E / {industry} I / {science} S";
    }
}

public record StarResources(
    int economy,
    int industry,
    int science
) {
    override public string ToString() {
        return $"{economy}";
    }
}

public record GameSettingsPlayerPopulationCap(
    string enabled,
    int shipsPerStar
);

public record PlayerConstantsDevelopmentCosts(
    string economy,
    string industry,
    string science
);

public record GameSettingsDiplomacy(
    string enabled,
    string tradeRestricted,
    int maxAlliances,
    string upkeepCost,
    string globalEvents
);

public record GameSettingsTechnology(
    EffectiveTechnologyLevels startingTechnologyLevel,
    ResearchCostTiers researchCosts,
    string bankingReward,
    string experimentationReward,
    string specialistTokenReward
);

public record GameSettingsAfk(
    int lastSeenTimeout,
    int cycleTimeout,
    int turnTimeout
);

public record GameSettingsGameTime(
    GameSettingsAfk afk,
    string gameType,
    int speed,
    int startDelay,
    int turnJumps,
    int maxTurnWait,
    string is_tickLimited,
    int? tickLimit
);

public record GameSettingsPlayer(
    StarInfrastructure startingInfrastructure,
    PlayerConstantsDevelopmentCosts developmentCost,
    GameSettingsPlayerPopulationCap populationCap,
    int startingStars,
    int startingCredits,
    int startingCreditsSpecialists,
    int startingShips,
    bool tradeCredits,
    bool tradeCreditsSpecialists,
    int tradeCost,
    string tradeScanning
);

public record GameSettings(
    GameSettingsGeneral general,
    GameSettingsGalaxy galaxy,
    GameSettingsSpecialGalaxy specialGalaxy,
    GameSettingsConquest conquest,
    GameSettingsKingOfTheHill kingOfTheHill,
    GameSettingsOrbitalMechanics orbitalMechanics,
    GameSettingsPlayer player,
    GameSettingsDiplomacy diplomacy,
    GameSettingsTechnology technology,
    GameSettingsGameTime gameTime
);

public record DistanceConstants(
    Location galaxyCenterLocation,
    int lightYear,
    int minDistanceBetweenStars,
    int maxDistanceBetweenStars,
    int warpSpeedMultiplier
);

public record ResearchConstants(
    int progressMultiplier,
    int sciencePointMultiplier,
    int experimentationMultiplier
);

public record StarConstantsResources(
    int min_naturalResources,
    int max_naturalResources
);

public record StarConstantsInfrastructureCostMultipliers(
    int warpGate,
    float economy,
    int industry,
    int science,
    int carrier
);

public record StarConstants(
    StarConstantsResources resources,
    StarConstantsInfrastructureCostMultipliers infrastructureCostMultipliers,
    ExpenseModifiers infrastructureExpenseMultipliers,
    ExpenseModifiers specialistsExpenseMultipliers,
    int captureRewardMultiplier,
    int homeStarDefenderBonusMultiplier
);

public record DiplomacyConstants(
    ExpenseModifiers upkeepExpenseMultipliers
);

public record PlayerConstants(
    int rankRewardMultiplier,
    int bankingCycleRewardMultiplier
);

public record SpecialistConstants(
    int monthlyBanAmount
);

public record GameConstants(
    DistanceConstants distances,
    ResearchConstants research,
    StarConstants star,
    DiplomacyConstants diplomacy,
    PlayerConstants player,
    SpecialistConstants specialists
);

public record SpecialistModifier(
    SpecialistLocalModifiers? local,
    SpecialistSpecialModifiers? special
);

public record SpecialistActiveInfo(
    bool official,
    bool custom
);

public record Specialist(
    int id,
    string key,
    string name,
    string description,
    SpecialistActiveInfo active,
    int baseCostCredits,
    int baseCostCreditsSpecialists,
    bool oneShot,
    int? expireTicks,
    SpecialistModifier modifiers
);

public record PlayerGuild(
    string _id,
    string name,
    string tag
);

public record PlayerStats(
    int totalStars,
    int totalHomeStars,
    int totalCarriers,
    int totalShips,
    int? totalShipsMax,
    int totalEconomy,
    int totalIndustry,
    int totalScience,
    float newShips,
    int warpgates,
    int totalStarSpecialists,
    int totalCarrierSpecialists,
    int totalSpecialists
);

public record Player(
    TechnologyLevels research,
    string avatar,
    string shape,
    bool isOpenSlot,
    bool defeated,
    bool afk,
    bool ready,
    bool readyToQuit,
    int missedTurns,
    bool hasFilledAfkSlot,
    string _id,
    string homeStar_id,
    string alias,
    PlayerColor colour,
    List<PlayerDiplomaticStatus> diplomacy,
    PlayerStats stats,
    bool HasDuplicateIP,
    bool hasPerspective,
    bool isAIControlled,
    bool isInScanningRange,
    bool isRealUser,
    string? userId,
    string? lastSeen,
    string? researchingNow,
    string? researchingNext,
    string? defeatedDate,
    // GameAiState? aiState,
    int? renownToGive,
    bool? readyToCycle,
    bool? hasSentTurnReminder,
    List<string>? spectators,
    int? credits,
    int? creditsSpecialists,
    Ledger? ledger,
    List<Reputation>? reputations,
    int? currentResearchTicksEta,
    int? nextResearchTicksEta,
    Reputation? reputation,
    bool? isOnline,
    PlayerGuild? guild
) {
    override public string ToString() {
        return $"{alias} ({_id})";
    }
}

public record Carrier(
    Location location,
    int? specialistExpireTick,
    bool isGift,
    string _id,
    string ownedByPlayerId,
    string name,
    List<Waypoint> waypoints,
    EffectiveTechnologyLevels effectiveTechs,
    string? orbiting,
    bool? waypointsLooped,
    int? specialistId,
    int? ships,
    int? ticksEta,
    int? ticksEtaTotal,
    Specialist? specialist
);

public record Star(
    Location location,
    string? ownedByPlayerId,
    bool warpGate,
    bool isNebula,
    bool isAsteroidField,
    bool isBinaryStar,
    bool isBlackHole,
    bool isPulsar,
    string _id,
    string name,
    EffectiveTechnologyLevels effectiveTechs,
    bool isInScanningRange,
    StarInfrastructure? infrastructure,
    int? ships,
    int? specialistId,
    int? specialistExpireTick,
    bool? homeStar,
    string? wormHoleToStarId,
    StarResources? naturalResources,
    StarResources? terraformedResources,
    float? manufacturing,
    StarIgnoreBulkUpgrade? ignoreBulkUpgrade,
    StarUpgradeCost? upgradeCosts,
    Specialist? specialist
) {
    public override string ToString() {
        return $"{name} ({location})";
    }
}

public record GameGalaxy(
    List<Player> players,
    List<Star> stars,
    List<Carrier> carriers
);

public record GameSyncResponse(
    string _id,
    GameSettings settings,
    GameGalaxy galaxy,
    GameState state,
    GameConstants constants,
    List<string> spectators
) {
    public string Name => settings.general.name;
    public long CurrentTick => state.tick;

    public Player? GetPlayer(string playerId) => galaxy.players.SingleOrDefault(p => p._id == playerId);
    public Player? GetPlayerByUserId(string userId) => galaxy.players.SingleOrDefault(p => p.userId == userId);
    public Star? GetStar(string starId) => galaxy.stars.SingleOrDefault(s => s._id == starId);
    public Carrier? GetCarrier(string carrierId) => galaxy.carriers.SingleOrDefault(c => c._id == carrierId);

    public List<Carrier> GetPlayerCarriers(string playerId) =>
        galaxy.carriers.Where(c => c.ownedByPlayerId == playerId).ToList();

    public List<Star> GetPlayerStars(string playerId) =>
        galaxy.stars.Where(s => s.ownedByPlayerId == playerId).ToList();
}
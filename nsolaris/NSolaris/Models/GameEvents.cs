// ReSharper disable InconsistentNaming

using System.Text.Json;
using System.Text.Json.Serialization;

namespace NSolaris.Models;

public record PlayerCombatResultWeapons(
    int? defender,
    int? defenderBase,
    int? attacker,
    int? attackerBase
);

public record PlayerCombatShipCounts(
    int? defender,
    int? attacker
);

public record PlayerCombatSpecialist(
    int id,
    string key,
    string name
);

public class PlayerCombatShipCount : Object {
    private int intValue;
    private string? stringValue;

    public static implicit operator PlayerCombatShipCount(int value) => new() { intValue = value };
    public static implicit operator int(PlayerCombatShipCount value) => value.intValue;
    public static implicit operator PlayerCombatShipCount(string value) => new() { stringValue = value };
    public static implicit operator string(PlayerCombatShipCount value) => value.stringValue ?? value.intValue.ToString();

    public class Converter : JsonConverter<PlayerCombatShipCount> {
        public override PlayerCombatShipCount Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            if (reader.TokenType == JsonTokenType.Number) {
                return reader.GetInt32();
            } else if (reader.TokenType == JsonTokenType.String) {
                return reader.GetString()!;
            } else {
                throw new JsonException();
            }
        }

        public override void Write(Utf8JsonWriter writer, PlayerCombatShipCount value, JsonSerializerOptions options) {
            if (value.stringValue is not null) {
                writer.WriteStringValue(value.stringValue);
            } else {
                writer.WriteNumberValue(value.intValue);
            }
        }
    }
}

public record PlayerCombatStar(
    string _id,
    string name,
    string ownedByPlayerId,
    PlayerCombatSpecialist? specialist,
    PlayerCombatShipCount before,
    PlayerCombatShipCount after,
    PlayerCombatShipCount lost,
    bool scrambled
);

public record PlayerCombatCarrier(
    string _id,
    string name,
    string ownedByPlayerId,
    PlayerCombatSpecialist? specialist,
    PlayerCombatShipCount before,
    PlayerCombatShipCount lost,
    PlayerCombatShipCount after,
    bool scrambled
);

public record PlayerCombatResult(
    PlayerCombatResultWeapons weapons,
    PlayerCombatShipCounts before,
    PlayerCombatShipCounts after,
    PlayerCombatShipCounts lost,
    PlayerCombatStar? star,
    List<PlayerCombatCarrier> carriers
);

public record PlayerCombatStarCaptureResult(
    string capturedById,
    string capturedByAlias,
    int captureReward
);

public record EloRatingChange(
    string _id,
    int newRating,
    int oldRating
);

public record EloRatingChangeResult(
    EloRatingChange winner,
    EloRatingChange loser
);

public record GameRanking(
    string playerId,
    int current,
    int @new
);

public record GameRankingResult(
    List<GameRanking> ranks,
    EloRatingChangeResult? eloRating
);

public record BulkUpgradeReportStar(
    string starId,
    string starName,
    StarResources naturalResources,
    int infrastructureCurrent,
    int infrastructureCostTotal,
    int infrastructure,
    int infrastructureCost,
    float? manufacturing
);

public record BulkUpgradeReport(
    int budget,
    List<BulkUpgradeReportStar> stars,
    int cost,
    int upgraded,
    string infrastructureType,
    int ignoredCount
);

public record TechnologyTransfer(
    string name,
    int level,
    int difference
);

public record PlayerCombatCarrierEventData(
    List<string> playerIdDefenders,
    List<string> playerIdAttackers,
    PlayerCombatResult combatResult
);

public record PlayerGalacticCycleCompleteEventData(
    string gameId,
    int gameTick,
    string playerId,
    int creditsEconomy,
    int creditsBanking,
    int creditsSpecialists,
    string experimentTechnology,
    int experimentTechnologyLevel,
    int experimentAmount,
    bool experimentLevelUp,
    string? experimentResearchingNext,
    int? carrierUpkeep,
    int? allianceUpkeep
);

public record PlayerCombatStarEventData(
    string playerId_owner,
    List<string> playerId_defenders,
    List<string> playerId_attackers,
    string starId,
    string starName,
    PlayerCombatStarCaptureResult? captureResult,
    PlayerCombatResult? combatResult
);

public record PlayerJoinedEventData(
    string playerId,
    string alias
);

public record PlayerQuitEventData(
    string playerId,
    string alias
);

public record PlayerDefeatedEventData(
    string playerId,
    string alias,
    bool openSlot
);

public record PlayerAfkEventData(
    string playerId,
    string alias
);

public record GameEndedEventData(
    GameRankingResult rankingResult
);

public record PlayerResearchCompleteEventData(
    string technologyKey,
    int technologyLevel,
    string technologyKeyNext,
    int technologyLevelNext
);

public record PlayerTechnologyReceivedEventData(
    string fromPlayerId,
    TechnologyTransfer technology
);

public record PlayerTechnologySentEventData(
    string toPlayerId,
    TechnologyTransfer technology
);

public record PlayerCreditsReceivedEventData(
    string fromPlayerId,
    int credits
);

public record PlayerCreditsSentEventData(
    string toPlayerId,
    int credits
);

public record PlayerCreditsSpecialistsReceivedEventData(
    string fromPlayerId,
    int creditsSpecialists
);

public record PlayerCreditsSpecialistsSentEventData(
    string toPlayerId,
    int creditsSpecialists
);

public record PlayerRenownReceivedEventData(
    string fromPlayerId,
    int renown
);

public record PlayerRenownSentEventData(
    string toPlayerId,
    int renown
);

public record PlayerGiftReceivedEventData(
    string fromPlayerId,
    string carrierId,
    string carrierName,
    int carrierShips,
    string starId,
    string starName
);

public record PlayerGiftSentEventData(
    string toPlayerId,
    string carrierId,
    string carrierName,
    int carrierShips,
    string starId,
    string starName
);

public record PlayerStarAbandonedEventData(
    string starId,
    string starName
);

public record PlayerStarDiedEventData(
    string starId,
    string starName
);

public record PlayerStarReignitedEventData(
    string starId,
    string starName
);

public record PlayerBulkInfrastructureUpgradedEventData(
    BulkUpgradeReport upgradeReport
);

public record PlayerDebtAddedEventData(
    string debtorPlayerId,
    string creditorPlayerId,
    int amount,
    string ledgerType
);

public record PlayerDebtSettledEventData(
    string debtorPlayerId,
    string creditorPlayerId,
    int amount,
    string ledgerType
);

public record PlayerDebtForgivenEventData(
    string debtorPlayerId,
    string creditorPlayerId,
    int amount,
    string ledgerType
);

public record PlayerStarSpecialistHiredEventData(
    string starId,
    string starName,
    int specialistId,
    string specialistName,
    string specialistDescription
);

public record PlayerCarrierSpecialistHiredEventData(
    string carrierId,
    string carrierName,
    int specialistId,
    string specialistName,
    string specialistDescription
);

public record PlayerConversationCreatedEventData(
    string conversationId,
    string createdBy,
    string name,
    List<string> participants
);

public record PlayerConversationInvitedEventData(
    string conversationId,
    string name,
    string playerId
);

public record PlayerConversationLeftEventData(
    string conversationId,
    string name,
    string playerId
);

public record GamePlayerBadgePurchasedEventData(
    string purchasedByPlayerId,
    string purchasedByPlayerAlias,
    string purchasedForPlayerId,
    string purchasedForPlayerAlias,
    string badgeKey,
    string badgeName
);

public record PlayerDiplomacyStatusChangedEventData(
    string playerIdFrom,
    string playerIdTo,
    string playerFromAlias,
    string playerToAlias,
    string statusFrom,
    string statusTo,
    string actualStatus
);

public record GameEvent(
    string _id,
    string? playerId,
    bool? read,
    string gameId,
    int tick,
    string type
) {
    public virtual object? GetData() => null;
}

public record GameEvent<TData>(
    string _id,
    string? playerId,
    bool? read,
    string gameId,
    int tick,
    string type,
    TData? data
) : GameEvent(_id, playerId, read, gameId, tick, type) where TData : class {
    public override object? GetData() => data;
}

public record GameEventsResponse(
    int count,
    List<GameEvent> events
);
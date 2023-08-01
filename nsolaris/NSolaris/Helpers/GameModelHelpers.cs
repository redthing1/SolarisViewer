using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using NSolaris.Models;

namespace NSolaris.Helpers;

public static class GameModelHelpers {
    public static IEnumerable<JsonConverter> JsonConverters => new JsonConverter[] {
        new PlayerCombatShipCount.Converter(),
    };

    private static void RegisterJsonConverters(JsonSerializerOptions options) {
        foreach (var converter in JsonConverters) {
            options.Converters.Add(converter);
        }
    }

    public static JsonSerializerOptions CreateJsonSerializerOptions() {
        var options = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        };
        RegisterJsonConverters(options);
        return options;
    }

    /// <summary>
    /// helps with parsing game events, because data is a union type, so we need to create the correct TData based on the type string
    /// </summary>
    public static class GameEventsParserHelper {
        public static Type? GetEventDataType(string eventType) {
            return eventType switch {
                "gamePlayerJoined" => typeof(PlayerJoinedEventData),
                "gamePlayerQuit" => typeof(PlayerQuitEventData),
                "gamePlayerDefeated" => typeof(PlayerDefeatedEventData),
                "gamePlayerAFK" => typeof(PlayerAfkEventData),
                "gameStarted" => null,
                "gameEnded" => typeof(GameEndedEventData),
                "gamePlayerBadgePurchased" => typeof(GamePlayerBadgePurchasedEventData),
                "gameDiplomacyPeaceDeclared" => typeof(PlayerDiplomacyStatusChangedEventData),
                "gameDiplomacyWarDeclared" => typeof(PlayerDiplomacyStatusChangedEventData),
                "playerDiplomacyStatusChanged" => typeof(PlayerDiplomacyStatusChangedEventData),
                "playerGalacticCycleComplete" => typeof(PlayerGalacticCycleCompleteEventData),
                "playerCombatStar" => typeof(PlayerCombatStarEventData),
                "playerCombatCarrier" => typeof(PlayerCombatCarrierEventData),
                "playerResearchComplete" => typeof(PlayerResearchCompleteEventData),
                "playerTechnologyReceived" => typeof(PlayerTechnologyReceivedEventData),
                "playerTechnologySent" => typeof(PlayerTechnologySentEventData),
                "playerCreditsReceived" => typeof(PlayerCreditsReceivedEventData),
                "playerCreditsSent" => typeof(PlayerCreditsSentEventData),
                "playerCreditsSpecialistsReceived" => typeof(PlayerCreditsSpecialistsReceivedEventData),
                "playerCreditsSpecialistsSent" => typeof(PlayerCreditsSpecialistsSentEventData),
                "playerRenownReceived" => typeof(PlayerRenownReceivedEventData),
                "playerRenownSent" => typeof(PlayerRenownSentEventData),
                "playerGiftReceived" => typeof(PlayerGiftReceivedEventData),
                "playerGiftSent" => typeof(PlayerGiftSentEventData),
                "playerStarAbandoned" => typeof(PlayerStarAbandonedEventData),
                "playerStarDied" => typeof(PlayerStarDiedEventData),
                "playerStarReignited" => typeof(PlayerStarReignitedEventData),
                "playerBulkInfrastructureUpgraded" => typeof(PlayerBulkInfrastructureUpgradedEventData),
                "playerDebtSettled" => typeof(PlayerDebtSettledEventData),
                "playerDebtForgiven" => typeof(PlayerDebtForgivenEventData),
                "playerStarSpecialistHired" => typeof(PlayerStarSpecialistHiredEventData),
                "playerCarrierSpecialistHired" => typeof(PlayerCarrierSpecialistHiredEventData),
                "playerConversationCreated" => typeof(PlayerConversationCreatedEventData),
                "playerConversationInvited" => typeof(PlayerConversationInvitedEventData),
                "playerConversationLeft" => typeof(PlayerConversationLeftEventData),
                _ => null
            };
        }

        public static GameEventsResponse ParseGameEventsResponse(string json) {
            var jsonObj = JsonNode.Parse(json) as JsonObject;
            if (jsonObj == null)
                throw new Exception("json is not an object");
            var count = jsonObj["count"]?.AsValue().GetValue<int>() ?? throw new Exception("count is missing");
            var eventsJsonArr = jsonObj["events"]?.AsArray() ?? throw new Exception("events is missing");

            var gameEvents = new List<GameEvent>();

            foreach (var eventJson in eventsJsonArr) {
                var eventObj = eventJson as JsonObject;
                if (eventObj == null)
                    throw new Exception("event is not an object");
                var id = eventObj["_id"]?.AsValue().GetValue<string>() ?? throw new Exception("_id is missing");
                var playerId = eventObj["playerId"]?.AsValue()?.GetValue<string>();
                var read = eventObj["read"]?.AsValue()?.GetValue<bool>();
                var gameId = eventObj["gameId"]?.AsValue().GetValue<string>() ??
                             throw new Exception("gameId is missing");
                var tick = eventObj["tick"]?.AsValue().GetValue<int>() ?? throw new Exception("tick is missing");
                var type = eventObj["type"]?.AsValue().GetValue<string>() ?? throw new Exception("type is missing");
                var dataJson = eventObj["data"]?.AsObject();
                var eventDataType = GetEventDataType(type);
                if (eventDataType == null) {
                    gameEvents.Add(new GameEvent(id, playerId, read, gameId, tick, type));
                } else {
                    if (dataJson == null)
                        throw new ArgumentNullException(nameof(dataJson),
                            $"data expected to be of type {eventDataType} is null");
                    var dataJsonRaw = dataJson.ToString();
                    object gameEventData;
                    var serOptions = CreateJsonSerializerOptions();
                    try {
                        gameEventData = JsonSerializer.Deserialize(dataJsonRaw, eventDataType, serOptions)!;
                    }
                    catch (Exception e) {
                        throw new Exception(
                            $"failed to deserialize data of type {eventDataType}: {dataJsonRaw} - {e.Message}");
                    }

                    var eventRecordType = typeof(GameEvent<>).MakeGenericType(eventDataType);
                    GameEvent ev;
                    ev = (GameEvent)Activator.CreateInstance(eventRecordType, id, playerId, read, gameId, tick, type,
                        gameEventData)!;
                    gameEvents.Add(ev);
                }
            }

            return new GameEventsResponse(count, gameEvents);
        }
    }
}
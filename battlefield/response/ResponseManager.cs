namespace FSDClient.battlefield.response;

using System.Text.Json.Serialization;
using System.Text.Json;
using System;

public enum MessageType
{
    StateUpdate,
    ActionResult,
    Error
}

public enum ActionType
{
    JOIN_GAME,
    SELECT_CARD,
    DESELECT_CARD,
    CARD_PLACED,
    RECONNECT,
    SURRENDER,
    TICK_UPDATE
}

public class ResponseManager
{
    [JsonPropertyName("message_type")]
    public string MessageType { get; set; }
    
    [JsonPropertyName("action")]
    public string ActionType { get; set; }
    [JsonPropertyName("params")]
    public JsonElement Parameters { get; set; }
    
    [JsonPropertyName("result")]
    public string Result { get; set; }
    
    [JsonPropertyName("error_message")]
    public string ErrorMessage { get; set; }
    
    [JsonPropertyName("state_view")]
    public JsonElement StateView { get; set; }
    
    [JsonPropertyName("sequence_number")]
    public long SequenceNumber { get; set; }
    
    [JsonPropertyName("tick_number")]
    public ulong TickNumber { get; set; }
    
    [JsonPropertyName("timestamp")]
    public DateTime TimeStamp { get; set; }

}

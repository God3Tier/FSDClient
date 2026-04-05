namespace FSDClient.battlefield.responseType;

using System;
using System.Text.Json.Serialization;
using FSDClient.battlefield.responseType;

public class ResponseBase
{
    [JsonPropertyName("message_type")]
    public string MessageType;
    [JsonPropertyName("action")]
    public string Action;
    [JsonPropertyName("result")]
    public string Result;
    [JsonPropertyName("state_view")]
    public string StateView;
    [JsonPropertyName("sequence_number")]
    public int SequenceNumber;
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp;

}

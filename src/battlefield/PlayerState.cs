namespace FSDClient.battlefield.responseType;

using System.Text.Json.Serialization;
using System.Text;
using System.IO.Hashing;
using System;

public class PlayerState
{
	[JsonPropertyName("session_id")]
	public string SessionID { get; set; }
	[JsonPropertyName("phase")]
	public string Phase { get; set; }
	[JsonPropertyName("sequence_number")]
	public long SequenceNumber { get; set; }

	[JsonPropertyName("your_user_id")]
	public long YourUserID { get; set; }
	[JsonPropertyName("your_username")]
	public string YourUsername { get; set; }

	[JsonPropertyName("opponent_user_id")]
	public long OpponentUserID { get; set; }
	[JsonPropertyName("opponent_username")]
	public string OpponentUsername { get; set; }
	[JsonPropertyName("opponent_connected")]
	public bool OpponentConnected { get; set; }

    [JsonPropertyName("tick_number")]
    public ulong TickNumber { get; set; }

    [JsonPropertyName("state_hash")]
    public ulong StateHash { get; set; }


	private void WriteByte(XxHash64 hash, byte[] bytes)
	{
		hash.Append(bytes);
	}

	private void WriteString(XxHash64 hash, string value)
	{
		hash.Append(Encoding.UTF8.GetBytes(value ?? string.Empty));
	}

	private void WriteInt(XxHash64 hash, int val)
	{
		Span<byte> buffer = stackalloc byte[4];
		BitConverter.TryWriteBytes(buffer, val);
		if (!BitConverter.IsLittleEndian)
		{
			buffer.Reverse();
		}

		hash.Append(buffer);
	}

	private void WriteLong(XxHash64 hash, long val)
	{
		Span<byte> buffer = stackalloc byte[8];
		BitConverter.TryWriteBytes(buffer, val);
		if (!BitConverter.IsLittleEndian)
		{
			buffer.Reverse();

		}
		hash.Append(buffer);
	}

	private void WriteULong(XxHash64 hash, ulong val)
	{
		Span<byte> buffer = stackalloc byte[8];
		BitConverter.TryWriteBytes(buffer, val);
		if (!BitConverter.IsLittleEndian)
		{
			buffer.Reverse();
		}
		hash.Append(buffer);
	}


	public ulong HashPlayerView()
	{
		var hash = new XxHash64();

		// Hash all fields in deterministic order
		WriteString(hash, SessionID);
		WriteString(hash, Phase);
		WriteLong(hash, SequenceNumber);
		WriteULong(hash, TickNumber);
		
		// Your state
		WriteLong(hash, YourUserID);
		WriteString(hash, YourUsername);
		
		// Opponent's state
		WriteLong(hash, OpponentUserID);
		WriteString(hash, OpponentUsername);
		if (OpponentConnected)
		{
			WriteByte(hash, new byte[1] { 1 });
		}
		else
		{
			WriteByte(hash, new byte[1] { 0 });
		}
		
		return BitConverter.ToUInt64(hash.GetHashAndReset());
	}
}

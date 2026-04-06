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
	[JsonPropertyName("turn_number")]
	public int TurnNumber { get; set; }
	[JsonPropertyName("current_player")]
	public int CurrentPlayer { get; set; }
	[JsonPropertyName("sequence_number")]
	public long SequenceNumber { get; set; }

	[JsonPropertyName("your_user_id")]
	public long YourUserID { get; set; }
	[JsonPropertyName("your_username")]
	public string YourUsername { get; set; }
	[JsonPropertyName("your_game_data")]
	public string YourGameData { get; set; }

	[JsonPropertyName("opponent_user_id")]
	public long OpponentUserID { get; set; }
	[JsonPropertyName("opponent_username")]
	public string OpponentUsername { get; set; }
	[JsonPropertyName("opponent_connected")]
	public bool OpponentConnected { get; set; }
	[JsonPropertyName("opponent_game_data")]
	public string OpponentGameData { get; set; }



	private void WriteByte(XxHash64 hash, byte[] bytes)
	{
		hash.Append(bytes);
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


	public ulong HashPlayerView()
	{
		var hash = new XxHash64();

		// Hash all fields in deterministic order
		// Session and game metadata
		WriteByte(hash, Encoding.UTF8.GetBytes(SessionID));
		WriteByte(hash, Encoding.UTF8.GetBytes(Phase));
		WriteInt(hash, TurnNumber);
		WriteInt(hash, CurrentPlayer);
		WriteLong(hash, SequenceNumber);
		
		// Your state
		WriteLong(hash, YourUserID);
		WriteByte(hash, Encoding.UTF8.GetBytes(YourUsername));
		if (YourGameData != null)
		{
			WriteByte(hash, Encoding.UTF8.GetBytes(YourGameData));
		}
		
		// Opponent's state
		WriteLong(hash, OpponentUserID);
		WriteByte(hash, Encoding.UTF8.GetBytes(OpponentUsername));
		if (OpponentConnected)
		{
			WriteByte(hash, new byte[1] { 1 });
		}
		else
		{
			WriteByte(hash, new byte[1] { 0 });
		}
		
		if (OpponentGameData != null)
		{
			WriteByte(hash, Encoding.UTF8.GetBytes(OpponentGameData));
		}		
		
		return BitConverter.ToUInt64(hash.GetHashAndReset());
	}
}

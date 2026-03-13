public class GamePacket {

	private GamePacketStream buffer;

	public GamePacket(short message_id) {
		buffer = new GamePacketStream(message_id);
	}

	public void AddShort16(short val) {
		buffer.Add(val);
	}

	public void AddInt32(int val) {
		buffer.Add(val);
	}

	public void AddLong64(long val) {
		buffer.Add(val);
	}

	public void AddBool(bool val) {
		buffer.Add(val);
	}

	public void AddBytes(byte[] bytes) {
		buffer.Add(bytes);
	}

	public void AddString(string val) {
		buffer.Add((short) val.Length);
		buffer.Add(val);
	}
	
	public void AddFloat32(float val) {
	}
	
	public int Size() {
		return buffer.Size();
	}
	
	public byte[] GetBytes() {
		return buffer.ToByteArray();
	}
}

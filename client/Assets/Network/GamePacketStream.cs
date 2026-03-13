using System;
using System.IO;
using System.Text;

public class GamePacketStream {
	
	private MemoryStream stream;
	
	public GamePacketStream(short message_id) {
		stream = new MemoryStream();
		
		stream.WriteByte(0xff);
		stream.WriteByte(0xff);

		Add(message_id);
	}
	
	public void Add(byte[] bytes) {
		stream.Write(bytes, 0, bytes.Length);
	}
	
	public void Add(short val) {
		Add(BitConverter.GetBytes(val));
	}

	public void Add(int val) {
		Add(BitConverter.GetBytes(val));
	}
	
	public void Add(long val) {
		Add(BitConverter.GetBytes(val));
	}
	
	public void Add(bool val) {
		Add(BitConverter.GetBytes(val));
	}
	
	public void Add(string val) {
		Add(Encoding.UTF8.GetBytes(val));
	}

	public byte[] ToByteArray() {
		byte[] bytes = stream.ToArray();

		bytes[0] = (byte) ((stream.Length - 2) & 0xff);
		bytes[1] = (byte) ((stream.Length - 2) >> 8);
		
		return bytes;
	}
	
	public int Size() {
		return (int) stream.Length;
	}
}

using System.IO;

public abstract class NetworkResponse {
	
	public MemoryStream DataStream { get; set; }
	public short Response_id { get; set; }
	
	public abstract void Parse();
	public abstract ExtendedEventArgs Process();
}

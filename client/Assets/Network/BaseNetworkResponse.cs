using System.IO;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Base class for all network responses that handles common status and message parsing
/// All server responses extend GameResponse which writes status and message first
/// </summary>
public abstract class BaseNetworkResponse : NetworkResponse {
    
    protected short status;
    protected string message;
    
    public short GetStatus() { return status; }
    public string GetMessage() { return message; }
    public override sealed void Parse() {
        // Always read status and message first (from GameResponse base class)
        status = DataReader.ReadShort(DataStream);
        message = DataReader.ReadString(DataStream);

        if(Response_id != Constants.SMSG_HEARTBEAT)
        Debug.Log($"{GetType().Name} - Status: {status}, Message: {message}");

        // Only continue parsing if status is 0 (success) or if derived class handles errors
        if (status == 0 || ShouldParseOnError()) {
            ParseResponseData();
        } else {
            Debug.LogError($"{GetType().Name} failed - Status: {status}, Message: {message}");
        }
    }
    
    /// <summary>
    /// Override this to parse response-specific data after status and message
    /// </summary>
    protected abstract void ParseResponseData();
    
    /// <summary>
    /// Override this to return true if response should parse data even on error status
    /// Default is false (only parse on success)
    /// </summary>
    protected virtual bool ShouldParseOnError() {
        return false;
    }
    
    /// <summary>
    /// Utility method to add status and message to event args using reflection
    /// </summary>
    protected void SetBaseEventArgs(object eventArgs) {
        var type = eventArgs.GetType();
        
        var statusProperty = type.GetProperty("status");
        if (statusProperty != null && statusProperty.CanWrite) {
            statusProperty.SetValue(eventArgs, status, null);
        }
        
        var messageProperty = type.GetProperty("message");
        if (messageProperty != null && messageProperty.CanWrite) {
            messageProperty.SetValue(eventArgs, message, null);
        }
    }
}
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MessageQueue : MonoBehaviour {

	public delegate void Callback(ExtendedEventArgs eventArgs);
	public Dictionary<int, Callback> CallbackList { get; set; }
	public Queue<ExtendedEventArgs> MsgQueue { get; set; }

	void Awake() {
		CallbackList = new Dictionary<int, Callback>();
		MsgQueue = new Queue<ExtendedEventArgs>();
	}

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
		while (MsgQueue!=null && MsgQueue.Count > 0) {
			ExtendedEventArgs args = MsgQueue.Dequeue();
			if (CallbackList.ContainsKey(args.Event_id)) {
				CallbackList[args.Event_id](args);
				if (args.Event_id != Constants.SMSG_HEARTBEAT)
				{
					Debug.Log("Processed Event No. " + args.Event_id + " [" + args.GetType() + "]");
					Debug.Log("Processed Event No. " + args.Event_id + " [" + args.ToString() + "]");
				}				
			} else {
				Debug.Log("Missing Event No. " + args.Event_id + " [" + args.GetType() + "]");
			}
		}
	}
	
	public void AddCallback(int eventId, Callback callback) {
		if (CallbackList.ContainsKey(eventId))
		{
			CallbackList[eventId] = callback;
		}
		else
		{
			CallbackList.Add(eventId, callback);
		}
	}

	public void RemoveCallback(int eventId) {
		if(!CallbackList.ContainsKey(eventId)) {
			return;
		}
		CallbackList.Remove(eventId);
	}
	
	public void AddMessage(int eventId, ExtendedEventArgs args) {
		MsgQueue.Enqueue(args);
	}
}

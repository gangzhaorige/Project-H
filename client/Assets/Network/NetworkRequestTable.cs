using UnityEngine;

using System;
using System.Collections.Generic;

public class NetworkRequestTable {

	public static Dictionary<short, Type> requestTable { get; set; }

	public static void Init()
	{
		requestTable = new Dictionary<short, Type>();
		Add(Constants.CMSG_AUTH, "RequestLogin");
		Add(Constants.CMSG_HEARTBEAT, "RequestHeartBeat");
		Add(Constants.CMSG_LOGOUT, "RequestLogout");
		Add(Constants.CMSG_PICK_CHAMPION, "RequestPickChampion");
		Add(Constants.CMSG_SELECT_CHAMPION, "RequestSelectChampion");
		Add(Constants.CMSG_READY_FOR_CHAMPION_SELECT, "RequestReadyForChampionSelect");
		Add(Constants.CMSG_READY_FOR_GAME_SETUP, "RequestReadyForGameSetup");
		Add(Constants.CMSG_READY_TO_PLAY, "RequestReadyToPlay");
		Add(Constants.CMSG_PLAY_CARD, "RequestPlayCard");
		Add(Constants.CMSG_END_TURN, "RequestEndTurn");
		Add(Constants.CMSG_PASS_PRIORITY, "RequestPassPriority");
		Add(Constants.CMSG_SKILL_RESPONSE, "RequestSkillResponse");
		Add(Constants.CMSG_ACTIVATE_SKILL, "RequestActivateSkill");
		Add(Constants.CMSG_SELECT_CARDS, "RequestSelectCards");
		Add(Constants.CMSG_CONFIRMATION, "RequestConfirmation");

        // Room System
        Add(Constants.CMSG_CREATE_ROOM, "RequestCreateRoom");
        Add(Constants.CMSG_JOIN_ROOM, "RequestJoinRoom");
        Add(Constants.CMSG_LEAVE_ROOM, "RequestLeaveRoom");
        Add(Constants.CMSG_ALL_ROOMS, "RequestAllRooms");
        Add(Constants.CMSG_RECONNECT, "RequestReconnect");
        
        // Match Lifecycle
        Add(Constants.CMSG_GAME_START, "RequestStartGame");
        Add(Constants.CMSG_END_GAME, "RequestEndGame");
	}
	
	public static void Add(short request_id, string name) {
		requestTable.Add(request_id, Type.GetType(name));
	}
	
	public static NetworkRequest Get(short request_id) {
		NetworkRequest request = null;
		
		if (requestTable.ContainsKey(request_id)) {
			request = (NetworkRequest) Activator.CreateInstance(requestTable[request_id]);
			request.Request_id = request_id;
		} else {
			Debug.Log("Request [" + request_id + "] Not Found");
		}
		
		return request;
	}
}

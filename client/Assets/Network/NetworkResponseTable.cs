using UnityEngine;

using System;
using System.Collections.Generic;

public class NetworkResponseTable {

	public static Dictionary<short, Type> ResponseTable { get; set; }

	public static void Init()
	{
		ResponseTable = new Dictionary<short, Type>();
		Add(Constants.SMSG_AUTH, "ResponseLogin");
		Add(Constants.SMSG_LOGOUT, "ResponseLogout");
		Add(Constants.SMSG_GAME_START, "ResponseGameStart");
		Add(Constants.SMSG_PICK_CHAMPION, "ResponsePickChampion");
		Add(Constants.SMSG_NOTIFY_PLAYER_PICK, "ResponseNotifyPlayerPick");
		Add(Constants.SMSG_NOTIFY_PLAYER_SELECT, "ResponseNotifyPlayerSelect");
		Add(Constants.SMSG_NOTIFY_FOR_CHAMPION_PICK, "ResponseNotifyForChampionPick");
		Add(Constants.SMSG_CHAMPION_SELECT_COMPLETED, "ResponseChampionSelectCompleted");
		Add(Constants.SMSG_CHAMPION_SELECT_READY, "ResponseChampionSelectReady");
		Add(Constants.SMSG_START_CHAMPION_SELECTION, "ResponseStartChampionSelection");
		Add(Constants.SMSG_GAME_SETUP, "ResponseGameSetup");
		Add(Constants.SMSG_CARD_DRAW, "ResponseDrawCard");
		Add(Constants.SMSG_CARD_DRAW_OTHER, "ResponseDrawCardOther");
		Add(Constants.SMSG_PLAY_CARD, "ResponsePlayCard");
		Add(Constants.SMSG_END_TURN, "ResponseEndTurn");
		Add(Constants.SMSG_TURN_START, "ResponseTurnStart");
		Add(Constants.SMSG_HEARTBEAT, "ResponseHeartBeat");
		Add(Constants.SMSG_RESPONSE_TIMER_START, "ResponseTimerStart");
		Add(Constants.SMSG_RESPONSE_TIMER_CANCEL, "ResponseTimerCancel");
		Add(Constants.SMSG_PASS_PRIORITY, "ResponsePassPriority");
		Add(Constants.SMSG_PLAYER_STATS, "ResponsePlayerStats");
		Add(Constants.SMSG_ACTION_COMPLETED, "ResponseActionCompletedEvent");
		Add(Constants.SMSG_SKILL_ACTIVATION, "ResponseSkillActivated");
		Add(Constants.SMSG_SKILL_QUERY, "ResponseSkillQuery");
		Add(Constants.SMSG_JUDGE, "ResponseJudgement");
		Add(Constants.SMSG_SHOW_TARGET_HAND, "ResponseShowTargetHand");
		Add(Constants.SMSG_STATE_CHANGE, "ResponseGameState");
		Add(Constants.SMSG_CHAMPION_STATS_UPDATE_INTEGER, "ResponseChampionStatsUpdateInteger");
		Add(Constants.SMSG_UPDATE_ATTACK, "ResponseUpdateAttack");
		Add(Constants.SMSG_ADD_CARD, "ResponseAddCard");
		Add(Constants.SMSG_DISCARD_CARDS, "ResponseDiscardCard");
		Add(Constants.SMSG_CHANGE_JUDGEMENT, "ResponseChangeJudgement");
		Add(Constants.SMSG_SWAP_JUDGEMENT, "ResponseSwapJudgement");
		Add(Constants.SMSG_SWAP_FIELD_HAND, "ResponseSwapFieldHand");
		Add(Constants.SMSG_FIELD_TO_HAND, "ResponseFieldToHand");
		Add(Constants.SMSG_SELECT_CARDS_FROM_OPPONENT, "ResponseSelectCardsFromOpponent");
		Add(Constants.SMSG_SELECT_CARDS, "ResponseSelectCards");
		Add(Constants.SMSG_MOVE_CARD, "ResponseMoveCard");

        // Room System
        Add(Constants.SMSG_CREATE_ROOM, "ResponseCreateRoom");
        Add(Constants.SMSG_JOIN_ROOM, "ResponseJoinRoom");
        Add(Constants.SMSG_JOIN_ROOM_EXISTING, "ResponseJoinRoomExisting");
        Add(Constants.SMSG_LEAVE_ROOM, "ResponseLeaveRoom");
        Add(Constants.SMSG_ALL_ROOMS, "ResponseAllRooms");
        Add(Constants.SMSG_RECONNECT, "ResponseReconnect");
        
        // Match Lifecycle
        Add(Constants.SMSG_GAME_END, "ResponseGameEnd");
        Add(Constants.SMSG_MATCH_STATE, "ResponseMatchState");
	}
	
	public static void Add(short response_id, string name) {
		ResponseTable.Add(response_id, Type.GetType(name));
	}
	
	public static NetworkResponse Get(short response_id) {
		NetworkResponse response = null;
		if (ResponseTable != null && ResponseTable.ContainsKey(response_id)) {
			response = (NetworkResponse) Activator.CreateInstance(ResponseTable[response_id]);
			response.Response_id = response_id;
		} else {
			Debug.Log("Response [" + response_id + "] Not Found");
		}
		
		return response;
	}
}

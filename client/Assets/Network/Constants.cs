public class Constants {
	
	// Constants
	public static readonly string CLIENT_VERSION = "1.00";
	public static readonly string REMOTE_HOST = "localhost";
	public static readonly int REMOTE_PORT = 1729;
	
	// --- Core Systems (101-129) ---
    public static readonly short CMSG_AUTH = 101;
    public static readonly short SMSG_AUTH = 201;

    public static readonly short CMSG_HEARTBEAT = 102;
    public static readonly short SMSG_HEARTBEAT = 202;

    public static readonly short CMSG_PLAYERS = 103;
    public static readonly short SMSG_PLAYERS = 203;

    public static readonly short CMSG_LOGOUT = 104;
    public static readonly short SMSG_LOGOUT = 204;

    // --- Room System (130-149) ---
    public static readonly short CMSG_JOIN_ROOM = 130;
    public static readonly short SMSG_JOIN_ROOM = 230;

    public static readonly short CMSG_LEAVE_ROOM = 131;
    public static readonly short SMSG_LEAVE_ROOM = 231;

    public static readonly short CMSG_CREATE_ROOM = 132;
    public static readonly short SMSG_CREATE_ROOM = 232;

    public static readonly short CMSG_ALL_ROOMS = 133;
    public static readonly short SMSG_ALL_ROOMS = 233;

    public static readonly short CMSG_RECONNECT = 134;
    public static readonly short SMSG_RECONNECT = 234;

    public static readonly short CMSG_GET_ROOM_PLAYERS = 135;
    public static readonly short SMSG_GET_ROOM_PLAYERS = 235;

    public static readonly short SMSG_JOIN_ROOM_EXISTING = 236;

    // --- Match Lifecycle (150-169) ---
    public static readonly short CMSG_GAME_START = 150;
    public static readonly short SMSG_GAME_START = 250;

    public static readonly short CMSG_END_GAME = 151;
    public static readonly short SMSG_GAME_END = 251;

    public static readonly short SMSG_MATCH_STATE = 252;

    // --- Gameplay Logic (300+) ---
    // Champion Selection
    public static readonly short CMSG_READY_FOR_CHAMPION_SELECT = 300;
    public static readonly short SMSG_READY_FOR_CHAMPION_SELECT = 400;
    public static readonly short CMSG_PICK_CHAMPION = 301;
    public static readonly short SMSG_PICK_CHAMPION = 401;
    public static readonly short CMSG_SELECT_CHAMPION = 302;
    public static readonly short SMSG_NOTIFY_PLAYER_SELECT = 406;
    public static readonly short SMSG_NOTIFY_PLAYER_PICK = 402;
    public static readonly short SMSG_NOTIFY_FOR_CHAMPION_PICK = 403;
    public static readonly short SMSG_CHAMPION_SELECT_COMPLETED = 404;
    public static readonly short SMSG_START_CHAMPION_SELECTION = 405;
    public static readonly short SMSG_CHAMPION_SELECT_READY = 407;

    // Card System
    public static readonly short SMSG_CARD_DRAW = 410;
    public static readonly short CMSG_PLAY_CARD = 311;
    public static readonly short SMSG_PLAY_CARD = 411;
    public static readonly short SMSG_ADD_CARD = 412;
    public static readonly short SMSG_DISCARD_CARDS = 413;
    public static readonly short SMSG_FIELD_TO_HAND = 414;

    // Turn System
    public static readonly short CMSG_END_TURN = 320;
    public static readonly short SMSG_END_TURN = 420;
    public static readonly short SMSG_TURN_START = 421;

    // Timer/Stats/Action
    public static readonly short SMSG_RESPONSE_TIMER_START = 430;
    public static readonly short SMSG_RESPONSE_TIMER_CANCEL = 431;
    public static readonly short SMSG_PLAYER_STATS = 432;
    public static readonly short SMSG_ACTION_COMPLETED = 433;
    public static readonly short SMSG_STATE_CHANGE = 434;

    // Skills/Judgement
    public static readonly short CMSG_ACTIVATE_SKILL = 340;
    public static readonly short SMSG_SKILL_ACTIVATION = 440;
    public static readonly short SMSG_JUDGE = 441;
    public static readonly short SMSG_CHANGE_JUDGEMENT = 442;
    public static readonly short SMSG_SWAP_JUDGEMENT = 443;
    public static readonly short SMSG_SHOW_TARGET_HAND = 444;
    public static readonly short SMSG_UPDATE_ATTACK = 445;

    public static readonly short CMSG_CONFIRMATION = 350;

	// Other
	public static readonly short SUCCESS = 0;
	public static readonly short FAILED = 1;

    // Teams
    public static readonly int TEAM_BLUE = 0;
    public static readonly int TEAM_RED = 1;

	public static readonly string IMAGE_RESOURCES_PATH = "Images/";
	public static readonly string PREFAB_RESOURCES_PATH = "Prefabs/";
	public static readonly string TEXTURE_RESOURCES_PATH = "Textures/";

    public static readonly short AUTHENTICATION_FAILED = 2;
    public static readonly short ALREADY_IN_ROOM = 3;
    public static readonly short DUPLICATE_ROOM_NAME = 4;
    public static readonly short INVALID_ROOM_NAME = 5;
	
	// GUI Window IDs
	public enum GUI_ID {
		Login
	};

	public static int USER_ID = -1;
	public static string ROOM_ID = "";
	public static string ROOM_NAME = "";
	public static bool IS_HOST = false;
}

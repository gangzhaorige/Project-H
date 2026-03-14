package com.zzhgl.app.metadata;

/**
 * The Constants class stores important variables as constants for later use.
 */
public class Constants {

    // --- Core Systems (101-129) ---
    public final static short CMSG_AUTH = 101;
    public final static short SMSG_AUTH = 201;

    public final static short CMSG_HEARTBEAT = 102;
    public final static short SMSG_HEARTBEAT = 202;

    public final static short CMSG_PLAYERS = 103;
    public final static short SMSG_PLAYERS = 203;

    public final static short CMSG_LOGOUT = 104;
    public final static short SMSG_LOGOUT = 204;

    // --- Room System (130-149) ---
    public final static short CMSG_JOIN_ROOM = 130;
    public final static short SMSG_JOIN_ROOM = 230;

    public final static short CMSG_LEAVE_ROOM = 131;
    public final static short SMSG_LEAVE_ROOM = 231;

    public final static short CMSG_CREATE_ROOM = 132;
    public final static short SMSG_CREATE_ROOM = 232;

    public final static short CMSG_ALL_ROOMS = 133;
    public final static short SMSG_ALL_ROOMS = 233;

    public final static short CMSG_RECONNECT = 134;
    public final static short SMSG_RECONNECT = 234;

    public final static short CMSG_GET_ROOM_PLAYERS = 135;
    public final static short SMSG_GET_ROOM_PLAYERS = 235;

    public final static short SMSG_JOIN_ROOM_EXISTING = 236;

    // --- Match Lifecycle (150-169) ---
    public final static short CMSG_GAME_START = 150;
    public final static short SMSG_GAME_START = 250;

    public final static short CMSG_END_GAME = 151;
    public final static short SMSG_GAME_END = 251;

    public final static short SMSG_MATCH_STATE = 252;

    // --- Gameplay Logic (300+) ---
    // Champion Selection
    public final static short CMSG_READY_FOR_CHAMPION_SELECT = 300;
    public final static short SMSG_READY_FOR_CHAMPION_SELECT = 400;
    public final static short CMSG_PICK_CHAMPION = 301;
    public final static short SMSG_PICK_CHAMPION = 401;
    public final static short CMSG_SELECT_CHAMPION = 302;
    public final static short SMSG_NOTIFY_PLAYER_SELECT = 406;
    public final static short SMSG_NOTIFY_PLAYER_PICK = 402;
    public final static short SMSG_NOTIFY_FOR_CHAMPION_PICK = 403;
    public final static short SMSG_CHAMPION_SELECT_COMPLETED = 404;
    public final static short SMSG_START_CHAMPION_SELECTION = 405;
    public final static short SMSG_CHAMPION_SELECT_READY = 407;
    public final static short SMSG_GAME_SETUP = 408;

    // Card System
    public final static short SMSG_CARD_DRAW = 410;
    public final static short CMSG_PLAY_CARD = 311;
    public final static short SMSG_PLAY_CARD = 411;
    public final static short SMSG_ADD_CARD = 412;
    public final static short SMSG_DISCARD_CARDS = 413;
    public final static short SMSG_FIELD_TO_HAND = 414;

    // Turn System
    public final static short CMSG_END_TURN = 320;
    public final static short SMSG_END_TURN = 420;
    public final static short SMSG_TURN_START = 421;

    // Timer/Stats/Action
    public final static short SMSG_RESPONSE_TIMER_START = 430;
    public final static short SMSG_RESPONSE_TIMER_CANCEL = 431;
    public final static short SMSG_PLAYER_STATS = 432;
    public final static short SMSG_ACTION_COMPLETED = 433;
    public final static short SMSG_STATE_CHANGE = 434;

    // Skills/Judgement
    public final static short CMSG_ACTIVATE_SKILL = 340;
    public final static short SMSG_SKILL_ACTIVATION = 440;
    public final static short SMSG_JUDGE = 441;
    public final static short SMSG_CHANGE_JUDGEMENT = 442;
    public final static short SMSG_SWAP_JUDGEMENT = 443;
    public final static short SMSG_SHOW_TARGET_HAND = 444;
    public final static short SMSG_UPDATE_ATTACK = 445;

    public final static short CMSG_CONFIRMATION = 350;
    public final static short CMSG_READY_FOR_GAME_SETUP = 351;
    public final static short CMSG_READY_TO_PLAY = 352;

    // --- Other ---
    public static final float BIOMASS_SCALE = 1000;
    public static final String CLIENT_VERSION = "1.00";
    public static final int TIMEOUT_SECONDS = 5;
    public static final String CSV_SAVE_PATH = "src/log/";

    // --- Status Codes ---
    public final static short SUCCESS = 0;
    public final static short FAILED = 1;
    
    // Teams
    public final static int TEAM_BLUE = 0;
    public final static int TEAM_RED = 1;
    public final static short AUTHENTICATION_FAILED = 2;
    public final static short ALREADY_IN_ROOM = 3;
    public final static short DUPLICATE_ROOM_NAME = 4;
    public final static short INVALID_ROOM_NAME = 5;

}

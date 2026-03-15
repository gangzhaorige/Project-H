package com.zzhgl.app.metadata;


// Java Imports
import java.util.HashMap;
import java.util.Map;

import com.zzhgl.app.networking.request.GameRequest;
import com.zzhgl.app.utility.Log;

// Other Imports


/**
 * The GameRequestTable class stores a mapping of unique request code numbers
 * with its corresponding request class.
 */
public class GameRequestTable<T> {

    private static Map<Short, Class<?>> requestTable = new HashMap<Short, Class<?>>(); // Request Code -> Class

    /**
     * Initialize the hash map by populating it with request codes and classes.
     */
    public static void init() {
        add(Constants.CMSG_AUTH, "RequestLogin", "authentication");
        add(Constants.CMSG_HEARTBEAT, "RequestHeartbeat", "system");
        add(Constants.CMSG_CREATE_ROOM, "RequestCreateRoom", "room");
        add(Constants.CMSG_JOIN_ROOM, "RequestJoinRoom", "room");
        add(Constants.CMSG_LEAVE_ROOM, "RequestLeaveRoom", "room");
        add(Constants.CMSG_ALL_ROOMS, "RequestAllRooms", "room");
        add(Constants.CMSG_RECONNECT, "RequestReconnect", "authentication");
        add(Constants.CMSG_GAME_START, "RequestStartGame", "champSelect");
        add(Constants.CMSG_END_GAME, "RequestEndGame", "champSelect");
        add(Constants.CMSG_READY_FOR_CHAMPION_SELECT, "RequestReadyForChampionSelect", "champSelect");
        add(Constants.CMSG_PICK_CHAMPION, "RequestPickChampion", "champSelect");
        add(Constants.CMSG_SELECT_CHAMPION, "RequestSelectChampion", "champSelect");
        add(Constants.CMSG_READY_FOR_GAME_SETUP, "RequestReadyForGameSetup", "game");
        add(Constants.CMSG_READY_TO_PLAY, "RequestReadyToPlay", "game");
        add(Constants.CMSG_END_TURN, "RequestEndTurn", "game");
        add(Constants.CMSG_SKILL_RESPONSE, "RequestSkillResponse", "game");
        // Matchmaking
    }

    /**
     * Map the request code number with its corresponding request class, derived
     * from its class name using reflection, by inserting the pair into the
     * table.
     *
     * @param code a value that uniquely identifies the request type
     * @param name a string value that holds the name of the request class
     */
    public static void add(short code, String className, String category) {
        try {
            String fullPath = "com.zzhgl.app.networking.request." + (category.isEmpty() ? "" : category + ".") + className;
            requestTable.put(code, Class.forName(fullPath));
        } catch (ClassNotFoundException e) {
            Log.println_e("Failed to load class for code " + code + ": " + e.getMessage());
        }
    }
    /**
     * Get the instance of the request class by the given request code.
     *
     * @param request_code a value that uniquely identifies the request type
     * @return the instance of the request class
     */
    public static GameRequest get(short request_code) {
        GameRequest request = null;
        try {
            Class<?> name = requestTable.get(request_code);
            if (name != null) {
                request = (GameRequest) name.getDeclaredConstructor().newInstance();
                request.setID(request_code);
            } else {
                Log.printf_e("Request Code [%d] does not exist!\n", request_code);
            }
        } catch (Exception e) {
            Log.println_e(e.getMessage());
        }
        return request;
    }
}

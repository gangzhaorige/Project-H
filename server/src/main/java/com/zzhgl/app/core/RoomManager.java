package com.zzhgl.app.core;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.UUID;
import java.util.concurrent.ConcurrentHashMap;

import com.zzhgl.app.model.core.Player;
import com.zzhgl.app.model.core.Room;
import com.zzhgl.app.networking.response.game.ResponseGameEnd;

public class RoomManager {
    private static RoomManager instance;
    private Map<String, Room> roomsById;
    private Map<String, Room> roomsByName;

    private RoomManager() {
        roomsById = new ConcurrentHashMap<>();
        roomsByName = new ConcurrentHashMap<>();
    }

    public static RoomManager getInstance() {
        if (instance == null) {
            instance = new RoomManager();
        }
        return instance;
    }

    public Room createRoom(String name, Player host) {
        if (roomsByName.containsKey(name)) {
            return null; // Duplicate name
        }
        String id = UUID.randomUUID().toString();
        Room room = new Room(id, name, host.getID());
        room.addPlayer(host);
        roomsById.put(id, room);
        roomsByName.put(name, room);
        com.zzhgl.app.utility.Log.printf("RoomManager: Room '%s' created with ID starting with '%s'", 
            name, id.substring(0, 8));
        return room;
    }

    public Room getRoomById(String id) {
        return roomsById.get(id);
    }

    public Room getRoomByName(String name) {
        com.zzhgl.app.utility.Log.printf("RoomManager: Searching for room name '%s' in %d rooms.", 
            name, roomsByName.size());
        if (!roomsByName.isEmpty()) {
            com.zzhgl.app.utility.Log.printf("RoomManager: Available names: %s", 
                String.join(", ", roomsByName.keySet()));
        }
        return roomsByName.get(name);
    }

    public void removeRoom(String id) {
        Room room = roomsById.remove(id);
        if (room != null) {
            roomsByName.remove(room.getName());
            room.stop();
        }
    }
    
    public void endGame(String roomId) {
        Room room = roomsById.get(roomId);
        if (room != null) {
            // Notify all connected players that the game has ended
            ResponseGameEnd endResponse = new ResponseGameEnd();
            room.broadcast(endResponse);

            // Detach players from the room
            for (Player player : room.getPlayers()) {
                player.setCurrentRoom(null);
            }
            
            // Destroy the room
            removeRoom(roomId);
            com.zzhgl.app.utility.Log.printf("Game in room %s ended and room removed.", room.getName());
        }
    }
    
    public List<Room> getAllRooms() {
        return new ArrayList<>(roomsById.values());
    }
}

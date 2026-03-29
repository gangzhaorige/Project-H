package com.zzhgl.app.model.core;

import java.util.HashMap;
import java.util.Map;

/**
 * GameEvent represents something that happened in the game logic.
 */
public class GameEvent {
    public enum EventType {
        TURN_BEGIN,
        TURN_END,
        DAMAGE_TAKEN,
        CARD_PLAYED,
        BEFORE_CARD_PLAYED,
        CARD_DISCARDED,
        SKILL_ACTIVATED,
        BEFORE_JUDGEMENT_RESOLVE
    }

    private final EventType type;
    private final Map<String, Object> params = new HashMap<>();
    private Object data;

    public GameEvent(EventType type) {
        this.type = type;
    }

    public EventType getType() { return type; }

    public void setData(Object data) { this.data = data; }
    public Object getData() { return data; }

    public GameEvent setParam(String key, Object value) {
        params.put(key, value);
        return this;
    }

    public Object getParam(String key) {
        return params.get(key);
    }
}

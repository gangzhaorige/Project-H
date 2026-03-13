package com.zzhgl.app.model.skills;

/**
 * AbstractSkill represents a unique skill that can be possessed by a champion.
 */
public abstract class AbstractSkill {
    protected int id;
    protected String name;

    public AbstractSkill(int id, String name) {
        this.id = id;
        this.name = name;
    }

    public int getId() { return id; }
    public String getName() { return name; }
}

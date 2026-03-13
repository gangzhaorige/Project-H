package com.zzhgl.app.model.champions;

/**
 * The Champion class represents a game character with various combat attributes.
 */
public class Champion {
    private int id;
    private String championName;
    private int pathId;
    private String element;
    private int curHP;
    private int maxHP;
    private int attackRange = 1;
    private int specialRange = 1;
    private int curNumOfAttack;
    private int maxNumOfAttack = 1;
    private int specialDefenseRange = 0;
    private int maxTarget = 1;

    public Champion() {}

    public Champion(int id, String championName, int maxHP) {
        this.id = id;
        this.championName = championName;
        this.maxHP = maxHP;
        this.curHP = maxHP;
        this.curNumOfAttack = 0;
    }

    // Getters and Setters
    public int getId() { return id; }
    public void setId(int id) { this.id = id; }

    public String getChampionName() { return championName; }
    public void setChampionName(String championName) { this.championName = championName; }

    public int getPathId() { return pathId; }
    public void setPathId(int pathId) { this.pathId = pathId; }

    public String getElement() { return element; }
    public void setElement(String element) { this.element = element; }

    public int getCurHP() { return curHP; }
    public void setCurHP(int curHP) { this.curHP = curHP; }

    public int getMaxHP() { return maxHP; }
    public void setMaxHP(int maxHP) { this.maxHP = maxHP; }

    public int getAttackRange() { return attackRange; }
    public void setAttackRange(int attackRange) { this.attackRange = attackRange; }

    public int getSpecialRange() { return specialRange; }
    public void setSpecialRange(int specialRange) { this.specialRange = specialRange; }

    public int getCurNumOfAttack() { return curNumOfAttack; }
    public void setCurNumOfAttack(int curNumOfAttack) { this.curNumOfAttack = curNumOfAttack; }

    public int getMaxNumOfAttack() { return maxNumOfAttack; }
    public void setMaxNumOfAttack(int maxNumOfAttack) { this.maxNumOfAttack = maxNumOfAttack; }

    public int getSpecialDefenseRange() { return specialDefenseRange; }
    public void setSpecialDefenseRange(int specialDefenseRange) { this.specialDefenseRange = specialDefenseRange; }

    public int getMaxTarget() { return maxTarget; }
    public void setMaxTarget(int maxTarget) { this.maxTarget = maxTarget; }
}

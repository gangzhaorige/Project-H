package com.zzhgl.app.model.champions;

import com.google.gson.annotations.SerializedName;
import com.zzhgl.app.model.skills.AbstractSkill;
import java.util.ArrayList;
import java.util.List;

/**
 * The Champion class represents a game character with various combat attributes.
 */
public class Champion {
    // Stat IDs
    public static final int STAT_CUR_HP = 1;
    public static final int STAT_MAX_HP = 2;
    public static final int STAT_ATTACK = 3;
    public static final int STAT_ATTACK_RANGE = 4;
    public static final int STAT_SPECIAL_RANGE = 5;
    public static final int STAT_CUR_NUM_ATTACK = 6;
    public static final int STAT_MAX_NUM_ATTACK = 7;
    public static final int STAT_SPECIAL_DEFENSE_RANGE = 8;
    public static final int STAT_ADDITIONAL_TARGET_FOR_ATTACK = 9;
    public static final int STAT_PATH_ID = 10;
    public static final int STAT_ELEMENT = 11;

    public enum Element {
        @SerializedName("Physical") PHYSICAL(1),
        @SerializedName("Fire") FIRE(2),
        @SerializedName("Ice") ICE(3),
        @SerializedName("Lightning") LIGHTNING(4),
        @SerializedName("Wind") WIND(5),
        @SerializedName("Quantum") QUANTUM(6),
        @SerializedName("Imaginary") IMAGINARY(7);

        private final int id;

        Element(int id) {
            this.id = id;
        }

        public int getId() {
            return id;
        }

        public static Element fromId(int id) {
            for (Element e : values()) {
                if (e.id == id) return e;
            }
            return PHYSICAL;
        }
    }

    public enum Path {
        @SerializedName("Destruction") DESTRUCTION(0),
        @SerializedName("Hunt") HUNT(1),
        @SerializedName("Erudition") ERUDITION(2),
        @SerializedName("Harmony") HARMONY(3),
        @SerializedName("Nihility") NIHILITY(4),
        @SerializedName("Preservation") PRESERVATION(5),
        @SerializedName("Abundance") ABUNDANCE(6);

        private final int id;

        Path(int id) {
            this.id = id;
        }

        public int getId() {
            return id;
        }

        public static Path fromId(int id) {
            for (Path p : values()) {
                if (p.id == id) return p;
            }
            return DESTRUCTION;
        }
    }

    private int id;
    private String championName;
    private Path path = Path.DESTRUCTION;
    private Element element = Element.PHYSICAL;
    private int curHP;
    private int maxHP;
    private int attack = 1;
    private int attackRange = 1;
    private int specialRange = 1;
    private int curNumOfAttack = 0;
    private int maxNumOfAttack = 1;
    private int specialDefenseRange = 0;
    private int additionalTargetForAttack = 0;
    private List<AbstractSkill> skills = new ArrayList<>();
    private List<Integer> skillIds = new ArrayList<>();

    public Champion() {}

    public Champion(int id, String championName, int maxHP) {
        this.id = id;
        this.championName = championName;
        this.maxHP = maxHP;
        this.curHP = maxHP;
    }

    // Getters and Setters
    public int getId() { return id; }
    public void setId(int id) { this.id = id; }

    public String getChampionName() { return championName; }
    public void setChampionName(String championName) { this.championName = championName; }

    public Path getPath() { return path; }
    public void setPath(Path path) { this.path = path; }

    public Element getElement() { return element; }
    public void setElement(Element element) { this.element = element; }

    public int getCurHP() { return curHP; }
    public void setCurHP(int curHP) { this.curHP = curHP; }

    public int getMaxHP() { return maxHP; }
    public void setMaxHP(int maxHP) { this.maxHP = maxHP; }

    public int getAttack() { return attack; }
    public void setAttack(int attack) { this.attack = attack; }

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

    public int getAdditionalTargetForAttack() { return additionalTargetForAttack; }
    public void setAdditionalTargetForAttack(int additionalTargetForAttack) { this.additionalTargetForAttack = additionalTargetForAttack; }

    public List<AbstractSkill> getSkills() { return skills; }
    public void addSkill(AbstractSkill skill) { this.skills.add(skill); }

    public List<Integer> getSkillIds() { return skillIds; }
    public void setSkillIds(List<Integer> skillIds) { this.skillIds = skillIds; }
}

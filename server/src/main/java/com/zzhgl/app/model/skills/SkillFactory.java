package com.zzhgl.app.model.skills;

import java.util.ArrayList;
import java.util.List;

/**
 * SkillFactory creates skill instances by ID.
 */
public class SkillFactory {
    private static SkillFactory instance;

    private SkillFactory() {}

    public static SkillFactory getInstance() {
        if (instance == null) {
            instance = new SkillFactory();
        }
        return instance;
    }

    public AbstractSkill createSkill(int skillId) {
        switch (skillId) {
            case 1: return new DrawTwoOnTurnBeginSkill();
            case 2: return new DrawOnDamageSkill();
            case 3: return new DrawOnLastCardSkill();
            case 4: return new JudgementOverrideSkill();
            case 5: return new JudgementSwapperSkill();
            default: return null;
        }
    }

    public List<AbstractSkill> createSkills(List<Integer> skillIds) {
        List<AbstractSkill> skills = new ArrayList<>();
        if (skillIds != null) {
            for (int id : skillIds) {
                AbstractSkill skill = createSkill(id);
                if (skill != null) {
                    skills.add(skill);
                }
            }
        }
        return skills;
    }
}

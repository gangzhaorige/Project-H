package com.zzhgl.app.model.champions;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.zzhgl.app.model.skills.SkillFactory;
import com.zzhgl.app.utility.Log;

import java.io.InputStream;
import java.io.InputStreamReader;
import java.lang.reflect.Type;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * ChampionFactory handles the loading and creation of Champion instances.
 */
public class ChampionFactory {
    private static ChampionFactory instance;
    private Map<Integer, Champion> championPrototypes;
    private final Gson gson = new Gson();

    private ChampionFactory() {
        championPrototypes = new HashMap<>();
        loadChampions();
    }

    public static ChampionFactory getInstance() {
        if (instance == null) {
            instance = new ChampionFactory();
        }
        return instance;
    }

    private void loadChampions() {
        try {
            InputStream is = getClass().getResourceAsStream("/champions.json");
            if (is == null) {
                Log.printf("Error: champions.json not found!");
                return;
            }
            InputStreamReader reader = new InputStreamReader(is);
            Type listType = new TypeToken<List<Champion>>(){}.getType();
            List<Champion> champions = gson.fromJson(reader, listType);
            for (Champion champ : champions) {
                championPrototypes.put(champ.getId(), champ);
            }
            Log.printf("Loaded %d champion prototypes.", championPrototypes.size());
        } catch (Exception e) {
            Log.printf("Error loading champions: %s", e.getMessage());
            e.printStackTrace();
        }
    }

    public Champion createChampion(int id) {
        Champion prototype = championPrototypes.get(id);
        if (prototype == null) {
            Log.printf("Warning: Champion ID %d not found!", id);
            return null;
        }
        
        // Clone the prototype
        Champion newChamp = new Champion();
        newChamp.setId(prototype.getId());
        newChamp.setChampionName(prototype.getChampionName());
        newChamp.setPath(prototype.getPath());
        newChamp.setElement(prototype.getElement());
        newChamp.setMaxHP(prototype.getMaxHP());
        newChamp.setCurHP(prototype.getMaxHP());
        newChamp.setAttack(prototype.getAttack());
        newChamp.setAttackRange(prototype.getAttackRange());
        newChamp.setSpecialRange(prototype.getSpecialRange());
        newChamp.setMaxNumOfAttack(prototype.getMaxNumOfAttack());
        newChamp.setCurNumOfAttack(0);
        newChamp.setSpecialDefenseRange(prototype.getSpecialDefenseRange());
        newChamp.setAdditionalTargetForAttack(prototype.getAdditionalTargetForAttack());
        
        // Add skills from SkillFactory
        newChamp.setSkillIds(prototype.getSkillIds());
        newChamp.getSkills().addAll(SkillFactory.getInstance().createSkills(prototype.getSkillIds()));
        
        return newChamp;
    }
}

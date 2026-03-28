package com.zzhgl.app.model.champions;

import com.google.gson.annotations.SerializedName;

public class ChampionAudioConfig {
    private int id;
    private String damageTakenAudioFile;
    // Add other audio event files here as needed (e.g., skillUseAudioFile, selectionAudioFile)

    public ChampionAudioConfig() {}

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getDamageTakenAudioFile() {
        return damageTakenAudioFile;
    }

    public void setDamageTakenAudioFile(String damageTakenAudioFile) {
        this.damageTakenAudioFile = damageTakenAudioFile;
    }
}

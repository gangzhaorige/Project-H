package com.zzhgl.app.model.Command;

import com.zzhgl.app.model.core.GameManager;

public interface Command {
    void execute(GameManager game);
}

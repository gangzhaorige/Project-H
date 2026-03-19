package com.zzhgl.app.model.core;

import com.zzhgl.app.model.interactions.AbstractInteraction;
import java.util.Stack;

public class InteractionStack {
    private Stack<AbstractInteraction> stack;

    public InteractionStack() {
        this.stack = new Stack<>();
    }

    public void push(AbstractInteraction interaction) {
        stack.push(interaction);
    }

    public AbstractInteraction pop() {
        if (!stack.isEmpty()) {
            return stack.pop();
        }
        return null;
    }

    public AbstractInteraction peek() {
        if (!stack.isEmpty()) {
            return stack.peek();
        }
        return null;
    }

    public boolean isEmpty() {
        return stack.isEmpty();
    }
    
    public void clear() {
        stack.clear();
    }
    
    public int size() {
        return stack.size();
    }
}
package com.zzhgl.app.networking.request.system;


// Java Imports
import java.io.IOException;

import com.zzhgl.app.networking.request.GameRequest;
import com.zzhgl.app.networking.response.system.ResponseHeartbeat;


/**
 * The RequestHeartbeat class is mainly used to release all pending responses
 * the client. Also used to keep the connection alive.
 */
public class RequestHeartbeat extends GameRequest {

    public RequestHeartbeat() {
        responses.add(new ResponseHeartbeat());
    }

    @Override
    public void parse() throws IOException {
    }

    @Override
    public void doBusiness() throws Exception {
        // Heartbeat purely acts as a keep-alive; responses are now pushed immediately
    }
}

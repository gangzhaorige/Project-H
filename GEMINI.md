# Card Game Project

## Project Overview

This repository contains a multiplayer card game featuring a client-server architecture. The project is split into two main components: a Unity-based game client and a Java-based backend server. The client and server communicate using a custom binary TCP protocol.

- **Client (`/client`)**: A Unity project built with C#. It handles the game's UI, input, rendering, and network communication via custom `NetworkManager` and `ConnectionManager` components.
- **Server (`/server`)**: A Java 17 Maven project named `HonkaiServer`. It operates as a multithreaded TCP server using `java.net.ServerSocket`, allocating a `GameClient` thread for each connected player.

## Directory Structure

- **`/client/`**: The Unity project root.
  - `Assets/Network/`: Contains all custom networking scripts for the client, including packet definition and TCP socket management.
  - `Assets/Scenes/`: Unity scenes (e.g., `Start.unity`).
- **`/server/`**: The Java Maven backend root.
  - `pom.xml`: Maven configuration file specifying dependencies (like JUnit 5) and build plugins.
  - `conf/`: Server configuration files, such as `gameServer.conf` which dictates settings like the server `portNumber`.
  - `src/main/java/com/zzhgl/app/`: The core Java source code, including the `GameServer` entry point, model definitions, and networking logic.

## Building and Running

### Server
The server is built using Maven. Navigate to the `/server` directory to run these commands:

- **Build**: `mvn clean package`
- **Run**: You can run the main class directly via your IDE, or via Maven:
  ```bash
  mvn exec:java -Dexec.mainClass="com.zzhgl.app.core.GameServer"
  ```
  *(Note: Ensure the server is run from the `/server` directory so it can correctly locate `conf/gameServer.conf`)*

### Client
- Open the `/client` directory using the **Unity Editor**.
- Navigate to `Assets/Scenes/` and open the starting scene (e.g., `Start.unity`) to play or test the game within the editor.
- The connection parameters are defined in `Constants` (like `REMOTE_HOST` and `REMOTE_PORT`) and should match the server's configuration.

## Development Conventions

- **Networking Protocol**: Both client and server utilize a custom request/response binary message protocol. Packets are identified by a short integer (e.g., `CMSG_HEARTBEAT`). When adding new network features, you must update both the client's `NetworkRequestTable`/`NetworkResponseTable` and the server's `GameRequestTable`.
- **Architecture**: 
  - **Server**: Utilizes a Thread Pool (`Executors.newCachedThreadPool()`) to handle incoming connections, dispatching them to `GameClient` runnables. It relies heavily on singletons (e.g., `GameServer.getInstance()`).
  - **Client**: The `NetworkManager` is set up as a persistent Singleton across scenes (`DontDestroyOnLoad`). It relies on `ConnectionManager` for raw TCP socket communication and a `MessageQueue` for dispatching events to the main Unity thread.
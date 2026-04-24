# FSD Digital Collectible Card Game (CCG) Client

A professional-grade Digital Collectible Card Game client built using the **Godot Engine** and **C#**. This project serves as the front-end interface for a multiplayer card game, featuring real-time battles, collection management, and a robust networking layer.

## 🚀 Project Overview

This client manages the end-to-end player experience, from account authentication to high-stakes tactical combat. It is designed to communicate with a backend server to store data and have multi player capabilities.

## ✨ Key Features

*   **User Authentication**: Full login and registration flow integrated with `NetworkManager`.
*   **Card Management**: 
    *   **Deck Builder**: Create and edit multiple decks.
    *   **Card Upgrades**: Level up cards using "Crystals" and duplicate copies to boost Attack and HP stats.
    *   **Validation**: Automatic enforcement of deck rules (12 cards per deck, max 2 duplicates).
*   **Economy & Gacha**: 
    *   **Pack Opening**: Animated opening of different pack rarities (Common to Legendary).
    *   **Currency Tracking**: Real-time updates for Crystals.
*   **Multiplayer Battle System**:
    *   **Matchmaking**: Queue-based system with reconnection support for active sessions.
    *   **Grid Combat**: Tactical 3x2 board placement.
    *   **Real-time Sync**: WebSocket-driven state updates ensures both players see the same board.
    *   **Resource Management**: Dynamic "Elixir" generation based on turn rounds.

## 🏁 Getting Started

### Prerequisites
*   **Godot Engine 4.6 (.NET Edition)**: You must use the .NET version of Godot to support the C# scripts.
*   **.NET SDK**: Ensure you have a version compatible with your Godot installation (e.g., .NET 10.0.102).
*   **Backend Environment**: A running instance of the FSD Backend (REST and WebSocket servers) is required for full functionality.

### Installation & Launch
1.  **Clone the Repository**:
    ```bash
    git clone https://github.com/YourUsername/FSDClient.git
    ```
2.  **Open in Godot**: Launch the Godot Engine and import the project using the `project.godot` file.

#### Debug
3.  **Build Solution**: Click the **Build** icon in the top right of the Godot Editor (or press `Ctrl+B`) to compile the C# code.
4.  **Configuration**:
    *   Update the `BASE_URL` in `NetworkManager.cs` to match your backend API.
    *   Ensure the WebSocket URL in `Gameloop.cs` points to your active game server.
5.  **Run**: Press **F5** or the **Play** button. The game starts at the `login_menu.tscn` scene.

#### Export to game file

## 🛠 Technical Architecture

### Core Managers (Autoloads)
*   **`GameStateManager.cs`**: The central hub for scene transitions. It manages the current application state (Login, Home, Cards, Gameplay) and handles safe scene instantiation.
*   **`PlayerStateManager.cs`**: A global singleton that stores the logged-in player's profile, including their JWT token, user ID, currency balances, and active deck configuration.
*   **`NetworkManager.cs`**: Handles RESTful communication with the backend API, managing headers and token injection for authorized requests.

### Networking Logic
*   **REST API**: Used for "heavy" data like fetching collections, saving decks, and opening packs.
*   **WebSockets (`Gameloop.cs`)**: Utilizes `WebSocketPeer` for the battle phase. It handles complex message types like `TICK_UPDATE`, `CARD_PLACED`, and `PHASE_CHANGE` (e.g., transitioning between `PRE_TURN` and `ACTIVE`).

## 🎮 How to play

### 1. Authentication
Launch the client and use the **Login Menu**. If you don't have an account, switch to the **Register** state to create one. Your session token will be saved in the `PlayerStateManager`.

### 2. Managing Your Collection
Navigate to the **Card Management** screen.
*   **Building Decks**: Select a deck tab (1, 2, or 3) and move cards from your collection into the deck.
*   **Leveling Up**: Click on a card and select **Info**. If you have enough duplicate copies and Crystals, you can level the card up to improve its performance in battle.
*   **Saving**: Ensure your deck has exactly **12 cards** (max 2 duplicate copies per card) before hitting **Save**.

### 3. Entering Battle
On the **Home Screen**, ensure you have an active deck selected and press **Battle**. 
*   The client will enter a matchmaking queue.
*   Once a match is found, you will automatically transition to the **Battlefield**.

### 4. Gameplay Mechanics
*   **Placement**: Drag cards from your hand onto the 3x2 grid. Each card costs Elixir.
*   **Phases**: 
    *   **PRE_TURN**: Review the board; the deck area is raised.
    *   **ACTIVE**: Combat occurs; Elixir regenerates, and cards interact.
*   **Winning**: Reduce the opponent's health (visible on their icon) to zero while defending your own.

## 📁 Folder Structure

*   `assets/`: All image assets stored here
*   `godotResource/`: godot resources used to store character stats
*   `scenes/`: Visual tscn files here
*   `src/`: Core logic files stored here
*   `styles/`: Fixed style files (.tres) used for visual elements

## How To Export Cilent

* After cloning the repo, launch godot_mono and open the project folder
* Go to Project > Export...
* - If godot warns you about missing templates, click on manage export templates on the bottom left, then go online mode, and select best mirror to download
* Once that is done, Export Project
* Godot will then generate the dmg file to executable

## ⚖️ Gameplay Rules

*   **Deck Size**: Exactly 12 cards.
*   **Duplicates**: Max 2 of the same Card ID per deck.
*   **Turns**: There is a draw phase where you will see cards from your deck, and are able to add cards to your hand, and a playing phase where you play cards from your hand onto the battefield.
*   **Elixir**: Maximum cap of 8. Players start with 5 elixir on turn 1, and the cap gets increased up to 8 with each passing turn. Elixir is regenerated only during playing phase.
*   **Board**: Two 3x2 grids (Player vs. Opponent).
*   **Cards**: Once a card is played onto the battlefield, it will automatically charge its attack meter and attack whatever is in front of it (unless otherwise mentioned in the card). If there is nothing in front of it, it will attack the hero.
*   **Hero**: Your own hero will counterattack cards that attacked it first. When you reduce your opponent's HP to 0 or below, you win, and vice versa if your hero's HP gets reduced to 0 or below, you lose.

---
*Developed as a Modular CCG Framework.*

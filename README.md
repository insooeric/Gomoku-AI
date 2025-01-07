# Gomoku-AI

This project implements a Gomoku (also known as Five in a Row) game AI with **Renju** rules using the **Minimax** algorithm for move evaluation and selection.

Below is an overview of the project's logic, structure, and how to use it.

---

## Table of Contents

1. [Overview](#overview)  
2. [Structure](#structure)  
3. [How It Works](#how-it-works)  
   - [Entry](#entry)  
   - [Restrictions](#restrictions)  
   - [Logical Process](#logical-process)  
4. [Endpoints](#endpoints)  
   - [Request Body](#request-body)  
   - [Response](#response)  
5. [Running the Project](#running-the-project)  
6. [Examples](#examples)  
7. [Notes on Renju Rules](#notes-on-renju-rules)  

---

## Overview

This .NET 8 (or later) project exposes a single API endpoint that receives a 2D board layout for a Gomoku game. It analyzes the board to determine:
- Whether the board is valid (dimensions, counts of black/white stones, etc.).
- Whether the game is already over (detects a winner or a draw).
- If the board is valid and the game is ongoing, it uses **Minimax** to find the best next move for the AI player under **Renju** rules (with forbidden moves for Black).

## Structure

The project is divided into the following main components:

1. **Controllers**  
   - **GameController**: Handles the HTTP POST request for computing the best move.

2. **MiddleWares** (Game Logic and AI)  
   - **GomokuRenju**:  
     - Provides all the logic for Renju-specific checks (e.g., forbidden moves such as overlines, double-threes, double-fours, etc.)  
     - Checks for a winner, counts consecutive stones, and so on.
   - **MinimaxAI**:  
     - Implements the Minimax algorithm with alpha-beta pruning to evaluate potential moves.  
     - Scores the board state to choose the optimal position for the AI player’s next move.

3. **Models**  
   - **MinimaxRequest**: Defines the shape of the incoming request body (`List<List<int>> Board` and an optional `Depth` parameter).

---

## How It Works

### Entry
1. The client (or another application) sends a **POST** request to the `/api/game/minimax-move` endpoint.
2. The request must include a JSON body matching **MinimaxRequest**, which contains:
   - A 2D list (`List<List<int>>`) representing the board:
     - **0** indicates an empty cell.
     - **1** indicates a Black stone.
     - **-1** indicates a White stone.
   - An integer `Depth` (optional in the code) used to potentially set the search depth for Minimax.

3. Based on the counts of Black (`1`) and White (`-1`) stones, the code determines whose turn it is:
   - If there are more Black stones than White stones, it’s White’s turn (`-1`).
   - Otherwise, it’s Black’s turn (`1`).

### Restrictions
1. If the board is null or empty, the API returns a **BadRequest**.
2. If the board dimensions are smaller than 5×5, the API returns a **BadRequest**.
3. If the board contains invalid values (anything other than `0`, `1`, or `-1`), the API returns a **BadRequest**.
4. If the difference between the number of Black and White stones exceeds `1`, the API returns a **BadRequest**.
5. If **Black** has made a forbidden move according to **Renju** rules, the API returns a **BadRequest**.
6. If the code detects multiple winners simultaneously, the API returns a **BadRequest**.

### Logical Process
1. **Check for a winner**  
   - Uses `HasWinner` to see if there is exactly one winner. If a winner is found, returns an **Ok** with `"The game is already over."`.
2. **Check for draw**  
   - If every cell is occupied and no winner is found, returns an **Ok** with `"The game is a draw."`.
3. **Run Minimax**  
   - If the game is valid and ongoing, the code constructs a `MinimaxAI` object with the appropriate player assignments (AI vs. Human).
   - It calls `GetBestMove` to run the Minimax search and find the best cell to place the next stone.
   - Returns the best move along with `"Best move found by Minimax"`.

---

## Endpoints

### `POST /api/game/minimax-move`

**Description**:  
Given a Gomoku board and an optional depth, returns either:
- An error describing why the request is invalid, or  
- The best move for the next player (or a game over/draw response).

#### Request Body

```jsonc
{
  "Board": [
    [ 0,  0,  0,  0,  0],
    [ 0,  1,  0,  0,  0],
    [ 0,  0, -1,  0,  0],
    [ 0,  0,  0,  1,  0],
    [ 0,  0,  0,  0, -1]
  ],
  "Depth": 4 // optional
}
```

- **Board**: A 2D array of integers.  
  - **0** = Empty cell  
  - **1** = Black stone  
  - **-1** = White stone  
- **Depth**: An integer controlling the Minimax depth (defaults to 4 in this example, but the code can be adjusted).

#### Response

**On success** (and the game still in progress):
```jsonc
{
  "Player": "Black",       // or "White"
  "Row": 3,
  "Column": 4,
  "Message": "Best move found by Minimax",
  "Status": "Playing"
}
```

**If the game is already won**:
```jsonc
{
  "Player": "Black", // or "White"
  "Column": -1,
  "Row": -1,
  "Message": "The game is already over.",
  "Status": "Game Over"
}
```

**If the game is a draw**:
```jsonc
{
  "Player": "Draw",
  "Column": -1,
  "Row": -1,
  "Message": "The game is a draw.",
  "Status": "Draw"
}
```

**On invalid board**:
```jsonc
{
  "Message": "Invalid board values. Only 0 (empty), 1 (black), and -1 (white) are allowed.",
  "Status": "Error"
}
```

(Or another relevant message explaining the specific issue.)

---

## Running the Project

Run from Visual Studio

Double-click the .sln file to open it in Visual Studio.
In Solution Explorer, right-click on the project that you want to run (the one containing your Program.cs or Startup.cs) and choose Set as Startup Project.
Press F5 (or click the green "Start" button) to build and run the project.

---

## Examples

### 1. Basic Valid Request

**Request**:
```jsonc
POST /api/game/minimax-move
{
  "Board": [
    [ 0,  0,  0,  0,  0],
    [ 0,  1,  0,  0,  0],
    [ 0,  0, -1,  0,  0],
    [ 0,  0,  0,  1,  0],
    [ 0,  0,  0,  0,  0]
  ],
  "Depth": 4
}
```

**Response** (example):
```jsonc
{
  "Player": "White",
  "Row": 2,
  "Column": 2,
  "Message": "Best move found by Minimax",
  "Status": "Playing"
}
```
(*Note*: The exact row/column result may vary depending on the board configuration.)

### 2. Invalid Board Dimensions

**Request**:
```jsonc
POST /api/game/minimax-move
{
  "Board": [
    [1, 0, 0],
    [0, 1, 0],
    [0, 0, -1]
  ]
}
```
**Response**:
```jsonc
{
  "Message": "Invalid board dimensions. The board must be at least 5x5.",
  "Status": "Error"
}
```

### 3. Forbidden Move Detected (Renju Overline)

If **Black** places a stone and forms an overline (6 or more in a row), the code treats it as forbidden. The server will respond with an **Error** message:
```jsonc
{
  "Message": "Invalid board state. Black made a forbidden move according to Renju rule.",
  "Status": "Error"
}
```

---

## Notes on Renju Rules

1. **Forbidden Moves for Black**:  
   - Overline (more than 5 in a row, e.g., 6, 7, ...).
   - Double-Three (a move that simultaneously creates two (or more) open three patterns).
   - Double-Four (a move that simultaneously creates two (or more) four-in-a-row patterns).

2. **White** has no forbidden moves.  

3. When **Black** violates any forbidden move condition, the game state is considered invalid in this API’s context, and a **BadRequest** is returned.

---

_This project demonstrates a straightforward integration of Renju-specific rules with a Minimax-based AI. For more customization, adjust the `MinimaxAI` parameters (like `maxDepth`) or modify the evaluation function to account for more complex heuristics._

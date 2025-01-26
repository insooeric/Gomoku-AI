# Gomoku-AI 
(Hey, I need to modify this)

A Gomoku AI that uses various AI algorithm that evaluates the game under rules and give optimized result.

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
6. [Notes on Rules](#notes-on-rules)

---

## Overview
This project is build in .NET 8, with two API endpoints that receives 2D Board, Depth/Iteration, and Rule. 
The project implements a Gomoku (also known as Five in a Row) game with following AI algorithms for move evaluation and selection:
1. **Monte Carlo Tree Search** and **Priority**
2. **Minimax**, **Alpha-beta puring**, and **Heuristic Scoring**

It also utilizes at least one of the following rules:
1. **Free Style Rule** [https://en.wikipedia.org/wiki/Gomoku/#Freestyle gomoku]
2. **Renju Rule** [https://en.wikipedia.org/wiki/Gomoku/#Renju]

## Structure
Followings are the core components:

1. **AIModels**  
   - **HMA-B/**: Includes logics and behaviours of Minimax Algorithm along with Alpha-beta puring with Heuristic scoring.
   - **MCTS/**: Includes logics and behaviours of Monte carlo tree search based on prioritized moves.

2. **RuleModels/** (Game Logic and AI)  
   - Provides all the logic for Free Style Rule as well as Renju Rule (e.g., forbidden moves such as overlines, double threes, double fours, etc.)
   - Checks for a winner, counts consecutive stones, and so on.

3. **Models**  
   - Defines the shape of the incoming request body (`List<List<int>> Board` for overall board, `Depth` for Minimax search depth and number of Monte Carlo Tree Search iteration).

---

## How It Works

### Entry
1. The client (or another application) sends a **POST** request to either:
   - `/api/gomoku/minimax-move` endpoint for Minimax.
   - `/api/gomoku/minimax-move` endpoint for Monte Carlo Tree Search.
2. The request must include a JSON body matching **InputModel**, which contains:
   - A 2D list (`List<List<int>>`) representing the board:
     - **0** indicates an empty cell.
     - **1** indicates a Black stone.
     - **-1** indicates a White stone.
   - An integer `Depth` used to potentially set:
        - The search depth for Minimax.
        - The iteration for Monte Carlo Tree Search

3. Based on the counts of Black (`1`) and White (`-1`) stones from the board, it automatically determines whose turn it is:
   - For any invalid or complete game, it returns ( `x: -1, y: -1`) with message and other params indicating it.
   - If there are more Black stones than White stones, it’s White’s turn (`-1`).
   - Otherwise, it’s Black’s turn (`1`).

### Restrictions
The endpoint will return **BadRequest** under following conditions:
1. If the board is null or empty
2. If the board dimensions are smaller than 7×7
3. If the board isn't rectangular
4. If the board contains invalid values (anything other than `0`, `1`, or `-1`)
5. If the difference between the number of Black and White stones exceeds `1`
6. If **Black** has made a forbidden move according to **Renju** rules
7. If the code detects multiple winners simultaneously

### Logical Process
1. **Check for a winner**  
   - Uses `HasWinner` to see if there is exactly one winner. If a winner is found, returns an **Ok** with winner's number (`1` for Black, `-1` for White) with  `"Black Wins 🎉"` or `"White Wins 🎉"`.
2. **Check for draw**  
   - If every cell is occupied and no winner is found, returns an **Ok** with `"It's a Draw 🤝"`.
3. **Run AI**  
   - If the game is valid and ongoing, based on the API endpoint, the code runs either **Minimax** or **Monte Carlo Tree Search** logic with the appropriate player assignments (AI vs Human).
   - Then it performs followings:
        - Minimax: It runs `GetBestMove` to run Minimax move
        - Monte Carlo Tree Search: It runs `Search` to run MCTS move
   - Both returns `Move` for the best cell to place the next stone.

---

## Endpoints

### `POST /api/game/minimax-move`
### `POST /api/game/mcts-move`

**Description**:  
For both endpoints, given a Board, Depth, and RuleType, returns either:
- An error describing why the request is invalid, or  
- The best move for the next player (or a game over/draw response).

#### Request Body

```jsonc
{
    "Board": [
        [0, 0, 0, 0, 0, 0, 0, 0],
        [0, 0, 0, 0, 0, 0, 0, 0],
        [0, 0, 0, 1, -1, 0, 0, 0],
        [0, 0, 0, 1, -1, 0, 0, 0],
        [0, 0, 0, 1, -1, 0, 0, 0],
        [0, 0, 0, 0, 0, 0, 0, 0],
        [0, 0, 0, 0, 0, 0, 0, 0],
        [0, 0, 0, 0, 0, 0, 0, 0]
    ],
    "Depth": 3,
    "RuleType": "freestyle" // or "renju"
}
```

- **Board**: A 2D array of integers.  
  - **0** = Empty cell  
  - **1** = Black stone  
  - **-1** = White stone  
- **Depth**: An integer controlling either the Minimax depth or a number of iterations for MCTS

#### Response

**On success** (and the game still in progress):
```jsonc
{
   Status: "Playing",
   X: 5,
   Y: 3,
   Color: "Black", // or "White"
   Message: "Playing"
}
```

**If the game is already won**:
```jsonc
{
   Status: "Win",
   X: 2,
   Y: 6,
   Color: "White", // always AI's color
   Message: "White Wins 🎉"
}
```

**If the game is a draw**:
```jsonc
{
   Status:"'Draw",
   X: -1,
   Y: -1,
   Color: "White", // always AI's color
   Message: "It's a Draw 🤝"
}
```

**On invalid board**:
```jsonc
{
   Status: "Error",
   X: -1,
   Y: -1,
   Color: "White", // always AI's color
   Message: "Invalid board values. Only 0 (empty), 1 (black), and -1 (white) are allowed."
}
```

(Or another relevant message explaining the specific issue.)

---

## Running the Project

Run from Visual Studio

Double-click the .sln file to open it in Visual Studio.
In Solution Explorer, right-click on the project that you want to run (the one containing your Program.cs) and choose Set as Startup Project.
Press F5 (or click the green "Start Without Debugging" button) to build and run the project.

---

## Notes on Rules

### Free Style Rule
- Black plays first, then white plays. Both players take turn until either one of the player connects more than 5 lines or the board is full, which is draw.
- There is no restriction for both Black and White players.

### Renju Rule
1. **Forbidden Moves for Black**:  
   - Overline (more than 5 in a row, e.g., 6, 7, ...).
   - Double-Three (a move that simultaneously creates two (or more) open three patterns).
   - Double-Four (a move that simultaneously creates two (or more) four-in-a-row patterns).

2. **White** has no forbidden moves.  

3. When **Black** violates any forbidden move condition, the game state is considered invalid in this API’s context, and a **BadRequest** is returned.

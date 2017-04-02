# MedUCate

A trivia/learning game created for a client as the primary project of COMP 585H at UNC-Chapel Hill.

# Table of Contents
  * [TODO](#TODO)
  * [Requirements](#Requirements)
  * [Installation](#Installation)

<a name = "TODO"></a>
## TODO
  * Networking
    * Manage leaderboard on server
    * Handle Question Sets on server (including IDs for adding/updating them in game)
    * Handle activationCode
    * Handle expiryTime
    * Handle multiplayer
    * Handle usernames
    * Verify user information with server on startup
  * GameLogicManager.cs
    * Remove hardcoded Question Sets
  * UIManager.cs
    * Implement "ADD QUESTIONS"
    * Implement "VIEW HISTORY"
    * Implement "HOST GAME" (get game code for other player to join game)
    * Implement "JOIN GAME"
    * (Nonessential) XML parser for menus

<a name = "Requirements"></a>
## Requirements
Should work natively on Unity versions >= 5.5.1f1, but will be compatible with most recent versions of Unity if packaged.

<a name = "Installation"></a>
## Installation
Simply importing all the assets into a new Unity project and opening the Main scene should suffice.
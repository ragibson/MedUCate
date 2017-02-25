# MedUCate

A trivia/learning game created for a client as the primary project of COMP 585H at UNC-Chapel Hill.

# Table of Contents
  * [TODO](#TODO)
  * [Requirements](#Requirements)
  * [Installation](#Installation)

<a name = "TODO"/>
## TODO
  * General
    * XML parser for QuestionSet <-> XML
    * Networking for Multiplayer
    * Handle player reputation
  * GameLogicManager.cs
    * Remove hardcoded Question Sets
    * Implement Campaign Level Completion Triggers
  * QuestionSet.cs
    * Handle activationCode
    * Handle expiryTime
  * TriviaRound.cs
    * If neither player answers correctly, skip combat
  * UIManager.cs
    * Implement "ADD QUESTIONS"
    * Implement "VIEW HISTORY"
    * Implement "HOST GAME"
    * Implement "JOIN GAME"
    * Implement "LEADERBOARD"
    * (Nonessential) XML parser for menus
  
<a name = "Requirements"/>
## Requirements
Should work natively on Unity versions >= 5.5.1f1, but will be compatible with most recent versions of Unity if packaged.
  
<a name = "Installation"/>
## Installation
Simply importing all the assets into a new Unity project and opening the Main scene should suffice.
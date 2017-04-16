﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class UIManager : MonoBehaviour
{

	/*
	 * 	TODO: The following menu items were not
	 * 	implemented in the original prototype
	 * 	and have not been implemented here:
	 * 		VIEW HISTORY
	 * 		HOST GAME
	 * 		JOIN GAME
	 * 
	 * 	TODO: Add server-based multiplayer
	 * 
	 * 	TODO: LEADERBOARD needs to be connected
	 * 	to the server once networking  for the
	 * 	game is more complete.
	 */

	private Settings settings;
	private ComputerPlayer computer;
	private GameLogicManager gameLogic;

	public GameObject primaryDisplay;
	public Slider slider;
	public Button[] buttons;
	public Sprite[] images;
	private Action[] buttonBehaviors;

	// objects = [sword, shield, star]
	public GameObject[] objects;

	public Action currentMenu;

	/*
	 *	Runs once on first frame of game
	 *	Used to display main menu, set up
	 *	settings, and set up computer player
	 */
	void Start ()
	{
		objectVisibility (false, false, false);
		settings = GameObject.Find ("GameLogicManager").GetComponent<GameLogicManager> ().settings;
		computer = GameObject.Find ("GameLogicManager").GetComponent<GameLogicManager> ().computer;
		gameLogic = GameObject.Find ("GameLogicManager").GetComponent<GameLogicManager> ();
		slider = GameObject.Find ("Timer Slider").GetComponent<Slider> ();
		currentMenu = connectingToServer;
	}
	
	// Update is called once per frame
	void Update ()
	{
		currentMenu ();
	}

	/*
	 * 	Sets the visibility of the sword, shield,
	 * 	and multiplier blocks
	 */
	void objectVisibility (bool sword, bool shield, bool star)
	{
		objects [0].SetActive (sword);
		objects [1].SetActive (shield);
		objects [2].SetActive (star);
	}

	/*
	 * 	This method receives the number of the button just pressed
	 * 	and then runs the method associated with the button
	 */
	public void buttonPressed (int i)
	{
		buttonBehaviors [i] ();
	}

	void setDisplayImage (Sprite image)
	{
		primaryDisplay.GetComponent<Image> ().sprite = image;
	}

	void setDisplayColor (Color color)
	{
		primaryDisplay.GetComponent<Image> ().color = color;
	}

	void setDisplayText (String text)
	{
		primaryDisplay.GetComponentInChildren<Text> ().text = text;
	}

	/*
	 * 	Sets the button's texts to match the passed string[]
	 * 
	 * 	If button text is empty, we make that button uniteractable
	 * 
	 * 	params:
	 * 		buttonText: string[] of [Button1Text, ...]
	 */
	void setButtonsText (string[] buttonText)
	{
		for (int i = 0; i < 4; i++) {
			buttons [i].interactable = !String.Equals (buttonText [i], "");
		}

		for (int i = 0; i < buttons.Length; i++) {
			buttons [i].GetComponentInChildren<Text> ().text = buttonText [i];
		}
	}

	/*
	 * 	Sets the button's behaviors to match the passed Action[]
	 * 
	 * 	params:
	 * 		buttonText: Action[] of [ButtonMethod1, ...]
	 */
	void setButtonBehaviors (Action[] behaviors)
	{
		buttonBehaviors = behaviors;
	}

	// ===== ALL METHODS PAST THIS POINT ARE MENU HANDLERS ===== //
	// ===== IT'S JUST THE TEXT THAT IS DISPLAYED IN MENUS ===== //

	void mainMenu ()
	{
		setButtonsText (new string[] { "SINGLEPLAYER >>>", 
			"MULTIPLAYER >>>",
			"PROFILE >>>",
			"Exit"
		});
		setButtonBehaviors (new Action[] { singlePlayer, multiPlayer, profile, Application.Quit });

		setDisplayImage (images [0]);
		setDisplayColor (Color.white);
		setDisplayText ("");

		currentMenu = mainMenu;
	}

	void singlePlayer ()
	{
		setButtonsText (new string[] { "QUICK PLAY >>>", 
			"TUTORIAL >>>", 
			"CAMPAIGN >>>", 
			"<<< BACK TO MENU"
		});
		setButtonBehaviors (new Action[] { singlePlayerQuickPlay, tutorialStart, singlePlayerCampaign, mainMenu });

		setDisplayImage (images [1]);
		setDisplayColor (Color.blue);
		setDisplayText ("MAIN MENU > SINGLEPLAYER\n\n" +
		"QUICK PLAY -\n" +
		"PLAY YOUR SELECTED QUESTIONS\n" +
		"AGAINST A COMPUTER OPPONENT\n\n" +
		"TUTORIAL -\n" +
		"LEARN HOW TO PLAY\n\n" +
		"CAMPAIGN -\n" +
		"IF YOU CAN COMPLETE ALL THE\n" +
		"LEVELS, THEN YOU HAVE\n" +
		"COMPLETED YOUR LEARNING");

		currentMenu = singlePlayer;
	}

	void multiPlayer ()
	{
		setButtonsText (new string[] { "QUICK PLAY >>>",
			"BATTLE CODE >>>", 
			"ONE MAN ARMY >>>",
			"<<< BACK TO MENU"
		});
		setButtonBehaviors (new Action[] { multiPlayerQuickPlay, multiPlayerBattleCode, multiPlayerOneManArmy, mainMenu });

		setDisplayImage (images [2]);
		setDisplayColor (Color.red);
		setDisplayText ("MAIN MENU > MULTIPLAYER\n\n" +
		"QUICK PLAY -\n" +
		"PLAY QUESTIONS AGAINST A\n" +
		"RANDOM HUMAN OPPONENT\n\n" +
		"BATTLE CODE -\n" +
		"CREATE AND SHARE CODES TO\n" +
		"SELECT QUESTIONS AND WHO YOU\n" +
		"PLAY AGAINST\n\n" +
		"ONE MAN ARMY -\n" +
		"RISK REPUTATION TO EARN A SPOT\n" +
		"ON THE LEADERBOARDS FOR THE\n" +
		"SPONSORED QUESTIONS");

		currentMenu = multiPlayer;
	}

	void profile ()
	{
		setButtonsText (new string[] {
			"SELECT QUESTION SETS >>>",
			"ADD/REMOVE QUESTION SETS>>>",
			"REVIEW QUESTIONS >>>",
			"<<< BACK TO MENU"
		});
		setButtonBehaviors (new Action[] { changeQuestions, addQuestions, reviewQuestions, mainMenu });

		setDisplayImage (images [3]);
		setDisplayColor (Color.blue);
		setDisplayText ("MAIN MENU > PROFILE\n\n" +
		"CHANGE QUESTIONS -\n" +
		"CHANGE YOUR CURRENT\n" +
		"QUESTION SET\n\n" +
		"ADD QUESTIONS -\n" +
		"IMPORT YOUR OWN QUESTIONS\n" +
		"FROM A CODE TO CUSTOMIZE\n" +
		"YOUR GAMES\n\n" +
		"REVIEW QUESTIONS -\n" +
		"SHOW ALL THE QUESTIONS FOR THE\n" +
		"CURRENT QUESTION SET");

		// Make sure the input field for adding questions not being shown
		gameLogic.inputfield.SetActive (false);

		currentMenu = profile;
	}

	void addQuestions ()
	{
		setButtonsText (new string[] {
			"Add Set To Device",
			"Remove Set From Device",
			"",
			"<<< BACK TO PROFILE"
		});
		setButtonBehaviors (new Action[] { addThisSet, removeThisSet, noMenu, profile });

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("Go to\n" +
		"www.meducate.cs.unc.edu\n" +
		"to update and add question sets!\n\n" +
		"Then, come back and type the name\n" +
		"of the Question Set here!");

		gameLogic.inputfield.SetActive (true);

		currentMenu = addQuestions;
	}

	void addThisSet ()
	{
		string setName = gameLogic.inputfield.GetComponentInChildren<InputField> ().text.Replace (' ', '_');
		gameLogic.setsToAdd.Enqueue (setName);
		gameLogic.inputfield.GetComponentInChildren<InputField> ().text = "Requested Set From Server!";
		currentMenu = addQuestions;
	}

	void removeThisSet ()
	{
		string setName = gameLogic.inputfield.GetComponentInChildren<InputField> ().text;
		for (int i = 0; i < gameLogic.questionSets.Count; i++) {
			if (String.Equals (gameLogic.questionSets [i].setName.ToLower (), setName.ToLower ())) {
				gameLogic.questionSets.RemoveAt (i);
			}
		}
		gameLogic.setCurrentSet (0);
		gameLogic.inputfield.GetComponentInChildren<InputField> ().text = "Removed Set From Device!";
		currentMenu = addQuestions;
	}

	void singlePlayerQuickPlay ()
	{
		setButtonsText (new string[] { "<<< START GAME >>>",
			"CHANGE COMPUTER DIFFICULTY >>>", 
			"CHANGE COMPUTER SPEED >>>",
			"<<< BACK TO SINGLEPLAYER"
		});
		setButtonBehaviors (new Action[] {
			singlePlayerQuickPlayStartGame,
			computer.changeDifficulty,
			computer.changeSpeed,
			singlePlayer
		});

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("MAIN MENU > SINGLEPLAYER > QUICK PLAY\n\n" +
		"CURRENT QUESTIONS -\n" +
		settings.selected.setName +
		"\n(GO TO PROFILE TO CHANGE QUESTIONS)\n\n" +
		"CURRENT DIFFICULTY -\n" +
		computer.getDifficultyString () +
		String.Format ("\n(OPTION {0} OF {1})\n\n", computer.difficulty + 1, computer.difficulties.Length) +
		"CURRENT OPPONENT SPEED -\n" +
		computer.getSpeedString () +
		String.Format ("\n(OPTION {0} OF {1})", computer.speed + 1, computer.speeds.Length));

		currentMenu = singlePlayerQuickPlay;
	}

	void singlePlayerQuickPlayStartGame ()
	{
		gameLogic.gameMode = "Single Player Quick Play";
		currentMenu = startGame;
	}

	void tutorialStart ()
	{
		objectVisibility (false, false, false);
		setButtonsText (new string[] { "THESE BUTTONS WILL HAVE HELPFUL TEXT",
			"OR DESCRIBE THE BUTTON'S ACTION", 
			"PROCEED >>>",
			"<<< BACK TO SINGLEPLAYER"
		});
		setButtonBehaviors (new Action[] {
			noMenu,
			noMenu,
			tutorialTrivia,
			singlePlayer
		});

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("WELCOME TO THE TUTORIAL\n\n" +
		"HERE, WE'LL LEARN ABOUT BASIC GAMEPLAY,\n" +
		"MAINLY THE TRIVIA AND COMBAT PHASES\n\n" +
		"LET'S START WITH AN EXAMPLE TRIVIA ROUND:\n" +
		"CLICK THE BUTTON BELOW TO PROCEED");

		currentMenu = tutorialStart;
	}

	void tutorialTrivia ()
	{
		objectVisibility (false, false, false);
		setButtonsText (new string[] { "ANSWERS WILL BE LISTED HERE",
			"ONLY ONE WILL BE CORRECT", 
			"PROCEED >>>",
			"<<< PREVIOUS MENU"
		});
		setButtonBehaviors (new Action[] {
			noMenu,
			noMenu,
			tutorialTrivia1,
			tutorialStart
		});

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("ROUND 1\n\n" +
		"1+1?");

		currentMenu = tutorialTrivia;
	}

	void tutorialTrivia1 ()
	{
		objectVisibility (false, false, false);
		setButtonsText (new string[] { "",
			"", 
			"PROCEED >>>",
			"<<< PREVIOUS MENU"
		});
		setButtonBehaviors (new Action[] {
			noMenu,
			noMenu,
			tutorialCombat,
			tutorialTrivia
		});

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("YOU WILL HAVE 10 SECONDS TO ANSWER EACH QUESTION\n\n" +
		"ANSWER CORRECTLY BEFORE YOUR OPPONENT DOES AND\n" +
		"YOU'LL BE ALLOWED TO ``ATTACK'' IN THE COMBAT PHASE\n\n" +
		"ANSWER INCORRECTLY OR TOO SLOWLY AND\n" +
		"YOU'LL HAVE TO ``DEFEND''\n\n" +
		"IF NEITHER PLAYER ANSWERS CORRECTLY,\n" +
		"COMBAT WILL BE SKIPPED ALTOGETHER\n\n" +
		"NOW, LET'S DISCUSS THE COMBAT PHASE!");

		currentMenu = tutorialTrivia1;
	}

	void tutorialCombat ()
	{
		objectVisibility (false, false, false);
		setButtonsText (new string[] { "",
			"", 
			"PROCEED >>>",
			"<<< PREVIOUS MENU"
		});
		setButtonBehaviors (new Action[] {
			noMenu,
			noMenu,
			tutorialCombat1,
			tutorialTrivia1
		});

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("AS THE ATTACKING PLAYER,\n" +
		"YOU'LL PLACE A SWORD SQUARE\n\n" +
		"THIS SQUARE WILL DEAL DAMAGE\n" +
		"THROUGHOUT ITS ENTIRE AREA\n\n" +
		"IT WILL DEAL *DOUBLE DAMAGE* WHERE\n" +
		"IT OVERLAPS WITH THE STAR BLOCK\n\n" +
		"LET'S SEE HOW THIS WORKS IN PRACTICE:\n\n" +
		"NOW, WE'LL SHOW HOW MUCH DAMAGE YOU'LL DO,\n" +
		"BUT KEEP IN MIND THAT YOU WON'T BE ABLE\n" +
		"TO SEE THIS IN A REAL GAME!");

		currentMenu = tutorialCombat;
	}

	// Setup for next tutorial menu
	void tutorialCombat1 ()
	{	
		game = new Game (gameLogic.gameHP, gameLogic.secondsPerRound, gameLogic.damagePerAttack);

		RectTransform bounds = primaryDisplay.GetComponent<RectTransform> ();
		gameLogic.randomlyPlaceStar (bounds, objects);

		objectVisibility (true, false, true);

		game.nextRound ();
		game.currentTriviaRound.correct = true;
		game.nextCombat ();

		currentMenu = tutorialCombat2;
	}

	void tutorialCombat2 ()
	{
		setDisplayColor (Color.clear);
		setDisplayText ("");

		game.currentCombatRound.damageCalc (objects [0], objects [1], objects [2], out damageDealt, out damageBlocked);

		setButtonsText (new string[] { "DAMAGE: " + damageDealt,
			"TRY DRAGGING THE SWORD AROUND!", 
			"PROCEED >>>",
			"<<< PREVIOUS MENU"
		});
		setButtonBehaviors (new Action[] {
			noMenu,
			noMenu,
			tutorialCombat3,
			tutorialCombat
		});

		currentMenu = tutorialCombat2;
	}

	void tutorialCombat3 ()
	{
		objectVisibility (false, false, false);
		setButtonsText (new string[] { "",
			"", 
			"PROCEED >>>",
			"<<< PREVIOUS MENU"
		});
		setButtonBehaviors (new Action[] {
			noMenu,
			noMenu,
			tutorialCombat4,
			tutorialCombat1
		});

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("AS THE DEFENDING PLAYER,\n" +
		"YOU'LL PLACE A SHIELD SQUARE\n\n" +
		"THIS SQUARE WILL BLOCK ALL DAMAGE\n" +
		"THROUGHOUT ITS ENTIRE AREA\n\n" +
		"LET'S SEE HOW THIS WORKS IN PRACTICE:\n\n" +
		"NOW, WE'LL SHOW HOW MUCH DAMAGE YOU'LL BLOCK,\n" +
		"AND THE POSITION OF THE SWORD BLOCK\n" +
		"BUT KEEP IN MIND THAT YOU WON'T BE ABLE\n" +
		"TO SEE THESE IN A REAL GAME!");

		currentMenu = tutorialCombat3;
	}

	// Setup for next tutorial menu
	void tutorialCombat4 ()
	{
		game = new Game (gameLogic.gameHP, gameLogic.secondsPerRound, gameLogic.damagePerAttack);

		RectTransform bounds = primaryDisplay.GetComponent<RectTransform> ();
		gameLogic.randomlyPlaceStar (bounds, objects);

		objectVisibility (true, true, true);

		game.nextRound ();
		game.currentTriviaRound.correct = false;
		computer.placeBlockTutorial (bounds, objects);
		game.nextCombat ();

		currentMenu = tutorialCombat5;
	}

	void tutorialCombat5 ()
	{
		setDisplayColor (Color.clear);
		setDisplayText ("");

		game.currentCombatRound.damageCalc (objects [0], objects [1], objects [2], out damageDealt, out damageBlocked);

		setButtonsText (new string[] { string.Format ("DAMAGE BLOCKED: {0} OUT OF {1}", damageBlocked, damageDealt),
			"TRY DRAGGING THE SHIELD AROUND!", 
			"PROCEED >>>",
			"<<< PREVIOUS MENU"
		});
		setButtonBehaviors (new Action[] {
			noMenu,
			noMenu,
			endTutorial,
			tutorialCombat3
		});

		currentMenu = tutorialCombat5;
	}

	void endTutorial ()
	{
		objectVisibility (false, false, false);
		gameLogic.gameMode = "Tutorial";
		gameLogic.displayText = gameLogic.changeReputation ();
		currentMenu = finalTutorialScreen;
	}

	void finalTutorialScreen ()
	{
		setButtonsText (new string[] { "",
			"", 
			"TO MAIN MENU>>>",
			"<<< PREVIOUS MENU"
		});
		setButtonBehaviors (new Action[] {
			noMenu,
			noMenu,
			mainMenu,
			tutorialCombat4
		});

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("THAT'S ALL YOU NEED TO KNOW TO PLAY!\n\n" +
		"TO START OUT, TRY SINGLEPLAYER QUICK PLAY\n" +
		"OR THE SINGLEPLAYER CAMPAIGN\n\n" +
		"IF YOU'RE CONFIDENT IN YOUR SKILLS\n" +
		"TRY JUMPING STRAIGHT INTO MULTIPLAYER\n\n" +
		"HAVE FUN!\n\n" +
		gameLogic.displayText);

		currentMenu = finalTutorialScreen;
	}

	void singlePlayerCampaign ()
	{

		string startGameText = "<<< START GAME >>>";
		Action startGameAction = startCampaign;
		if (computer.level > 0 && gameLogic.campaignScores [computer.level - 1] == 0) {
			startGameText = "NOT UNLOCKED";
			startGameAction = noMenu;
		}

		setButtonsText (new string[] { startGameText,
			"CHANGE LEVEL >>>", 
			"<<< CHANGE LEVEL",
			"<<< BACK TO SINGLEPLAYER"
		});
		setButtonBehaviors (new Action[] {
			startGameAction,
			computer.increaseLevel,
			computer.decreaseLevel,
			singlePlayer
		});

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("MAIN MENU > SINGLEPLAYER > CAMPAIGN\n\n" +
		"CURRENT QUESTIONS -\n" +
		settings.selected.setName +
		"\n(GO TO PROFILE TO CHANGE QUESTIONS)\n\n" +
		"CURRENT LEVEL -\n" +
		computer.getLevelString () +
		String.Format ("\n(OPTION {0} OF {1})\n\n", computer.level + 1, computer.numberOfLevels ()) +
		"LEVEL STATUS -\n" +
		gameLogic.getCampaignScore (computer.level) +
		"\n(THERE ARE 9 LEVELS TOTAL)");

		currentMenu = singlePlayerCampaign;
	}

	void startCampaign ()
	{
		gameLogic.gameMode = "Campaign";
		currentMenu = startGame;
	}

	void multiPlayerQuickPlay ()
	{
		setButtonsText (new string[] { "<<< START GAME >>>",
			"<<< VIEW HISTORY >>>", 
			"CHANGE TYPE >>>",
			"<<< BACK TO MULTIPLAYER"
		});
		setButtonBehaviors (new Action[] {
			multiPlayerQuickPlayStartGame,
			noMenu,
			settings.changeMultiPlayerType,
			multiPlayer
		});

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("MAIN MENU > MULTIPLAYER > QUICK PLAY\n\n" +
		"CURRENT QUESTIONS -\n" +
		settings.selected.setName +
		"\n(GO TO PROFILE TO CHANGE QUESTIONS)\n\n" +
		"SPONSORED QUESTIONS -\n" +
		"DEFAULT NUTRITIONAL HEALTH SET\n" +
		"(THESE CHANGE EVERY DAY)\n\n" +
		"CURRENT MODE -\n" +
		settings.getMultiPlayerModeString () +
		String.Format ("\n(OPTION {0} OF {1})\n\n", settings.multiplayerMode + 1, settings.multiplayerModes.Length));

		currentMenu = multiPlayerQuickPlay;
	}

	float currentMultiplayerWaitTime;
	float timeToWaitUntilRandomAI;

	void multiPlayerQuickPlayStartGame ()
	{
		currentMultiplayerWaitTime = 0;
		// If we wait for 30-60 seconds with no opponent, we'll play with a random AI
		timeToWaitUntilRandomAI = 30 + UnityEngine.Random.Range (0, 30);
		currentMenu = multiPlayerQuickPlayWaitForGame;
	}

	// TODO: Add server-based multiplayer here
	void multiPlayerQuickPlayWaitForGame ()
	{
		setButtonsText (new string[] { "",
			"", 
			"",
			"<<< BACK TO QUICK PLAY"
		});
		setButtonBehaviors (new Action[] {
			noMenu,
			noMenu,
			noMenu,
			multiPlayerQuickPlay
		});

		currentMultiplayerWaitTime += Time.deltaTime;

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("Waiting for another player...\n\n" +
		String.Format ("Time in Queue: {0} seconds", Mathf.Round (currentMultiplayerWaitTime)));

		if (currentMultiplayerWaitTime > timeToWaitUntilRandomAI) {
			slider.value = 5;
			currentMenu = proceedToAIGame;
		} else {
			currentMenu = multiPlayerQuickPlayWaitForGame;
		}
	}

	void proceedToAIGame ()
	{
		slider.value -= Time.deltaTime;
		slider.GetComponentInChildren<Text> ().text = "" + (int)slider.value;

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("Found an Opponent!\n\n" +
		"Setting up game...");

		if (slider.value <= 0) {
			gameLogic.computer.level = UnityEngine.Random.Range (0, 9);
			gameLogic.computer.updateSpeedAndDifficulty ();
			gameLogic.gameMode = "Multiplayer Quick Play";
			Debug.Log (gameLogic.computer.getLevelString ());
			currentMenu = startGame;
		} else {
			currentMenu = proceedToAIGame;
		}
	}

	void multiPlayerBattleCode ()
	{
		setButtonsText (new string[] { "<<< JOIN GAME >>>",
			"<<< VIEW HISTORY >>>", 
			"<<< HOST GAME >>>",
			"<<< BACK TO MULTIPLAYER"
		});
		setButtonBehaviors (new Action[] {
			startGame,
			noMenu,
			startGame,
			multiPlayer
		});

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("MAIN MENU > MULTIPLAYER > BATTLE CODES\n\n" +
		"CURRENT QUESTIONS -\n" +
		settings.selected.setName +
		"\n(GO TO PROFILE TO CHANGE QUESTIONS)\n\n" +
		"THE HOST USES THEIR QUESTION SET. THE PLAYER\n" +
		"THAT JOINS WILL USE THE QUESTIONS OF THE HOST.\n" +
		"WHEN YOU HOST A GAME, YOU WILL RECEIVE A CODE\n" +
		"TO SHARE WITH THE PERSON YOU WANT TO PLAY WITH");

		currentMenu = multiPlayerBattleCode;
	}

	void multiPlayerOneManArmy ()
	{
		string startGameText = "<<< START GAME >>>";
		if (gameLogic.reputation < settings.getWager ()) {
			startGameText = "NOT ENOUGH REPUTATION TO WAGER";
		}

		setButtonsText (new string[] { startGameText,
			"<<< SHOW LEADERBOARD >>>", 
			"CHANGE WAGER >>>",
			"<<< BACK TO MULTIPLAYER"
		});
		setButtonBehaviors (new Action[] {
			oneManArmyStartGame,
			leaderboardLoad,
			settings.changeWager,
			multiPlayer
		});

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("MAIN MENU > MULTIPLAYER > ONE MAN ARMY\n\n" +
		"SPONSORED QUESTIONS -\n" +
		"DEFAULT NUTRITIONAL HEALTH SET\n" +
		"(THESE CHANGE EVERY DAY)\n\n" +
		"CURRENT REPUTATION -\n" +
		String.Format ("{0}\n", gameLogic.reputation) +
		"(THIS CHANGES AS YOU PLAY MORE GAMES)\n\n" +
		"CURRENT WAGER -\n" +
		settings.getWagerString () +
		String.Format ("\n(OPTION {0} OF {1})", settings.wagerIndex + 1, settings.wagers.Length));

		currentMenu = multiPlayerOneManArmy;
	}

	void oneManArmyStartGame ()
	{
		if (gameLogic.reputation >= settings.getWager ()) {
			
			// Computer is Hard, Moderate Speed for One Man Army
			computer.level = 7;
			gameLogic.computer.updateSpeedAndDifficulty ();

			gameLogic.gameMode = "One Man Army";
			currentMenu = startGame;
		}
	}

	string leaderboardURL = "http://meducate.cs.unc.edu/leaderboard.txt";
	WWW leaderboardWWW;

	void leaderboardLoad ()
	{
		leaderboardWWW = new WWW (leaderboardURL);
		currentMenu = leaderboard;
	}

	void leaderboard ()
	{
		setButtonsText (new string[] { "",
			"", 
			"",
			"<<< BACK TO ONE MAN ARMY"
		});
		setButtonBehaviors (new Action[] {
			noMenu,
			noMenu,
			noMenu,
			multiPlayerOneManArmy
		});

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);

		if (leaderboardWWW.isDone) {
			if (string.Equals (leaderboardWWW.text, "")) {
				setDisplayText ("Error with internet connection");
			} else if (leaderboardWWW.text.Contains ("was not found on this server.")) {
				setDisplayText ("Could not find leaderboard");
			} else {
				setDisplayText (leaderboardWWW.text);
			}
		} else {
			setDisplayText ("Loading...");
		}

		currentMenu = leaderboard;
	}

	void changeQuestions ()
	{
		setButtonsText (new string[] { "<<< SELECT QUESTION SET >>>",
			"CHANGE QUESTION SET >>>", 
			"<<< CHANGE QUESTION SET",
			"<<< BACK TO PROFILE"
		});
		setButtonBehaviors (new Action[] {
			gameLogic.selectQuestionSet,
			gameLogic.increaseSetToChange,
			gameLogic.decreaseSetToChange,
			profile
		});

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("MAIN MENU > PROFILE > CHANGE QUESTIONS\n\n" +
		"CURRENTLY SELECTED SET NAME -\n" +
		settings.selected.setName +
		String.Format ("\n\nSELECTED? - {0}\n", gameLogic.getSetToChange ().selected) +
		String.Format ("NAME - {0}\n", gameLogic.getSetToChange ().setName) +
		String.Format ("AUTHORS - {0}\n", gameLogic.getSetToChange ().authorName) +
		String.Format ("NUMBER OF QUESTIONS - {0}\n", gameLogic.getSetToChange ().numberOfQuestions ()) +
		String.Format ("(SET {0} OF {1})", gameLogic.setToChange + 1, gameLogic.questionSets.Count));

		currentMenu = changeQuestions;
	}

	void reviewQuestions ()
	{
		setButtonsText (new string[] { "<<< HIDE ANSWER >>>",
			"CHANGE QUESTION >>>", 
			"<<< CHANGE QUESTION",
			"<<< BACK TO PROFILE"
		});
		setButtonBehaviors (new Action[] {
			gameLogic.hideAnswer,
			gameLogic.increaseCurrentQuestion,
			gameLogic.decreaseCurrentQuestion,
			profile
		});

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);

		string text = "MAIN MENU > PROFILE > REVIEW\n\n" +
		              "CURRENTLY SELECTED SET NAME -\n" +
		              settings.selected.setName +
		              "\n\nSELECTED -\n" +
		              gameLogic.getCurrentQuestion ().questionText +
		              "\n\n";
		if (!gameLogic.hideAnswers) {
			text += "CORRECT ANSWER -\n" +
			gameLogic.getCurrentQuestion ().correctAnswer;
		}
		text += String.Format ("\n\n(QUESTION {0} OF {1})", gameLogic.currentQuestion + 1,
			gameLogic.getSetToChange ().numberOfQuestions ());

		setDisplayText (text);

		currentMenu = reviewQuestions;
	}

	void connectingToServer ()
	{
		if (gameLogic.setsToAdd.Count != 0) {
			setButtonsText (new string[] { "", "", "", "" });
			setButtonBehaviors (new Action[] { noMenu, noMenu, noMenu, noMenu });
			setDisplayImage (images [5]);
			setDisplayColor (Color.blue);
			setDisplayText ("Connecting to meducate.cs.unc.edu...\n\n" +
			String.Format ("Retrieving {0}", gameLogic.setsToAdd.Peek ()));
			if (gameLogic.completedTutorial) {
				currentMenu = connectingToServer;
			} else {
				currentMenu = tutorialStart;
			}
		} else {
			currentMenu = mainMenu;
		}
	}

	void noMenu ()
	{
		/*
		 * 	Placeholder implementation or
		 * 	for buttons that are meant to have
		 * 	no effect
		 */
	}

	// === Trivia Game Menus and Combat Menus === //

	Game game;

	/*
	 * 	These answer methods should be done with Action<int>[],
	 * 	but it's good enough for now
	 */
	void answer1 ()
	{
		game.answerClicked (0);
	}

	void answer2 ()
	{
		game.answerClicked (1);
	}

	void answer3 ()
	{
		game.answerClicked (2);
	}

	void answer4 ()
	{
		game.answerClicked (3);
	}

	void startGame ()
	{
		game = new Game (gameLogic.gameHP, gameLogic.secondsPerRound, gameLogic.damagePerAttack);
		currentMenu = continueGame;
	}

	void continueGame ()
	{
		game.nextRound ();
		currentMenu = triviaRound;
	}

	void endGame ()
	{
		gameLogic.displayText = "";

		if (game.playerHealth <= 0) {
			setDisplayColor (Color.red);
			gameLogic.displayText += "YOU LOSE!\n\n";
		} else {
			// Dark Green
			setDisplayColor (new Color (0.0f, 0.5f, 0.0f));
			gameLogic.displayText += "YOU WIN!\n\n";
		}

		gameLogic.displayText += gameLogic.changeReputation (game.playerHealth);

		currentMenu = endGameScreen;
	}

	void endGameScreen ()
	{
		setDisplayImage (images [5]);

		slider.value -= Time.deltaTime;
		slider.GetComponentInChildren<Text> ().text = "" + (int)slider.value;

		setDisplayText (gameLogic.displayText);

		if (slider.value <= 0) {
			slider.value = game.roundTime / 2;
			slider.GetComponentInChildren<Text> ().text = "" + 0;
			objectVisibility (false, false, false);
			currentMenu = mainMenu;
		}
	}

	void triviaRound ()
	{
		game.currentTriviaRound.Update ();

		slider.value = game.currentTriviaRound.timeRemaining ();
		slider.GetComponentInChildren<Text> ().text = "" + (int)slider.value;

		setButtonsText (game.currentTriviaRound.answers);
		setButtonBehaviors (new Action[] {
			answer1,
			answer2,
			answer3,
			answer4
		});
		setDisplayImage (images [5]);
		if (!game.currentTriviaRound.answered ()) {
			setDisplayColor (Color.blue);
		} else {
			if (game.currentTriviaRound.playerAttacks (computer)) {
				// Dark Green
				setDisplayColor (new Color (0.0f, 0.5f, 0.0f));
			} else if (game.currentTriviaRound.correct) {
				// Dark Yellow
				setDisplayColor (new Color (0.5f, 0.5f, 0.0f));
			} else {
				setDisplayColor (Color.red);
			}
		}

		setDisplayText (game.textToDisplay ());

		currentMenu = triviaRound;

		if (game.currentTriviaRound.proceedToCombat) {
			game.nextCombat ();

			RectTransform bounds = primaryDisplay.GetComponent<RectTransform> ();
			gameLogic.randomlyPlaceStar (bounds, objects);
			gameLogic.computer.placeBlock (game.currentTriviaRound.playerAttacks (computer), bounds, objects);

			currentMenu = combatRound;

			if (game.currentTriviaRound.skipCombat (computer)) {
				currentMenu = continueGame;
			}
		}
	}

	int damageDealt;
	int damageBlocked;

	void combatRound ()
	{
		setDisplayColor (Color.clear);
		setDisplayText ("");

		game.currentCombatRound.Update ();

		slider.value = game.currentCombatRound.timeRemaining ();
		slider.GetComponentInChildren<Text> ().text = "" + (int)slider.value;

		if (game.currentTriviaRound.playerAttacks (computer)) {
			setButtonsText (new string[] { "",
				"",
				"",
				""
			});
			objectVisibility (true, false, true);
		} else {
			setButtonsText (new string[] { "",
				"",
				"",
				""
			});
			objectVisibility (false, true, true);
		}

		currentMenu = combatRound;

		if (game.currentCombatRound.calculateDamage) {

			objectVisibility (true, true, true);

			// This calculates damageDealt, damageBlocked
			game.currentCombatRound.damageCalc (objects [0], objects [1], objects [2], out damageDealt, out damageBlocked);

			if (game.currentTriviaRound.playerAttacks (computer)) {
				game.dealDamage (0, damageDealt - damageBlocked);
			} else {
				game.dealDamage (damageDealt - damageBlocked, 0);
			}

			slider.value = game.roundTime / 2;
			slider.GetComponentInChildren<Text> ().text = "" + (int)slider.value;
			currentMenu = combatResults;
		}
	}

	void combatResults ()
	{
		slider.value -= Time.deltaTime;
		slider.GetComponentInChildren<Text> ().text = "" + (int)slider.value;

		setButtonsText (new string[] { "Damage Dealt: " + damageDealt,
			"Damage Blocked: " + damageBlocked,
			"Your Health: " + game.playerHealth,
			"Their Health: " + game.enemyHealth
		});

		if (slider.value > 0) {

			// Make the sword and shield uninteractable at this point
			objects [0].GetComponent<Draggable> ().draggingEnabled = false;
			objects [1].GetComponent<Draggable> ().draggingEnabled = false;

			currentMenu = combatResults;
		} else if (game.playerHealth < 1 || game.enemyHealth < 1) {
			slider.value = game.roundTime / 2;
			currentMenu = endGame;
		} else {

			// Restore ability to drag objects around for later rounds
			objects [0].GetComponent<Draggable> ().draggingEnabled = true;
			objects [1].GetComponent<Draggable> ().draggingEnabled = true;

			currentMenu = continueGame;
		}
	}

	public void debugWipeAllSettings ()
	{
		slider.value = 10;
		currentMenu = wipeSettingsVerification;
	}

	void wipeSettingsVerification ()
	{
		slider.value -= Time.deltaTime;
		slider.GetComponentInChildren<Text> ().text = "" + (int)slider.value;

		setDisplayImage (images [5]);
		setDisplayColor (Color.red);

		setDisplayText (String.Format ("WIPING *ALL* LOCAL USER DATA IN\n{0} SECONDS!\n\n", Mathf.Round (slider.value)) +
		"IF THIS IS NOT INTENDED, EXIT THE APP IMMEDIATELY"
		);

		if (slider.value > 0) {
			currentMenu = wipeSettingsVerification;
		} else {
			slider.value = 5;
			gameLogic.deleteAllPrefs ();
			currentMenu = mainMenu;
		}
	}
}

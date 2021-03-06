﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Text.RegularExpressions;

public class UIManager : MonoBehaviour
{

	public string addScoreURL = "http://meducate.cs.unc.edu/addscore.php";
	public string highscoreURL = "http://meducate.cs.unc.edu/display.php";
	public string uniqueUsernameURL = "http://meducate.cs.unc.edu/uniqueUsername.php";

	// Leaderboard text, updated by web request
	public string Scores;

	// Username update text, updated by web request
	public string usernameWaitText;

	private Settings settings;
	private ComputerPlayer computer;
	private GameLogicManager gameLogic;
	private SoundEffectManager soundEffects;

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
		settings = GameObject.Find ("GameLogicManager").GetComponent<GameLogicManager> ().settings;
		computer = GameObject.Find ("GameLogicManager").GetComponent<GameLogicManager> ().computer;
		gameLogic = GameObject.Find ("GameLogicManager").GetComponent<GameLogicManager> ();
		soundEffects = GameObject.Find ("Sound Effect Manager").GetComponent<SoundEffectManager> ();
		slider = GameObject.Find ("Timer Slider").GetComponent<Slider> ();
		slider.maxValue = gameLogic.secondsPerRound;
		currentMenu = connectingToServer;

		objectVisibility (false, false, false);
	}
	
	// The current menu method is called once per frame
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

		if (gameLogic.currentlyNetworking ()) {
			networkedObjectAuthority (sword, shield, star);
		}
	}

	/*
	 * 	Propagates the server's understanding of object visibility
	 * 	and authority to the client.
	 */
	void networkedObjectAuthority (bool sword, bool shield, bool star)
	{
		if (gameLogic.isServer) {
			// If the server can see the object, it has authority over it
			gameLogic.ourGamestate.RpcUpdateClientSwordAuthority (!sword);
			gameLogic.ourGamestate.RpcUpdateClientShieldAuthority (!shield);
			gameLogic.ourGamestate.RpcUpdateClientStarAuthority (!star);

			gameLogic.theirGamestate.RpcUpdateClientSwordAuthority (!sword);
			gameLogic.theirGamestate.RpcUpdateClientShieldAuthority (!shield);
			gameLogic.theirGamestate.RpcUpdateClientStarAuthority (!star);
		}
	}

	/*
	 * 	This method receives the number of the button just pressed
	 * 	and then runs the method associated with the button
	 */
	public void buttonPressed (int i)
	{
		soundEffects.buttonPressSound ();
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

	void mainMenu ()
	{
		setButtonsText (new string[] { "SINGLEPLAYER >>>", 
			"MULTIPLAYER >>>",
			"PROFILE >>>",
			"Exit"
		});
		setButtonBehaviors (new Action[] { singlePlayer, multiPlayer, profile, exitGame });

		setDisplayImage (images [0]);
		setDisplayColor (Color.white);
		setDisplayText ("");

		currentMenu = mainMenu;
	}

	// Exits the game (but not the Unity editor)
	void exitGame ()
	{
		Application.Quit ();

		// Unity standalone crash bug fix
		if (!Application.isEditor) {
			System.Diagnostics.Process.GetCurrentProcess ().Kill ();
		}
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
		"PLAY AGAINST A COMPUTER\n" +
		"OPPONENT OF ANY DIFFICULTY\n\n" +
		"TUTORIAL -\n" +
		"LEARN HOW TO PLAY\n\n" +
		"CAMPAIGN -\n" +
		"SEE IF YOU CAN COMPLETE ALL\n" +
		"NINE LEVELS WITH YOUR\n" +
		"FAVORITE QUESTION SET");

		currentMenu = singlePlayer;
	}

	void multiPlayer ()
	{
		setButtonsText (new string[] { "QUICK PLAY >>>",
			"ONE MAN ARMY >>>",
			"<<< SHOW LEADERBOARD >>>",
			"<<< BACK TO MENU"
		});
		setButtonBehaviors (new Action[] { multiPlayerQuickPlay, multiPlayerOneManArmy, leaderboardLoad, mainMenu });

		setDisplayImage (images [2]);
		setDisplayColor (Color.red);
		setDisplayText ("MAIN MENU > MULTIPLAYER\n\n" +
		"QUICK PLAY -\n" +
		"PLAY AGAINST ANOTHER PERSON\n" +
		"OVER THE INTERNET\n\n" +
		"ONE MAN ARMY -\n" +
		"PLAY AGAINST THE COMPUTER AND\n" +
		"RISK REPUTATION TO EARN A SPOT\n" +
		"ON THE LEADERBOARD");

		currentMenu = multiPlayer;
	}

	void profile ()
	{
		setButtonsText (new string[] {
			"SELECT A QUESTION SET >>>",
			"CHANGE SETS AND USERNAME >>>",
			"REVIEW QUESTIONS >>>",
			"<<< BACK TO MENU"
		});
		setButtonBehaviors (new Action[] { changeQuestions, addQuestions, loadReviewQuestions, mainMenu });

		setDisplayImage (images [3]);
		setDisplayColor (Color.blue);
		setDisplayText ("MAIN MENU > PROFILE\n\n" +
		"SELECT A QUESTION SET -\n" +
		"CHANGE YOUR SELECTED SET\n\n" +
		"CHANGE SETS AND USERNAME -\n" +
		"IMPORT YOUR OWN QUESTION\n" +
		"SETS AND CHANGE YOUR\n" +
		"ONLINE USERNAME\n\n" +
		"REVIEW QUESTIONS -\n" +
		"SHOW ALL THE QUESTIONS AND\n" +
		"ANSWERS FROM THE\n" +
		"SELECTED QUESTION SET");

		// Make sure the input field for adding questions is not being shown
		gameLogic.inputfield.SetActive (false);

		currentMenu = profile;
	}

	// This menu also allows the user to change their username
	void addQuestions ()
	{
		setButtonsText (new string[] {
			"<<< Add Set To Device >>>",
			"<<< Remove Set From Device >>>",
			"<<< Use as new Username >>>",
			"<<< BACK TO PROFILE"
		});
		setButtonBehaviors (new Action[] { addThisSet, removeThisSet, requestNewUserName, profile });

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("Go to www.meducate.cs.unc.edu\n" +
		"to update and add question sets!\n\n" +
		"Then, come back and type the name\n" +
		"of the Question Set here!\n\n" +
		"You can also change your username here!\n\n" +
		"Current Username: " + gameLogic.username);

		// Show the input field for adding questions and changing usernames
		gameLogic.inputfield.SetActive (true);

		currentMenu = addQuestions;
	}

	void requestNewUserName ()
	{
		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		StartCoroutine (newUsername (gameLogic.inputfield.GetComponentInChildren<InputField> ().text));
		gameLogic.inputfield.GetComponentInChildren<InputField> ().text = "Waiting for server...";
		currentMenu = addQuestions;
	}

	// Checks whether the username is already taken on the server
	IEnumerator newUsername (string desiredName)
	{
		// Usernames are stored in uppercase
		desiredName = desiredName.ToUpper ();
		WWW name_get = new WWW (uniqueUsernameURL + "?name=" + WWW.EscapeURL (desiredName));
		yield return name_get;

		if (name_get.error != null) {
			gameLogic.inputfield.GetComponentInChildren<InputField> ().text = "Cannot access server...";
		} else {
			if (String.Equals (name_get.text, "name already exists\n")) {
				gameLogic.inputfield.GetComponentInChildren<InputField> ().text = "Username already taken!";
			} else if (String.Equals (name_get.text, "username updated\n")) {
				gameLogic.inputfield.GetComponentInChildren<InputField> ().text = "Updated Username!";
				changeUsername (desiredName);
			} else {
				gameLogic.inputfield.GetComponentInChildren<InputField> ().text = "Cannot access server...";
			}
		} 
	}

	void addThisSet ()
	{
		string setName = gameLogic.inputfield.GetComponentInChildren<InputField> ().text.Replace (' ', '_');
		gameLogic.setsToAdd.Enqueue (setName);

		gameLogic.updatePlayerPrefs ();
		gameLogic.inputfield.GetComponentInChildren<InputField> ().text = "Requested Set From Server!";

		currentMenu = addQuestions;
	}

	void removeThisSet ()
	{
		// Make sure we're not removing beyond our selected set index
		gameLogic.setCurrentSet (0);

		string setName = gameLogic.inputfield.GetComponentInChildren<InputField> ().text;
		for (int i = 0; i < gameLogic.questionSets.Count; i++) {
			if (String.Equals (gameLogic.questionSets [i].setName.ToLower (), setName.ToLower ())) {
				gameLogic.questionSets.RemoveAt (i);
			}
		}

		// Restore selected set to first set
		gameLogic.setCurrentSet (0);

		gameLogic.updatePlayerPrefs ();
		gameLogic.inputfield.GetComponentInChildren<InputField> ().text = "Removed Set From Device!";

		currentMenu = addQuestions;
	}

	// Converts input to all capital letters before changing the username
	void changeUsername (string newName)
	{
		gameLogic.username = newName.ToUpper ();

		gameLogic.updatePlayerPrefs ();
		StartCoroutine (PostScores (gameLogic.username, gameLogic.reputation));

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
		"\n\nCURRENT OPPONENT SPEED -\n" +
		computer.getSpeedString ());

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
		gameLogic.randomlyPlaceBlocks (bounds, objects);

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
		gameLogic.randomlyPlaceBlocks (bounds, objects);

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
		"IF YOU'RE CONFIDENT IN YOUR SKILLS,\n" +
		"TRY JUMPING STRAIGHT INTO MULTIPLAYER\n\n" +
		"HAVE FUN AND DON'T FORGET TO SET YOUR\n" +
		"USERNAME IN THE PROFILE MENU!\n\n" +
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
		String.Format ("LEVEL {0} of {1} -\n", computer.level + 1, computer.numberOfLevels ()) +
		computer.getLevelString () +
		"\n\nLEVEL STATUS -\n" +
		gameLogic.getCampaignScore (computer.level));

		currentMenu = singlePlayerCampaign;
	}

	void startCampaign ()
	{
		gameLogic.gameMode = "Campaign";
		currentMenu = startGame;
	}

	void multiPlayerQuickPlay ()
	{
		setButtonsText (new string[] { "<<< HOST GAME >>>",
			"<<< JOIN GAME >>>", 
			"",
			"<<< BACK TO MULTIPLAYER"
		});
		setButtonBehaviors (new Action[] {
			setupHostGameMenu,
			setupJoinGameMenu,
			noMenu,
			multiPlayer
		});

		gameLogic.matchMaker.endMatchMaker ();

		slider.value = gameLogic.secondsPerRound / 2;
		slider.GetComponentInChildren<Text> ().text = "" + 0;

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("MAIN MENU > MULTIPLAYER > QUICK PLAY\n\n" +
		"CURRENT QUESTION SET -\n" +
		settings.selected.setName +
		"\n(GO TO PROFILE TO SELECT A DIFFERENT SET)\n\n" +
		"BOTH PLAYER'S WILL USE THE HOST'S\n" +
		"SELECTED QUESTION SET");

		currentMenu = multiPlayerQuickPlay;
	}

	void setupHostGameMenu ()
	{
		System.Random random = new System.Random ();
		gameLogic.roomName = gameLogic.username +
		"-" + gameLogic.settings.selected.setName + "-" +
		random.Next (1000, 9999);
		currentMenu = multiPlayerHostGame;
	}

	void hostGame ()
	{
		gameLogic.matchMaker.startMatchMaker ();
		gameLogic.matchMaker.startMatch (gameLogic.roomName);
		currentMenu = multiPlayerQuickPlayStartGame;
	}

	void multiPlayerHostGame ()
	{
		setButtonsText (new string[] { "<<< START >>>",
			"", 
			"",
			"<<< BACK TO QUICK PLAY"
		});
		setButtonBehaviors (new Action[] {
			hostGame,
			noMenu,
			noMenu,
			multiPlayerQuickPlay
		});

		slider.value = gameLogic.secondsPerRound / 2;
		slider.GetComponentInChildren<Text> ().text = "" + 0;

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("MAIN MENU > MULTIPLAYER > QUICK PLAY > HOST GAME\n\n" +
		"CURRENT QUESTION SET -\n" +
		settings.selected.setName +
		"\n(GO TO PROFILE TO SELECT A DIFFERENT SET)\n\n" +
		"ROOM NAME:\n" + gameLogic.roomName);

		currentMenu = multiPlayerHostGame;
	}

	void setupJoinGameMenu ()
	{
		gameLogic.matchMaker.joiningGame = false;
		gameLogic.roomNumber = 0;
		gameLogic.matchMaker.startMatchMaker ();
		gameLogic.matchMaker.listMatches ();
		currentMenu = multiPlayerJoinGame;
	}

	void multiPlayerJoinGame ()
	{
		string joinText = "<<< JOIN >>>";
		if (gameLogic.matchMaker.joiningGame) {
			joinText = "JOINING...";
		}

		setButtonsText (new string[] { joinText,
			"NEXT ROOM >>>", 
			"<<< REFRESH LIST >>>",
			"<<< BACK TO QUICK PLAY"
		});
		setButtonBehaviors (new Action[] {
			joinGame,
			gameLogic.increaseRoomNumber,
			setupJoinGameMenu,
			multiPlayerQuickPlay
		});

		slider.value = gameLogic.secondsPerRound / 2;
		slider.GetComponentInChildren<Text> ().text = "" + 0;

		string roomNameToJoin = "Loading...";
		string roomCountString = "";
		if (gameLogic.matchMaker.roomList != null) {
			int numRooms = gameLogic.matchMaker.roomList.Count;
			if (numRooms > 0) {
				roomNameToJoin = gameLogic.matchMaker.roomNameToJoin (gameLogic.roomNumber);
				roomCountString = (gameLogic.roomNumber + 1) + " OUT OF " + numRooms;
			} else {
				roomNameToJoin = "No rooms exist!";
			}
		}

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("MAIN MENU > MULTIPLAYER > QUICK PLAY > JOIN GAME\n\n" +
		"CURRENT ROOM NAME:\n" +
		roomNameToJoin + "\n" +
		roomCountString);

		currentMenu = multiPlayerJoinGame;
	}

	void joinGame ()
	{
		gameLogic.matchMaker.joinGame (gameLogic.roomNumber);
	}

	void closeNetworkingThenMultiPlayerQuickPlay ()
	{
		gameLogic.closeAllNetworking ();
		currentMenu = multiPlayerQuickPlay;
	}

	float fatalErrorTimeUntilExit = 0;
	bool fatalErrorAlreadyThrown = false;
	/*
	 * 	Unity networking is currently unable to handle errors, so
	 * 	we have to exit the application completely
	 */
	public void fatalError ()
	{
		gameLogic.closeAllNetworking ();

		// Make sure we don't repeatedly reset the exit timer
		if (!fatalErrorAlreadyThrown) {
			fatalErrorTimeUntilExit = gameLogic.secondsPerRound / 2;
			fatalErrorAlreadyThrown = true;
		}
		if (fatalErrorTimeUntilExit > 0) {
			currentMenu = fatalErrorNotification;
		}
	}

	void fatalErrorNotification ()
	{
		// This needs to be done in a variable since errors can break the slider
		fatalErrorTimeUntilExit -= Time.deltaTime;

		slider.value = fatalErrorTimeUntilExit;
		slider.GetComponentInChildren<Text> ().text = "" + (int)fatalErrorTimeUntilExit;

		setDisplayImage (images [5]);
		setDisplayColor (Color.red);
		setDisplayText ("Opponent Disconnected or\n" +
		"we hit a fatal error.\n\n" +
		"If this is reproducible,\n" +
		"please contact the developers.\n\n" +
		String.Format ("Exiting game in {0} seconds...", (int)fatalErrorTimeUntilExit));
		setButtonsText (new string[] { "", "", "", "" });
		setButtonBehaviors (new Action[] { noMenu, noMenu, noMenu, noMenu });

		if (fatalErrorTimeUntilExit <= 0) {
			currentMenu = exitGame;
		} else {
			currentMenu = fatalErrorNotification;
		}
	}

	float currentMultiplayerWaitTime;
	float timeToWaitUntilRandomAI;

	public void multiPlayerQuickPlayStartGame ()
	{
		game = new Game (gameLogic.gameHP, gameLogic.secondsPerRound, gameLogic.damagePerAttack);

		currentMultiplayerWaitTime = 0;
		// If we wait for 30-60 seconds with no opponent, we'll play with a random AI
		timeToWaitUntilRandomAI = 30 + UnityEngine.Random.Range (0, 30);
		currentMenu = multiPlayerQuickPlayWaitForGame;
	}

	public void multiPlayerQuickPlayWaitForGame ()
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
			closeNetworkingThenMultiPlayerQuickPlay
		});

		currentMultiplayerWaitTime += Time.deltaTime;

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("Waiting for another player...\n\n" +
		String.Format ("Time in Queue: {0} seconds", Mathf.Round (currentMultiplayerWaitTime)));

		// If we wait too long, we'll play against an AI
		if (currentMultiplayerWaitTime > timeToWaitUntilRandomAI) {
			slider.value = gameLogic.secondsPerRound / 2;
			currentMenu = closeNetworkingThenAIGame;
		} else {
			currentMenu = multiPlayerQuickPlayWaitForGame;
		}
	}

	// We don't need networking for a multiplayer AI game
	void closeNetworkingThenAIGame ()
	{
		gameLogic.closeAllNetworking ();
		currentMenu = proceedToAIGame;
	}

	void proceedToAIGame ()
	{
		slider.value -= Time.deltaTime;
		slider.GetComponentInChildren<Text> ().text = "" + (int)slider.value;

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("Found an Opponent!\n\n" +
		"Setting up game...");

		setButtonsText (new string[] { "", "", "", "" });
		setButtonBehaviors (new Action[] { noMenu, noMenu, noMenu, noMenu });

		if (slider.value <= 0) {
			gameLogic.computer.level = UnityEngine.Random.Range (0, 9);
			gameLogic.computer.updateSpeedAndDifficulty ();
			gameLogic.gameMode = "Multiplayer Quick Play AI";
//			Debug.Log (gameLogic.computer.getLevelString ());
			currentMenu = startGame;
		} else {
			currentMenu = proceedToAIGame;
		}
	}

	// Called by NetworkGameState once client connects
	public void proceedToMultiplayerGame ()
	{
		slider.value -= Time.deltaTime;
		slider.GetComponentInChildren<Text> ().text = "" + (int)slider.value;

		setButtonsText (new string[] { "", "", "", "" });
		setButtonBehaviors (new Action[] { noMenu, noMenu, noMenu, noMenu });

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("Found an Opponent!\n\n" +
		"Setting up game...");

		if (slider.value <= 0) {
			gameLogic.gameMode = "Multiplayer Quick Play";
			currentMenu = multiplayerSetupNextRound;
		} else {
			currentMenu = proceedToMultiplayerGame;
		}
	}

	void multiplayerSetupNextRound ()
	{
		if (gameLogic.isServer) {
			game.nextRound ();

			Question currentQuestion = game.currentTriviaRound.currentQuestion;
			NetworkGameState network = gameLogic.ourGamestate;

			// Send question information to client
			network.RpcUpdateQuestion (currentQuestion.questionText);
			network.RpcUpdateAnswer1 (currentQuestion.correctAnswer);
			network.RpcUpdateAnswer2 (currentQuestion.incorrectAnswers [0]);
			network.RpcUpdateAnswer3 (currentQuestion.incorrectAnswers [1]);
			network.RpcUpdateAnswer4 (currentQuestion.incorrectAnswers [2]);
			network.RpcUpdateRoundNumber (game.currentTriviaRound.round);
		}

		slider.value = gameLogic.gameSyncTime;
		currentMenu = multiplayerNextRoundSyncTime;
	}

	// Wait some time to ensure client and server data are synced in multiplayer.
	void multiplayerNextRoundSyncTime ()
	{
		slider.value -= Time.deltaTime;
		slider.GetComponentInChildren<Text> ().text = "" + (int)slider.value;

		setButtonBehaviors (new Action[] { noMenu, noMenu, noMenu, noMenu });

		setButtonsText (new String[] { "", "", "", "" });

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("Syncing next round...");

		if (slider.value <= 0) {
			currentMenu = continueGame;
		} else {
			if (slider.value <= gameLogic.gameSyncTime / 2) {
				/*
				 * 	This delay prevents a race condition where the client 
				 * 	and server could get desynced due to answer resetting 
				 * 	being interpreted by the client as the server never
				 *	answering in the last trivia round.
				 */
				game.resetNetworkedAnswers ();
			}
			currentMenu = multiplayerNextRoundSyncTime;
		}
	}

	void multiPlayerOneManArmy ()
	{
		string startGameText = "<<< START GAME >>>";
		if (gameLogic.reputation < settings.getWager ()) {
			startGameText = "NOT ENOUGH REPUTATION TO WAGER";
		}

		setButtonsText (new string[] { startGameText,
			"CHANGE WAGER >>>", 
			"",
			"<<< BACK TO MULTIPLAYER"
		});
		setButtonBehaviors (new Action[] {
			oneManArmyStartGame,
			settings.changeWager,
			noMenu,
			multiPlayer
		});

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("MAIN MENU > MULTIPLAYER > ONE MAN ARMY\n\n" +
		"CURRENTLY SELECTED SET NAME -\n" +
		settings.selected.setName +
		"\n\n" +
		"CURRENT REPUTATION -\n" +
		String.Format ("{0}\n\n", gameLogic.reputation) +
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

	void leaderboardLoad ()
	{
		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		Scores = "Loading Scores";
		StartCoroutine (GetScores ());
		currentMenu = leaderboard;
	}

	void leaderboard ()
	{
		setButtonsText (new string[] { "",
			"", 
			"",
			"<<< BACK TO MULTIPLAYER"
		});
		setButtonBehaviors (new Action[] {
			noMenu,
			noMenu,
			noMenu,
			multiPlayer
		});
		setDisplayText (Scores + "\n\nYOUR REPUTATION: " + gameLogic.reputation);
	}

	// Send scores to leaderboard
	IEnumerator PostScores (string name, int score)
	{ 

		string post_url = addScoreURL + "?name=" + WWW.EscapeURL (name) + "&score=" + score;

		WWW hs_post = new WWW (post_url);
		yield return hs_post;

		if (hs_post.error != null) {
//			Scores = "There was an error posting the high score: " + hs_post.error;
		}
	}

	// Get leaderboard to display
	IEnumerator GetScores ()
	{
		WWW hs_get = new WWW (highscoreURL);
		yield return hs_get;

		if (hs_get.error != null) {
			Scores = "Could not access the leaderboard.\n\n" +
			"Are you connected to the UNC Network?";
//			Scores = "There was an error getting the high score board: " + hs_get.error;
		} else {
			Scores = hs_get.text;
		} 
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
		setDisplayText ("MAIN MENU > PROFILE > SELECT A QUESTION SET\n\n" +
		"CURRENTLY SELECTED SET NAME -\n" +
		settings.selected.setName +
		String.Format ("\n\nSELECTED? - {0}\n", gameLogic.getSetToChange ().selected ? "Yes" : "No") +
		String.Format ("NAME - {0}\n", gameLogic.getSetToChange ().setName) +
		String.Format ("AUTHOR - {0}\n", gameLogic.getSetToChange ().authorName) +
		String.Format ("NUMBER OF QUESTIONS - {0}\n", gameLogic.getSetToChange ().numberOfQuestions ()) +
		String.Format ("(SET {0} OF {1})", gameLogic.setToChange + 1, gameLogic.questionSets.Count));

		currentMenu = changeQuestions;
	}

	/*
	 * 	Make sure that the answers are updated for the current question
	 * 	before proceeding to the review questions menu.
	 */
	void loadReviewQuestions ()
	{
		gameLogic.updateShuffledAnswers ();
		currentMenu = reviewQuestions;
	}

	void reviewQuestions ()
	{
		string hideOrShow = "HIDE INCORRECT ANSWERS";
		if (gameLogic.hideAnswers) {
			hideOrShow = "SHOW ALL ANSWERS";
		}

		setButtonsText (new string[] { String.Format ("<<< {0} >>>", hideOrShow),
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

		string text = settings.selected.setName +
		              String.Format ("\n\nQUESTION {0} OF {1} -\n", gameLogic.currentQuestion + 1,
			              gameLogic.getSetToChange ().numberOfQuestions ()) +
		              gameLogic.getCurrentQuestion ().questionText +
		              "\n\n";
		
		text += "ANSWER CHOICES -";

		int answerCount = 1;
		foreach (string s in gameLogic.shuffledAnswers) {

			// We won't display blank answer choices.
			if (!String.IsNullOrEmpty (s)) {
				
				if (String.Equals (s, gameLogic.getCurrentQuestion ().correctAnswer)) {
					text += "\n" + answerCount + ") " + s;
				} else {
					if (gameLogic.hideAnswers) {
						/*
						 * 	If we're showing the correct answer, we'll hide the incorrect
						 * 	ones with asterisks.
						 * 
						 * 	We need to keep the spaces to ensure consistent formatting in
						 * 	Unity's text element.
						 */
						text += "\n" + answerCount + ") " + new Regex ("\\S").Replace (s, "*");
					} else {
						text += "\n" + answerCount + ") " + s;
					}
				}

				answerCount += 1;
			}
		}

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

			string slowLoading = "";

			/*
			 * 	If the server is taking a while to respond, remind the player that
			 * 	the meducate server is only available while on the UNC network.
			 */
			if (gameLogic.serverSlowToRespond ()) {
				slowLoading = "This is taking longer than expected...\n\n" +
				"You seem to be connected to the internet,\n" +
				"but the server is not responding.\n" +
				"Are you sure you're on UNC WiFi?\n" +
				String.Format ("Giving up in {0} seconds...", (int)gameLogic.timeLeftToWaitForServer);
			}

			setDisplayText ("Connecting to meducate.cs.unc.edu...\n\n" +
			String.Format ("Retrieving {0}\n\n", gameLogic.setsToAdd.Peek ()) +
			slowLoading);
		} else {
			if (gameLogic.completedTutorial) {
				currentMenu = mainMenu;
			} else {
				currentMenu = tutorialStart;
			}
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
		// Just in case, make sure objects are draggable
		objects [0].GetComponent<Draggable> ().draggingEnabled = true;
		objects [1].GetComponent<Draggable> ().draggingEnabled = true;

		game = new Game (gameLogic.gameHP, gameLogic.secondsPerRound, gameLogic.damagePerAttack);
		currentMenu = continueGame;
	}

	void continueGame ()
	{
		if (gameLogic.currentlyNetworking ()) {
			if (!gameLogic.isServer) {
				NetworkGameState network = gameLogic.theirGamestate;

				game.networkedNextRound (network.questionText, network.answer1, network.answer2, network.answer3, network.answer4);

				if (game.currentTriviaRound.round != gameLogic.theirGamestate.roundNumber) {
					/*
					 * 	If the client and server get desynced, procced to the fatalError() menu.
					 * 
					 * 	If this happens, we'll probably hit another exception first.
					 */
					fatalError ();
				}
			} else {
				// The server randomly places the star in multiplayer.
				RectTransform bounds = primaryDisplay.GetComponent<RectTransform> ();
				gameLogic.randomlyPlaceBlocks (bounds, objects);
			}
		} else {
			game.nextRound ();

			RectTransform bounds = primaryDisplay.GetComponent<RectTransform> ();
			gameLogic.randomlyPlaceBlocks (bounds, objects);
		}

		currentMenu = triviaRound;
	}

	void endGame ()
	{
		gameLogic.displayText = "";

		// Purple
		setDisplayColor (new Color (0.5f, 0, 0.5f));

		if (game.playerHealth <= 0) {
			gameLogic.displayText += "YOU LOSE!\n\n";
		} else {
			gameLogic.displayText += "YOU WIN!\n\n";

			// Play a fanfare sound effect
			soundEffects.winFanfareSound ();
		}

		// At the end of a multiplayer game, close all networking interfaces.
		if (gameLogic.currentlyNetworking ()) {
			gameLogic.closeAllNetworking ();
		}

		setButtonsText (new string[] { "", "", "", "" });
		setButtonBehaviors (new Action[] { noMenu, noMenu, noMenu, noMenu });

		// Restore ability to drag objects around for later games
		objects [0].GetComponent<Draggable> ().draggingEnabled = true;
		objects [1].GetComponent<Draggable> ().draggingEnabled = true;

		gameLogic.displayText += gameLogic.changeReputation (game.playerHealth);

		// Reset game mode
		gameLogic.gameMode = "";

		StartCoroutine (PostScores (gameLogic.username, gameLogic.reputation));
		setDisplayText (Scores);
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

		/*
		 * 	If we add extra time, don't change the slider value.
		 * 	
		 * 	Otherwise, the slider will "jump" back three seconds once
		 * 	it hits zero.
		 */
		if (!game.currentTriviaRound.addedExtraTime) {
			slider.value = game.currentTriviaRound.timeRemaining ();
			slider.GetComponentInChildren<Text> ().text = "" + (int)slider.value;
		}

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

		/*
		 * 	If we needed to add extra time to allow players to read the results of the round,
		 * 	add a countdown to the bottom of the primary display.
		 */
		string text = game.textToDisplay ();
		if (game.currentTriviaRound.addedExtraTime) {
			int timeLeft = (int)Math.Round (game.currentTriviaRound.timeRemaining ());
			text += String.Format ("\n\nCONTINUING IN {0} SECOND{1}...", timeLeft, 
				(timeLeft == 1) ? "" : "S");
		}

		setDisplayText (text);

		currentMenu = triviaRound;

		if (game.currentTriviaRound.proceedToCombat) {
			game.nextCombat ();

			RectTransform bounds = primaryDisplay.GetComponent<RectTransform> ();

			if (!gameLogic.currentlyNetworking ()) {
				// The computer places their sword/shield block in this case.
				gameLogic.computer.placeBlock (game.currentTriviaRound.playerAttacks (computer), bounds, objects);
			}
			
			currentMenu = combatRound;

			if (game.currentTriviaRound.skipCombat (computer)) {
				if (gameLogic.currentlyNetworking ()) {
					currentMenu = multiplayerSetupNextRound;
				} else {
					currentMenu = continueGame;
				}
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

		setButtonsText (new string[] { "",
			"",
			"",
			""
		});

		if (game.currentTriviaRound.playerAttacks (computer)) {
			// We move the sword
			objectVisibility (true, false, true);
		} else {
			// We move the shield
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

			if (gameLogic.currentlyNetworking ()) {
				if (gameLogic.isServer) {
					// The server propagates health and damage info to the client.
					gameLogic.theirGamestate.RpcUpdateClientHealth (game.enemyHealth);
					gameLogic.theirGamestate.RpcUpdateServerHealth (game.playerHealth);

					gameLogic.theirGamestate.RpcUpdateDamageDealt (damageDealt);
					gameLogic.theirGamestate.RpcUpdateDamageBlocked (damageBlocked);

					gameLogic.theirGamestate.RpcSwordPosition (objects [0].transform.position);
					gameLogic.theirGamestate.RpcShieldPosition (objects [1].transform.position);
					gameLogic.theirGamestate.RpcStarPosition (objects [2].transform.position);
				}

				slider.value = gameLogic.gameSyncTime;
				currentMenu = multiplayerCombatResultsSyncTime;
			} else {
				slider.value = game.roundTime / 4;

				// Play a sword clash sound effect when objects appear
				soundEffects.swordClashSound ();

				currentMenu = combatResults;
			}
		}
	}

	// Wait some time to ensure client and server data are synced in multiplayer.
	void multiplayerCombatResultsSyncTime ()
	{
		slider.value -= Time.deltaTime;
		slider.GetComponentInChildren<Text> ().text = "" + (int)slider.value;

		// Make the sword and shield uninteractable at this point
		objects [0].GetComponent<Draggable> ().draggingEnabled = false;
		objects [1].GetComponent<Draggable> ().draggingEnabled = false;

		setButtonsText (new string[] { "Syncing results",
			"Syncing results",
			"Syncing results",
			"Syncing results"
		});

		/*
		 * 	Make sure the client accepts the server's game piece positions
		 * 
		 * 	This will cause some strange piece movement on the client's end
		 * 	if the server and client disagree (i.e. if the client manages to
		 * 	move his/her game piece after the server thinks the round has
		 * 	already finished, the client's game piece will snap back to its
		 * 	position from a fraction of a second prior).
		 */
		if (gameLogic.currentlyNetworking () && !gameLogic.isServer) {
			gameLogic.ourGamestate.swordPos = gameLogic.theirGamestate.swordPos;
			gameLogic.ourGamestate.shieldPos = gameLogic.theirGamestate.shieldPos;
			gameLogic.ourGamestate.starPos = gameLogic.theirGamestate.starPos;
		}

		if (slider.value <= 0) {
			if (gameLogic.currentlyNetworking () && !gameLogic.isServer) {
				// The client accepts the server's health and damage info at this point.
				game.playerHealth = gameLogic.ourGamestate.clientHealth;
				game.enemyHealth = gameLogic.ourGamestate.serverHealth;

				damageDealt = gameLogic.ourGamestate.damageDealt;
				damageBlocked = gameLogic.ourGamestate.damageBlocked;
			}

			slider.value = game.roundTime / 4;
			objectVisibility (true, true, true);

			// Play a sword clash sound effect when objects appear
			soundEffects.swordClashSound ();

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

		/*
		 * 	Make sure the networking knows the game is over, so we
		 * 	don't throw an error when the other player disconnects.
		 * 
		 * 	This has to be done before proceeding to endGame() to
		 * 	avoid a race condition.
		 */
		if (gameLogic.currentlyNetworking ()) {
			if (game.playerHealth < 1 || game.enemyHealth < 1) {
				gameLogic.ourGamestate.gameOver = true;
			}
		}

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

			if (gameLogic.currentlyNetworking ()) {
				currentMenu = multiplayerSetupNextRound;
			} else {
				currentMenu = continueGame;
			}
		}
	}

	// Wipe all saved user data for debugging (see GameLogicManager)
	public void debugWipeAllSettings ()
	{
		slider.value = gameLogic.secondsPerRound;
		currentMenu = wipeSettingsVerification;
	}

	// Warns the user that we're wiping all settings for debugging.
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
			slider.value = gameLogic.secondsPerRound / 2;
			gameLogic.deleteAllPrefs ();
			currentMenu = mainMenu;
		}
	}
}

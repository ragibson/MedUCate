using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class UIManager : MonoBehaviour
{

	/*
	 * 	TODO: The following menu items were not
	 * 	implemented in the original prototype
	 * 	and have not been implemented here:
	 * 		ADD QUESTIONS
	 * 		VIEW HISTORY
	 * 		HOST GAME
	 * 		JOIN GAME
	 * 
	 * 	TODO: LEADERBOARD was a placeholder in
	 * 	the original prototype and has also
	 * 	not been implemented here.
	 * 
	 * 	TODO: Possibly store the menus in an
	 * 	XML file so they don't take up so much
	 * 	space here.
	 */

	private Settings settings;
	private ComputerPlayer computer;
	private GameLogicManager gameLogic;

	public GameObject primaryDisplay;
	public Slider slider;
	public Button[] buttons;
	public Sprite[] images;
	private Action[] buttonBehaviors;

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
		mainMenu ();
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
	 * 	params:
	 * 		buttonText: string[] of [Button1Text, ...]
	 */
	void setButtonsText (string[] buttonText)
	{
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
			"TRAINING >>>", 
			"CAMPAIGN >>>", 
			"<<< BACK TO MENU"
		});
		setButtonBehaviors (new Action[] { singlePlayerQuickPlay, singlePlayerTraining, singlePlayerCampaign, mainMenu });

		setDisplayImage (images [1]);
		setDisplayColor (Color.blue);
		setDisplayText ("MAIN MENU > SINGLEPLAYER\n\n" +
		"QUICK PLAY -\n" +
		"PLAY YOUR SELECTED QUESTIONS\n" +
		"AGAINST A COMPUTER OPPONENT\n\n" +
		"TRAINING -\n" +
		"NO COMBAT, JUST A NUMBER OF\n" +
		"ROUNDS TO PRACTICE QUESTIONS\n\n" +
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
		"RANDOM HUMAN OPPONENT WITH\n" +
		"NO STAKES\n\n" +
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
			"CHANGE QUESTIONS >>>",
			"ADD QUESTIONS >>>",
			"REVIEW QUESTIONS >>>",
			"<<< BACK TO MENU"
		});
		setButtonBehaviors (new Action[] { changeQuestions, noMenu, reviewQuestions, mainMenu });

		setDisplayImage (images [3]);
		setDisplayColor (Color.blue);
		setDisplayText ("MAIN MENU > PROFILE\n\n" +
		"CHANGE QUESTIONS -\n" +
		"CHANGE YOUR CURRENT QUESTION\n" +
		"SET AND RESETS THE\n" +
		"CAMPAIGN PROGRESS\n\n" +
		"ADD QUESTIONS -\n" +
		"IMPORT YOUR OWN QUESTIONS\n" +
		"FROM A CODE TO CUSTOMIZE\n" +
		"YOUR GAMES\n\n" +
		"REVIEW QUESTIONS -\n" +
		"SHOW ALL THE QUESTIONS FOR THE\n" +
		"CURRENT QUESTION SET");

		currentMenu = profile;
	}

	void singlePlayerQuickPlay ()
	{
		setButtonsText (new string[] { "<<< START GAME >>>",
			"CHANGE COMPUTER DIFFICULTY >>>", 
			"CHANGE COMPUTER SPEED >>>",
			"<<< BACK TO SINGLEPLAYER"
		});
		setButtonBehaviors (new Action[] {
			startGame,
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

	void singlePlayerTraining ()
	{
		setButtonsText (new string[] { "<<< START GAME >>>",
			"CHANGE ROUNDS >>>", 
			"CHANGE TYPE >>>",
			"<<< BACK TO SINGLEPLAYER"
		});
		setButtonBehaviors (new Action[] {
			startGame,
			settings.changeRounds,
			settings.changeTrainingType,
			singlePlayer
		});

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("MAIN MENU > SINGLEPLAYER > TRAINING\n\n" +
		"CURRENT QUESTIONS -\n" +
		settings.selected.setName +
		"\n(GO TO PROFILE TO CHANGE QUESTIONS)\n\n" +
		"CURRENT LENGTH -\n" +
		settings.getLengthString () +
		String.Format ("\n(OPTION {0} OF {1})\n\n", settings.round + 1, settings.rounds.Length) +
		"CURRENT MODE -\n" +
		settings.getTrainingModeString () +
		String.Format ("\n(OPTION {0} OF {1})", settings.trainingMode + 1, settings.trainingModes.Length));

		currentMenu = singlePlayerTraining;
	}

	void singlePlayerCampaign ()
	{
		setButtonsText (new string[] { "<<< START GAME >>>",
			"CHANGE LEVEL >>>", 
			"<<< CHANGE LEVEL",
			"<<< BACK TO SINGLEPLAYER"
		});
		setButtonBehaviors (new Action[] {
			startGame,
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
		"NOT YET COMPLETED\n" +
		"(THERE ARE 9 LEVELS TOTAL)");

		currentMenu = singlePlayerCampaign;
	}

	void multiPlayerQuickPlay ()
	{
		setButtonsText (new string[] { "<<< START GAME >>>",
			"<<< VIEW HISTORY >>>", 
			"CHANGE TYPE >>>",
			"<<< BACK TO MULTIPLAYER"
		});
		setButtonBehaviors (new Action[] {
			startGame,
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
		setButtonsText (new string[] { "<<< START GAME >>>",
			"<<< SHOW LEADERBOARD >>>", 
			"CHANGE WAGER >>>",
			"<<< BACK TO MULTIPLAYER"
		});
		setButtonBehaviors (new Action[] {
			startGame,
			noMenu,
			settings.changeWager,
			multiPlayer
		});

		setDisplayImage (images [5]);
		setDisplayColor (Color.blue);
		setDisplayText ("SPONSORED QUESTIONS -\n" +
		"DEFAULT NUTRITIONAL HEALTH SET\n" +
		"(THESE CHANGE EVERY DAY)\n\n" +
		"CURRENT REPUTATION -\n" +
		"1000\n" +
		"(THIS CHANGES AS YOU PLAY MORE GAMES)\n\n" +
		"CURRENT WAGER -\n" +
		settings.getWagerString () +
		String.Format ("\n(OPTION {0} OF {1})", settings.wager + 1, settings.wagers.Length));

		currentMenu = multiPlayerOneManArmy;
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
		String.Format ("ACTIVATION CODE - {0}\n", gameLogic.getSetToChange ().activationCode) +
		String.Format ("EXPIRES - {0}\n\n", gameLogic.getSetToChange ().expiryTime) +
		String.Format ("(SET {0} OF {1})", gameLogic.setToChange + 1, gameLogic.questionSets.Length));

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
		text += String.Format ("\n\n(QUESTION {0} OF {1})", gameLogic.currentQuestion + 1, gameLogic.questionSets.Length);

		setDisplayText (text);

		currentMenu = reviewQuestions;
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
		/*
		 * 	200 HP
		 * 	10 seconds per round
		 * 	25 damage per attack
		 */
		game = new Game (200, 10, 25);
		currentMenu = continueGame;
	}

	void continueGame ()
	{
		game.nextRound ();
		currentMenu = triviaRound;
	}

	void endGame ()
	{
		setDisplayImage (images [5]);

		slider.value -= Time.deltaTime;
		slider.GetComponentInChildren<Text> ().text = "" + (int)slider.value;

		if (game.playerHealth == 0) {
			setDisplayColor (Color.red);
			setDisplayText ("YOU LOSE!");
		} else {
			// Dark Green
			setDisplayColor (new Color (0.0f, 0.5f, 0.0f));
			setDisplayText ("YOU WIN!");
		}

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

		if (game.currentTriviaRound.correct) {
			setButtonsText (new string[] { "DRAG THE SWORD BLOCK TO CLAIM AN AREA TO ATTACK",
				"OVERLAP WITH THE STAR BLOCK TO DO MORE DAMAGE",
				"OVERLAP WITH THE SHIELD WILL DO NO DAMAGE",
				"PLACE IT WHERE YOUR OPPONENT WON'T DEFEND"
			});
			objectVisibility (true, false, true);
		} else {
			setButtonsText (new string[] { "DRAG THE SHIELD BLOCK TO CLAIM AN AREA TO DEFEND",
				"OVERLAP WITH THE SWORD BLOCK TO NEGATE DAMAGE",
				"THE STAR BLOCK IS A TEMPTING AREA TO ATTACK",
				"PLACE IT WHERE YOUR OPPONENT WILL ATTACK"
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
			currentMenu = combatResults;
		} else if (game.playerHealth < 1 || game.enemyHealth < 1) {
			slider.value = game.roundTime / 2;
			currentMenu = endGame;
		} else {
			currentMenu = continueGame;
		}
	}
}

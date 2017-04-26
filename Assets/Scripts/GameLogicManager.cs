using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameLogicManager : MonoBehaviour
{

	/*
	 *	In initial prototype, these were set to 200, 10, and 25, respectively.
	 *
	 *	Should consider shortening game length by changing these.
	 */
	public int gameHP;
	public int secondsPerRound;
	public int damagePerAttack;

	public bool debugDeleteAllSettings;

	public bool completedTutorial;
	public int reputation;

	public string displayText = "";

	public string username = "NEW USER";

	public Settings settings = new Settings ();
	public ComputerPlayer computer = new ComputerPlayer ();
	public List<QuestionSet> questionSets = new List<QuestionSet> ();
	int selectedSet = 0;

	// The input field for the "Add Questions" menu
	public GameObject inputfield;

	public float serverTimeout = 10;
	public float timeLeftToWaitForServer = 0;
	public float gameSyncTime = 1;

	WWW setWWW = null;
	String setNameToRetrieve = "";
	bool initialSetupComplete = false;

	// === Profile Change Questions === //
	public int setToChange = 0;

	// === Profile Review Questions === //
	public int currentQuestion = 0;
	public bool hideAnswers = false;

	// ===   Campaign High Scores   === //
	public string gameMode = "";
	public int[] campaignScores;

	// === Multiplayer === //
	public bool isServer = false;
	public NetworkGameState ourGamestate;
	public NetworkGameState theirGamestate;
	public string roomName = "";
	public int roomNumber = 0;

	public MatchMaking matchMaker;

	public Queue<String> setsToAdd = new Queue<String> ();

	// Initialization
	void Start ()
	{
		matchMaker = GameObject.Find ("Network Manager").GetComponent<MatchMaking> ();

		// Get default question sets
		foreach (String s in new String[] { "Mental", "Physical", "Social", "Nutritional" }) {
			setsToAdd.Enqueue ("Default_" + s + "_Health_Set");
		}

		getPlayerPrefs ();

		if (debugDeleteAllSettings) {
			UIManager UI = GameObject.Find ("UI").GetComponent<UIManager> ();
			UI.currentMenu = UI.debugWipeAllSettings;
		}
	}

	private void Awake ()
	{
		Application.logMessageReceived += handleUnityLog;
	}

	void handleUnityLog (string logString, string stackTrace, LogType type)
	{
		if (type.ToString ().Equals ("Exception") || type.ToString ().Equals ("Error")) {
			GameObject.Find ("UI").GetComponent<UIManager> ().fatalError ();
		}
	}

	// Handles the retrieval of question sets with timeout
	void Update ()
	{
		if (timeLeftToWaitForServer > 0 && setWWW != null && setsToAdd.Count > 0) {
			timeLeftToWaitForServer -= Time.deltaTime;

			if (!String.IsNullOrEmpty (setWWW.error)) {
				timeLeftToWaitForServer = 0;
			}

			if (setWWW.isDone) {
				List<Question> questionsToAdd = new List<Question> ();

				// TODO: get QuestionSet author from server
				string author = "MedUCate LLC";

				String fileData = setWWW.text;
				String[] lines = fileData.Split ('\n');
				for (int i = 0; i < lines.Length; i++) {
					String[] lineData = SplitCSVLine (lines [i].Trim ());

					if (!lineIsNotQuestion (lineData)) {
						questionsToAdd.Add (new Question (lineData [0], lineData [1], lineData [2], lineData [3], lineData [4]));
					}
				}

				String setName = setNameToRetrieve.Replace ("_", " ").ToLower ();

				addQuestionSet (new QuestionSet (questionsToAdd.ToArray (), setName, author));

				setWWW = null;
				timeLeftToWaitForServer = 0;
				setsToAdd.Dequeue ();
			}
		} else {
			// If we've actually timed out, we request no more sets
			if (timeLeftToWaitForServer < 0) {
				setsToAdd.Clear ();
			}

			// Check if we need to add any more sets
			if (setsToAdd.Count != 0) {
				retrieveSetFromServer (setsToAdd.Peek ());
			} else {
				if (questionSets.Count == 0) {
					addQuestionSet (new QuestionSet (
						new Question[]{ new Question ("You have no Question Sets", "", "", "", "") },
						"No Question Sets",
						"N/A"));
				}

				if (!initialSetupComplete) {
					updatePlayerPrefs ();
					getPlayerPrefs ();
					initialSetupComplete = true;
				}
			}
		}

		if (currentlyNetworking ()) {
			ourGamestate.CmdUpdateTimeSinceLastSync (0);
		}

		if (String.Equals (gameMode, "Multiplayer Quick Play") && !currentlyNetworking ()) {
			GameObject.Find ("UI").GetComponent<UIManager> ().fatalError ();
		}
	}

	// Returns true if we've waited two seconds or longer for a question set request
	public bool serverSlowToRespond ()
	{
		return timeLeftToWaitForServer != 0 && (timeLeftToWaitForServer < serverTimeout - 2);
	}

	public bool addQuestionSet (QuestionSet setToAdd)
	{
		if (setToAdd.numberOfQuestions () == 0) {
			return false;
		}

		for (int i = 0; i < questionSets.Count; i++) {
			if (String.Equals (questionSets [i].setName.ToLower (), "No Question Sets".ToLower ())) {
				questionSets.RemoveAt (i);
			}
		}

		for (int i = 0; i < questionSets.Count; i++) {

			// If this matches a set name we have, update the old set
			if (String.Equals (questionSets [i].setName, setToAdd.setName)) {
				questionSets [i] = setToAdd;
				return true;
			}
		}

		questionSets.Add (setToAdd);
		return true;
	}

	public string getCampaignScore (int level)
	{
		if (campaignScores [level] == 0) {
			return "NOT YET COMPLETED";
		}
		return String.Format ("HIGH SCORE: {0}/{1}", campaignScores [level], gameHP);
	}

	// Returns whether player got a high score
	public bool updateCampaign (float finalHP)
	{
		if (String.Equals (this.gameMode, "Campaign")) {
			int score = (int)Mathf.Round (finalHP);

			// If the player gets a new high score, we update their scores and reputation
			if (score > this.campaignScores [computer.level]) {
				this.campaignScores [computer.level] = score;
				updatePlayerPrefs ();
				return true;
			}

		}
		return false;
	}

	// Saves settings to file
	public void updatePlayerPrefs ()
	{
		PlayerPrefs.SetString ("Username", username);
		PlayerPrefs.SetInt ("Completed Tutorial", (completedTutorial ? 1 : 0));
		PlayerPrefs.SetInt ("Reputation", reputation);
		for (int i = 0; i < 9; i++) {
			PlayerPrefs.SetInt (String.Format ("Campaign{0}", i), campaignScores [i]);
		}
		PlayerPrefs.SetInt ("Selected Question Set", selectedSet);
		saveAllQuestionSetsToDevice ();
		PlayerPrefs.Save ();
	}

	// Reads settings from file
	public void getPlayerPrefs ()
	{
		username = PlayerPrefs.GetString ("Username");
		campaignScores = new int[9];

		completedTutorial = (PlayerPrefs.GetInt ("Completed Tutorial") == 1);
		reputation = PlayerPrefs.GetInt ("Reputation");
		for (int i = 0; i < 9; i++) {
			campaignScores [i] = PlayerPrefs.GetInt (String.Format ("Campaign{0}", i));
		}
		selectedSet = PlayerPrefs.GetInt ("Selected Question Set");

		if (selectedSet >= questionSets.Count) {
			selectedSet = 0;
		}

		getQuestionSetsFromDevice ();

		setCurrentSet (selectedSet);
	}

	public void deleteAllPrefs ()
	{
		PlayerPrefs.DeleteAll ();

		campaignScores = new int[9];
		reputation = 0;
		completedTutorial = false;
		selectedSet = 0;
		username = "NEW USER";

		questionSets = new List<QuestionSet> ();

		updatePlayerPrefs ();
		getPlayerPrefs ();
	}

	/*
	 * 	Gets all of the question sets from the device using PlayerPrefs
	 * 
	 * 	These are to be used in case the server does not respond so the
	 * 	player can still use offline game modes when not connected to
	 * 	the internet.
	 */
	public void getQuestionSetsFromDevice ()
	{
		String[] setNames = PlayerPrefs.GetString ("Question Set Names").Split ('_');
		foreach (String s in setNames) {
			if (!String.Equals (s, "")) {
				
				int numQuestions = PlayerPrefs.GetInt (s + "_numQuestions");

				List<Question> questions = new List<Question> ();

				for (int i = 0; i < numQuestions; i++) {
					// prefix = setName_questionID_
					String prefix = s + "_" + i + "_";

					questions.Add (new Question (
						PlayerPrefs.GetString (prefix + "q"),
						PlayerPrefs.GetString (prefix + "a1"),
						PlayerPrefs.GetString (prefix + "a2"),
						PlayerPrefs.GetString (prefix + "a3"),
						PlayerPrefs.GetString (prefix + "a4")
					));
				}

				// TODO: get QuestionSet author from server
				string author = "MedUCate LLC";

				addQuestionSet (new QuestionSet (questions.ToArray (), s, author));
			}
		}
	}

	// Saves all of our question sets to the device using PlayerPrefs
	public void saveAllQuestionSetsToDevice ()
	{
		String setNames = "";
		foreach (QuestionSet q in questionSets) {
			saveQuestionSetToDevice (q);
			setNames += q.setName + "_";
		}
		PlayerPrefs.SetString ("Question Set Names", setNames);
	}

	// Saves a question set to the device using PlayerPrefs
	public void saveQuestionSetToDevice (QuestionSet setToSave)
	{
		PlayerPrefs.SetInt (setToSave.setName + "_numQuestions", setToSave.numberOfQuestions ());

		for (int i = 0; i < setToSave.numberOfQuestions (); i++) {
			Question questionToSave = setToSave.questions [i];
			saveQuestionToDevice (setToSave.setName,
				i,
				questionToSave.questionText,
				questionToSave.correctAnswer,
				questionToSave.incorrectAnswers [0],
				questionToSave.incorrectAnswers [1],
				questionToSave.incorrectAnswers [2]);
		}
	}

	// Saves one question to the device using PlayerPrefs
	public void saveQuestionToDevice (String setName, int questionId, string q, string a1, string a2, string a3, string a4)
	{
		/*
		 *	We can use underscores as delimiters here since we replace them with spaces
		 *	when retrieving the question sets from the server.
		 *
		 *	Unity does not support saving of objects to the device, so we must save the fields
		 *	of the objects and rebuild them manually.
		 */
		PlayerPrefs.SetString (setName + "_" + questionId + "_" + "q", q);
		PlayerPrefs.SetString (setName + "_" + questionId + "_" + "a1", a1);
		PlayerPrefs.SetString (setName + "_" + questionId + "_" + "a2", a2);
		PlayerPrefs.SetString (setName + "_" + questionId + "_" + "a3", a3);
		PlayerPrefs.SetString (setName + "_" + questionId + "_" + "a4", a4);
	}

	/*
	 * 	Returns string to display after game ends.
	 * 	This includes change in reputation and campaign high score notification.
	 * 
	 * 	Tutorial gives 500 reputation on first completion.
	 * 
	 * 	Campaign gives 20-100 reputation on new high score, depending on level.
	 * 
	 * 	One Many Army awards and detracts reputation based on wager and win/loss result.
	 * 
	 * 	Single Player Quick Play awards 10-50 reputation on win, depending on level.
	 */
	public string changeReputation (float finalHP = 0)
	{
		string displayText = "";

		int reputationChange = 0;
		if (String.Equals (this.gameMode, "Tutorial") && !completedTutorial) {
			completedTutorial = true;
			reputationChange += 500;
		} else if (String.Equals (this.gameMode, "Campaign")) {
			if (updateCampaign (finalHP)) {
				displayText += String.Format ("CAMPAIGN LEVEL {0}\nNEW HIGH SCORE: {1}\n\n", computer.level + 1, finalHP);
				reputationChange += 10 * (2 + computer.level);
			}
		} else if (String.Equals (this.gameMode, "One Man Army")) {
			if (finalHP <= 0) {
				reputationChange -= settings.getWager ();
			} else {
				reputationChange += settings.getWager ();
			}
		} else if (String.Equals (this.gameMode, "Single Player Quick Play")) {
			if (finalHP > 0) {
				int level = 3 * computer.difficulty + computer.speed;
				reputationChange += 5 * (2 + level);
			}
		}

		displayText += "Reputation ";
		if (reputationChange >= 0) {
			displayText += "+";
		}
		displayText += "" + reputationChange;
		reputation += reputationChange;

		displayText += String.Format ("\n\n Current Reputation: {0}", reputation);

		this.gameMode = "";

		updatePlayerPrefs ();

		return displayText;
	}

	public void retrieveSetFromServer (String s)
	{
		setNameToRetrieve = s.ToLower ();
		string setURL = String.Format ("http://meducate.cs.unc.edu/sets/{0}.csv", setNameToRetrieve);
		setWWW = new WWW (setURL);
		timeLeftToWaitForServer = serverTimeout;

		// Update will handle the rest of the retrieval
	}

	/*
	 * 	params:
	 * 		lineData:	a String[] holding a line of the CSV to parse
	 * 
	 * 	returns true if the line contains only a question number
	 * 
	 * 	returns false if the line contains question text and answers
	 */
	bool lineIsNotQuestion (String[] lineData)
	{
		if (lineData.Length == 1) {
			return true;
		}

		if (!lineData [0].Contains ("Question ")) {
			return false;
		}

		for (int i = 1; i < lineData.Length; i++) {
			if (!String.Equals (lineData [i], "")) {
				return false;
			}
		}

		return true;
	}

	String[] SplitCSVLine (String s)
	{
		int i;
		int a = 0;
		int count = 0;
		List<string> str = new List<string> ();
		for (i = 0; i < s.Length; i++) {
			switch (s [i]) {
			case ',':
				if ((count & 1) == 0) {
					str.Add (s.Substring (a, i - a));
					a = i + 1;
				}
				break;
			case '"':
				count++;
				break;
			}
		}
		str.Add (s.Substring (a));
		return str.ToArray ();
	}

	public bool networkedTheirAnswerCorrect ()
	{
		if (this.isServer) {
			return theirGamestate.clientAnswerCorrect;
		} else {
			return theirGamestate.serverAnswerCorrect;
		}
	}

	public bool networkedOurAnswerCorrect ()
	{
		if (this.isServer) {
			return ourGamestate.serverAnswerCorrect;
		} else {
			return ourGamestate.clientAnswerCorrect;
		}
	}

	public float networkedTheirAnswerTime ()
	{
		if (this.isServer) {
			return theirGamestate.clientAnswerTime;
		} else {
			return theirGamestate.serverAnswerTime;
		}
	}

	public float networkedOurAnswerTime ()
	{
		if (this.isServer) {
			return ourGamestate.serverAnswerTime;
		} else {
			return ourGamestate.clientAnswerTime;
		}
	}

	public bool currentlyNetworking ()
	{
		return ourGamestate != null && theirGamestate != null;
	}

	public void closeAllNetworking ()
	{
		matchMaker.endMatchMaker ();
		GameObject.Find ("Network Manager").GetComponent<NetworkManager> ().StopHost ();
		ourGamestate = null;
		theirGamestate = null;
	}

	// ===   THE FOLLOWING METHODS ARE ALL SETTING   === //
	// === MANIPULATION FROM THE VARIOUS GAME MENUS  === //

	public void setCurrentSet (int i)
	{
		if (questionSets.Count == 0) {
			if (questionSets.Count == 0) {
				addQuestionSet (new QuestionSet (
					new Question[]{ new Question ("You have no Question Sets", "", "", "", "") },
					"No Question Sets",
					"N/A"));
			}
		}

		if (i > questionSets.Count) {
			i = 0;
		}

		setToChange = i;

		questionSets [selectedSet].selected = false;
		selectedSet = i;
		settings.selected = questionSets [selectedSet];
		questionSets [selectedSet].selected = true;

		updatePlayerPrefs ();
	}

	public void increaseCurrentSet ()
	{
		setCurrentSet ((selectedSet + 1) % questionSets.Count);
	}

	public void decreaseCurrentSet ()
	{
		/*
		 * 	This is equivalent to subtracting 1 modulo
		 * 	questionSets.Length
		 * 
		 * 	This is just since modulo of negative numbers
		 * 	does not behave as needed.
		 */
		setCurrentSet ((selectedSet + questionSets.Count - 1) % questionSets.Count);
	}

	public void selectQuestionSet ()
	{
		setCurrentSet (setToChange);
		currentQuestion = 0;
	}

	public QuestionSet getSetToChange ()
	{
		return questionSets [setToChange];
	}

	public void increaseSetToChange ()
	{
		setToChange += 1;
		setToChange %= questionSets.Count;
	}

	public void decreaseSetToChange ()
	{
		//	This is equivalent to subtracting 1 modulo Length
		setToChange += questionSets.Count - 1;
		setToChange %= questionSets.Count;
	}

	public void hideAnswer ()
	{
		hideAnswers ^= true;
	}

	public void increaseCurrentQuestion ()
	{
		currentQuestion += 1;
		currentQuestion %= questionSets [selectedSet].numberOfQuestions ();
	}

	public void decreaseCurrentQuestion ()
	{
		//	This is equivalent to subtracting 1 modulo Length
		currentQuestion += questionSets [selectedSet].numberOfQuestions () - 1;
		currentQuestion %= questionSets [selectedSet].numberOfQuestions ();
	}

	public Question getCurrentQuestion ()
	{
		return questionSets [selectedSet].questions [currentQuestion];
	}

	/*
	 * 	Randomly choose placement of the star block	
	 * 
	 * 	params:
	 * 		bounds: the RectTransform of the play area
	 * 		objects: [Sword, Shield, Star]
	 */
	public void randomlyPlaceStar (RectTransform bounds, GameObject[] objects)
	{
		float maxXDisplacement = bounds.rect.width * bounds.localScale.x / 2 - objects [2].transform.localScale.x / 2;
		float maxYDisplacement = bounds.rect.height * bounds.localScale.y / 2 - objects [2].transform.localScale.y / 2;

		Vector3 randomPlacement = new Vector3 (bounds.position.x + UnityEngine.Random.Range (-100, 100) * maxXDisplacement / 100,
			                          bounds.position.y + UnityEngine.Random.Range (-100, 100) * maxYDisplacement / 100,
			                          objects [2].transform.position.z);
		objects [2].transform.position = randomPlacement;
	}

	public void increaseRoomNumber ()
	{
		if (matchMaker.roomList != null && matchMaker.roomList.Count > 0) {
			roomNumber += 1;
			roomNumber %= matchMaker.roomList.Count;
		} else {
			roomNumber = 0;
		}
	}

}

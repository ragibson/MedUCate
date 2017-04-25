using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriviaRound
{
	public float roundTime;

	public Question currentQuestion;

	public string[] answers;
	private string correctAnswer;

	public float answerTime;
	public bool correct = false;

	public int round;
	private float currentTime = 0;

	public bool proceedToCombat = false;

	GameLogicManager gameLogic;

	public TriviaRound (float secondsPerRound, Question question, string[] responses, string correctResponse, int roundNumber)
	{
		gameLogic = GameObject.Find ("GameLogicManager").GetComponent<GameLogicManager> ();

		roundTime = secondsPerRound;
		currentQuestion = question;
		answers = responses;
		correctAnswer = correctResponse;
		round = roundNumber;

		/*
		 * 	answerTime == roundTime means the player took the entire round to respond
		 * 	I.e. the player didn't answer in time
		 */
		answerTime = roundTime;
	}

	bool addedExtraTime = false;

	public void Update ()
	{
		if (gameLogic.currentlyNetworking ()) {
			gameLogic.computer.networkedUpdateTimeAndAnswer (gameLogic.networkedTheirAnswerTime (), gameLogic.networkedTheirAnswerCorrect ());
		}

		bool slowAnswer = (answerTime != roundTime && answerTime >= (roundTime - 3));
		bool noAnswer = (answerTime == roundTime);

		// If networked, also consider the opponent's answer time
		if (gameLogic.currentlyNetworking ()) {
			slowAnswer |= (gameLogic.computer.answerTime != roundTime && gameLogic.computer.answerTime >= (roundTime - 3));
			noAnswer |= (gameLogic.computer.answerTime == roundTime);
		}

		currentTime += Time.deltaTime;
		if (timeRemaining () < 3 && slowAnswer && !addedExtraTime) {
			/*
			 * 	If the player answers with less than three seconds to go, we give 
			 * 	them some extra time to read the results of the round
			 */
			addedExtraTime = true;
			currentTime = roundTime - 3;
		} else if (timeRemaining () < 0 && noAnswer && !addedExtraTime) {
			/*
			 * 	If the player does not answer at all, we also give them some extra
			 * 	time to read the results of the round
			 */
			addedExtraTime = true;
			currentTime = roundTime - 3;
		} else if (timeRemaining () < 0) {
			proceedToCombat = true;
		}
	}

	public bool answered ()
	{
		return answerTime < roundTime || addedExtraTime;
	}

	public void answerChosen (int i)
	{
		// If the player hasn't answered yet, we let them
		if (!answered ()) {
			answerTime = Mathf.Round (100 * currentTime) / 100;
			correct = (string.Compare (answers [i], correctAnswer) == 0);
		}
	}

	public string displayText (ComputerPlayer computer)
	{
		if (answered () || roundOver ()) {
			return textAfterAnswer (computer);
		}
		return textBeforeAnswer ();
	}

	public string textBeforeAnswer ()
	{
		return string.Format ("ROUND {0}\n\n{1}", round, currentQuestion.questionText);
	}

	public string textAfterAnswer (ComputerPlayer computer)
	{
		string action = "DEFEND";
		string choice = "WRONG!";
		if (playerAttacks (computer)) {
			action = "ATTACK";
		}
		if (correct) {
			choice = "CORRECT!";
		}

		string computerChoice = "INCORRECTLY";
		float computerTime = Mathf.Round (100 * computer.answerTime) / 100;
		if (computer.correctAnswer) {
			computerChoice = "CORRECTLY";
		}

		string actionText = string.Format ("YOU GET TO {0} THIS ROUND!", action);
		if (skipCombat (computer)) {
			actionText = "COMBAT WILL BE SKIPPED!";
		}

		string playerAnswerTimeText = string.Format ("YOUR CHOICE WAS {0}\n" +
		                              "YOU ANSWERED IN {1} SECONDS\n\n", choice, answerTime);
		string computerAnswerTimeText = string.Format ("YOUR OPPONENT ANSWERED {0}\n" +
		                                "IN {1} SECONDS\n\n", computerChoice, computerTime);

		if (gameLogic.currentlyNetworking ()) {
			if (computer.answerTime == roundTime) {
				if (!addedExtraTime) {
					computerAnswerTimeText = "YOUR OPPONENT HAS NOT ANSWERED YET\n\n";
					actionText = "";
				} else {
					computerAnswerTimeText = "YOUR OPPONENT DID NOT ANSWER\n\n";
				}
			}
		} else {
			if (computerTime == roundTime) {
				computerAnswerTimeText = "YOUR OPPONENT DID NOT ANSWER\n\n";
			}
		}

		if (answerTime == roundTime) {
			playerAnswerTimeText = "YOU DID NOT ANSWER\n\n";
		}

		return string.Format ("THE ANSWER WAS\n\n" +
		"{0}\n\n" +
		playerAnswerTimeText +
		computerAnswerTimeText +
		actionText, correctAnswer);
	}

	public bool roundOver ()
	{
		return currentTime >= roundTime;
	}

	public float timeRemaining ()
	{
		return roundTime - currentTime;
	}

	public bool skipCombat (ComputerPlayer computer)
	{
		return !correct && !computer.correctAnswer;
	}

	/*
	 * 	Determines if the player is attacking
	 * 
	 * 	Assumes that we are not skipping combat
	 */
	public bool playerAttacks (ComputerPlayer computer)
	{
		if (correct && computer.correctAnswer) {
			return answerTime < computer.answerTime;
		} else {
			return correct;
		}
	}
}

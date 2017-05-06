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

	/*
	 * 	If the player does not answer at all or answers with less
	 * 	than 3 seconds left, we give them some extra time to read
	 * 	the results of the round
	 */
	public bool addedExtraTime = false;

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

	public void Update ()
	{
		/*
		 *	Update opponent's answer and time when playing multiplayer
		 *
		 *	We essentially consider the other player to be a "computer"
		 *	so that the same game logic code will work in both cases.
		 */
		if (gameLogic.currentlyNetworking ()) {
			gameLogic.computer.networkedUpdateTimeAndAnswer (gameLogic.networkedTheirAnswerTime (),
				gameLogic.networkedTheirAnswerCorrect ());
		}

		/*
		 * 	Determine whether we were slow to answer (less than 3 seconds left)
		 *  or didn't answer the current question.
		 */
		bool slowAnswer = (answerTime != roundTime && answerTime >= (roundTime - 3));
		bool noAnswer = (answerTime == roundTime);

		// Also consider the opponent's answer time.
		slowAnswer |= (gameLogic.computer.answerTime != roundTime &&
		gameLogic.computer.answerTime >= (roundTime - 3));
		noAnswer |= (gameLogic.computer.answerTime == roundTime);

		currentTime += Time.deltaTime;
		if (timeRemaining () < 0 && (slowAnswer || noAnswer) && !addedExtraTime) {
			/*
			 * 	If the player does not answer at all or answers with less
			 * 	than 3 seconds left, we give them some extra time to read
			 * 	the results of the round
			 */
			addedExtraTime = true;
			currentTime = roundTime - 3;
		} else if (timeRemaining () < 0) {
			proceedToCombat = true;
		}
	}

	// returns true if we've answered the question or ran out of time
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

			/*
			 * 	Wait to display the computer player's answer time until it would
			 * 	have actually been given (to be consistent with multiplayer games)
			 */
			if (!addedExtraTime && computerTime > currentTime) {
				computerAnswerTimeText = "YOUR OPPONENT HAS NOT ANSWERED YET\n\n";
				actionText = "";
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

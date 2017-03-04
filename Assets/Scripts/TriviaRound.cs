using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriviaRound
{
	/*
	 * 	TODO: If neither player answers correctly, skip combat
	 */

	public float roundTime;

	private Question currentQuestion;

	public string[] answers;
	private string correctAnswer;

	private float answerTime;
	public bool correct = false;

	private int round;
	private float currentTime = 0;

	public bool proceedToCombat = false;

	public TriviaRound (float secondsPerRound, Question question, string[] responses, string correctResponse, int roundNumber)
	{
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
		currentTime += Time.deltaTime;
		if (timeRemaining () < 0) {
			proceedToCombat = true;
		}
	}

	public bool answered ()
	{
		return answerTime < roundTime;
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

		return string.Format ("THE ANSWER WAS\n\n" +
		"{0}\n\n" +
		"YOUR CHOICE WAS {1}\n" +
		"YOU ANSWERED IN {2} SECONDS\n\n" +
		"YOUR OPPONENT ANSWERED {3}\n" +
		"IN {4} SECONDS\n\n" +
		actionText, correctAnswer, choice, answerTime, computerChoice, computerTime, action);
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

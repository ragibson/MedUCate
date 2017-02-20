using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriviaRound
{
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
			answerTime = currentTime;
			correct = (string.Compare (answers [i], correctAnswer) == 0);
		}
	}

	public string displayText ()
	{
		if (answered () || roundOver ()) {
			return textAfterAnswer ();
		}
		return textBeforeAnswer ();
	}

	public string textBeforeAnswer ()
	{
		return string.Format ("ROUND {0}\n\n{1}", round, currentQuestion.questionText);
	}

	public string textAfterAnswer ()
	{
		string action = "DEFEND";
		string choice = "WRONG!";
		if (correct) {
			action = "ATTACK";
			choice = "CORRECT!";
		}

		return string.Format ("THE ANSWER WAS\n\n" +
		"{0}\n\n" +
		"YOUR CHOICE WAS {1}\n\n" +
		"YOU GET TO {2} THIS ROUND!", correctAnswer, choice, action);
	}

	public bool roundOver ()
	{
		return currentTime >= roundTime;
	}

	public float timeRemaining ()
	{
		return roundTime - currentTime;
	}
}

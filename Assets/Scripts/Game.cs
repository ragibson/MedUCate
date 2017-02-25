using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game
{
	public float startingHealth;
	public float roundTime;
	public float damage;
	public float playerHealth;
	public float enemyHealth;

	public Game (float HP, float secondsPerRound, float defaultDamage)
	{
		startingHealth = HP;
		roundTime = secondsPerRound;
		damage = defaultDamage;
		playerHealth = startingHealth;
		enemyHealth = startingHealth;
	}

	private GameLogicManager gameLogic = GameObject.Find ("GameLogicManager").GetComponent<GameLogicManager> ();

	// ===   TRIVIA ROUND   === //

	public TriviaRound currentTriviaRound;

	private string[] answers;

	private int round = 1;

	public void nextRound ()
	{
		QuestionSet currentSet = gameLogic.settings.selected;
		Question currentQuestion = currentSet.questions [Random.Range (0, currentSet.numberOfQuestions () - 1)];
		answers = currentQuestion.shuffledAnswers ();

		gameLogic.computer.updateTimeAndAnswer ();

		currentTriviaRound = new TriviaRound (roundTime, currentQuestion, answers, currentQuestion.correctAnswer, round);
		round += 1;
	}

	public string textToDisplay ()
	{
		return currentTriviaRound.displayText (gameLogic.computer);
	}

	public void answerClicked (int i)
	{
		currentTriviaRound.answerChosen (i);
	}

	// ===   COMBAT ROUND   === //

	public CombatRound currentCombatRound;

	public void nextCombat ()
	{
		currentCombatRound = new CombatRound (roundTime, damage, playerHealth, enemyHealth);
	}

	public void dealDamage (int player, int enemy)
	{
		playerHealth -= player;
		enemyHealth -= enemy;
		playerHealth = Mathf.Max (playerHealth, 0);
		enemyHealth = Mathf.Max (enemyHealth, 0);
	}
}

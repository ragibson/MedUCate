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

	public Queue<Question> questionsToAsk = new Queue<Question>();

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

	public void fillQuestionQueue(QuestionSet currentSet) {
		List<int> questionIds = new List<int> ();
		for (int i = 0; i < currentSet.numberOfQuestions(); i++) {
			questionIds.Add (i);
		}

		for (int i = 0; i < questionIds.Count; i++) {
			int temp = questionIds[i];
			int randomIndex = Random.Range(i, questionIds.Count);
			questionIds[i] = questionIds[randomIndex];
			questionIds[randomIndex] = temp;
		}

		foreach (int i in questionIds) {
			questionsToAsk.Enqueue (currentSet.questions [i]);
		}
	}

	public void nextRound ()
	{
		QuestionSet currentSet = gameLogic.settings.selected;
		if (questionsToAsk.Count == 0) {
			fillQuestionQueue (currentSet);
		}
		Question currentQuestion = questionsToAsk.Dequeue ();
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

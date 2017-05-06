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

	public Queue<Question> questionsToAsk = new Queue<Question> ();

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

	/*
	 * 	We use a question queue so that questions do not repeat
	 * 	until the entire question set has been exhausted.
	 */
	public void fillQuestionQueue (QuestionSet currentSet)
	{
		List<int> questionIds = new List<int> ();
		for (int i = 0; i < currentSet.numberOfQuestions (); i++) {
			questionIds.Add (i);
		}

		// Shuffle the questions
		for (int i = 0; i < questionIds.Count; i++) {
			int temp = questionIds [i];
			int randomIndex = Random.Range (i, questionIds.Count);
			questionIds [i] = questionIds [randomIndex];
			questionIds [randomIndex] = temp;
		}

		foreach (int i in questionIds) {
			questionsToAsk.Enqueue (currentSet.questions [i]);
		}
	}

	/*
	 * 	Sets up the next round.
	 * 
	 * 	If playing multiplayer, this is only run by the server.
	 */
	public void nextRound ()
	{
		QuestionSet currentSet = gameLogic.settings.selected;
		if (questionsToAsk.Count == 0) {
			fillQuestionQueue (currentSet);
		}
		Question currentQuestion = questionsToAsk.Dequeue ();
		answers = currentQuestion.shuffledAnswers ();

		// If not networked, use computer player
		if (!gameLogic.currentlyNetworking ()) {
			gameLogic.computer.updateTimeAndAnswer ();
		}

		currentTriviaRound = new TriviaRound (roundTime, currentQuestion, answers, currentQuestion.correctAnswer, round);
		round += 1;
	}

	/*
	 * 	If playing multiplayer, this is run by the client
	 * 	and uses the server's question/answer values.
	 * 
	 * 	Called in continueGame in UIManager
	 */
	public void networkedNextRound (string q, string a1, string a2, string a3, string a4)
	{
		Question currentQuestion = new Question (q, a1, a2, a3, a4);
		string[] answers = currentQuestion.shuffledAnswers ();

		resetNetworkedAnswers ();

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

		if (gameLogic.currentlyNetworking ()) {
			if (gameLogic.isServer) {
				gameLogic.ourGamestate.RpcUpdateServerAnswerCorrect (currentTriviaRound.correct);
				gameLogic.ourGamestate.RpcUpdateServerAnswerTime (currentTriviaRound.answerTime);
			} else {
				gameLogic.ourGamestate.CmdUpdateClientAnswerCorrect (currentTriviaRound.correct);
				gameLogic.ourGamestate.CmdUpdateClientAnswerTime (currentTriviaRound.answerTime);
			}
		}
	}

	public void resetNetworkedAnswers ()
	{
		if (gameLogic.currentlyNetworking ()) {
			if (gameLogic.isServer) {
				gameLogic.ourGamestate.RpcUpdateServerAnswerCorrect (false);
				gameLogic.ourGamestate.RpcUpdateServerAnswerTime (gameLogic.secondsPerRound);
			} else {
				gameLogic.ourGamestate.CmdUpdateClientAnswerCorrect (false);
				gameLogic.ourGamestate.CmdUpdateClientAnswerTime (gameLogic.secondsPerRound);
			}
		}
	}

	// ===   COMBAT ROUND   === //

	public CombatRound currentCombatRound;

	public void nextCombat ()
	{
		currentCombatRound = new CombatRound (roundTime / 2, damage, playerHealth, enemyHealth);
	}

	public void dealDamage (int player, int enemy)
	{
		playerHealth -= player;
		enemyHealth -= enemy;
		playerHealth = Mathf.Max (playerHealth, 0);
		enemyHealth = Mathf.Max (enemyHealth, 0);
	}
}

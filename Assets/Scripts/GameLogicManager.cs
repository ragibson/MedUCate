using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogicManager : MonoBehaviour
{

	/*
	 * 	TODO: Remove hardcoded Question Sets
	 *
	 * 	TODO: Maintain settings past exiting app
	 */

	/*
	 *	In initial prototype, these were set to 200, 10, and 25, respectively.
	 *
	 *	Should consider shortening game length by changing these.
	 */
	public int gameHP;
	public int secondsPerRound;
	public int damagePerAttack;

	public bool completedTutorial;
	public int reputation;

	public string displayText = "";

	public Settings settings = new Settings ();
	public ComputerPlayer computer = new ComputerPlayer ();
	public QuestionSet[] questionSets;
	int selectedSet = 0;

	// === Profile Change Questions === //
	public int setToChange = 0;

	// === Profile Review Questions === //
	public int currentQuestion = 0;
	public bool hideAnswers = false;

	// ===   Campaign High Scores   === //
	public string gameMode = "";
	public int[] campaignScores;

	// Initialization
	void Start ()
	{
		// TODO: Remove hardcoded Question Sets
		questionSets = prototypeQuestionSets ();
		setCurrentSet (0);

		// TODO: Get these from a settings file
		campaignScores = new int[9];
		for (int i = 0; i < 9; i++) {
			campaignScores [i] = 0;
		}

		reputation = 0;

		completedTutorial = false;
	}

	// ===   THE FOLLOWING METHODS ARE ALL SETTING   === //
	// === MANIPULATION FROM THE VARIOUS GAME MENUS  === //

	void setCurrentSet (int i)
	{
		questionSets [selectedSet].selected = false;
		selectedSet = i;
		settings.selected = questionSets [selectedSet];
		questionSets [selectedSet].selected = true;
	}

	public void increaseCurrentSet ()
	{
		setCurrentSet ((selectedSet + 1) % questionSets.Length);
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
		setCurrentSet ((selectedSet + questionSets.Length - 1) % questionSets.Length);
	}

	public void selectQuestionSet ()
	{
		setCurrentSet (setToChange);
	}

	public QuestionSet getSetToChange ()
	{
		return questionSets [setToChange];
	}

	public void increaseSetToChange ()
	{
		setToChange += 1;
		setToChange %= questionSets.Length;
	}

	public void decreaseSetToChange ()
	{
		/*
		 * 	This is equivalent to subtracting 1 modulo
		 * 	questionSets.Length
		 * 
		 * 	This is just since modulo of negative numbers
		 * 	does not behave as needed.
		 */
		setToChange += questionSets.Length - 1;
		setToChange %= questionSets.Length;
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
		/*
		 * 	This is equivalent to subtracting 1 modulo
		 * 	questionSets [selectedSet].numberOfQuestions()
		 * 
		 * 	This is just since modulo of negative numbers
		 * 	does not behave as needed.
		 */
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

	public string getCampaignScore (int level)
	{
		if (campaignScores [level] == 0) {
			return "NOT YET COMPLETED";
		}
		return String.Format ("HIGH SCORE: {0}/200", campaignScores [level]);
	}

	// Returns whether player got a high score
	public bool updateCampaign (float finalHP)
	{
		if (String.Equals (this.gameMode, "Campaign")) {
			int score = (int)Mathf.Round (finalHP);

			// If the player gets a new high score, we update their scores and reputation
			if (score > this.campaignScores [computer.level]) {
				this.campaignScores [computer.level] = score;
				return true;
			}

		}
		return false;
	}

	/*
	 * 	Returns string to display after game ends.
	 * 	This includes change in reputation and campaign high score notification.
	 * 
	 * 	Tutorial gives 500 reputation on first completion.
	 * 
	 * 	Campaign gives 20-100 reputation on new high score, depending on level.
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
		}

		displayText += "Reputation ";
		if (reputationChange >= 0) {
			displayText += "+";
		}
		displayText += "" + reputationChange;
		reputation += reputationChange;

		displayText += String.Format ("\n\n Current Reputation: {0}", reputation);

		this.gameMode = "";
		return displayText;
	}

	// ===== HARD CODED QUESTION SETS FROM PROTOTYPE ===== //
	// =====      TODO: REPLACE WITH XML FILES       ===== //

	QuestionSet[] prototypeQuestionSets ()
	{
		QuestionSet[] hardcoded = new QuestionSet[4];

		hardcoded [0] = new QuestionSet (new Question[] {
			new Question ("1 + 1?", "2", "1", "12", "2017"),
			new Question ("2 + 2?", "4", "1", "12", "2017"),
			new Question ("3 + 3?", "6", "1", "12", "2017"),
			new Question ("4 + 4?", "8", "1", "12", "2017")
		}, "Default Mental Health Set", "MedUCate LLC", "0000001");
		hardcoded [1] = new QuestionSet (new Question[] {
			new Question ("10 + 10?", "20", "1", "12", "2017"),
			new Question ("20 + 20?", "40", "1", "12", "2017"),
			new Question ("30 + 30?", "60", "1", "12", "2017"),
			new Question ("40 + 40?", "80", "1", "12", "2017")
		}, "Default Physical Health Set", "MedUCate LLC", "0000002");
		hardcoded [2] = new QuestionSet (new Question[] {
			new Question ("100 + 100?", "200", "1", "12", "2017"),
			new Question ("200 + 200?", "400", "1", "12", "2017"),
			new Question ("300 + 300?", "600", "1", "12", "2017"),
			new Question ("400 + 400?", "800", "1", "12", "2017")
		}, "Default Social Health Set", "MedUCate LLC", "0000003");
		hardcoded [3] = new QuestionSet (new Question[] {
			new Question ("1000 + 1000?", "2000", "1", "12", "2017"),
			new Question ("2000 + 2000?", "4000", "1", "12", "2017"),
			new Question ("3000 + 3000?", "6000", "1", "12", "2017"),
			new Question ("4000 + 4000?", "8000", "1", "12", "2017")
		}, "Default Nutritional Health Set", "MedUCate LLC", "0000004");

		return hardcoded;
	}
}

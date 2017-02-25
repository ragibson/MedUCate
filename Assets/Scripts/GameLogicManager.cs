using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogicManager : MonoBehaviour
{

	/*
	 * 	TODO: Remove hardcoded Question Sets
	 *
	 * 	TODO: Implement Campaign Level Completion Triggers
	 */

	public Settings settings = new Settings ();
	public ComputerPlayer computer = new ComputerPlayer ();
	public QuestionSet[] questionSets;
	int selectedSet = 0;

	// === Profile Change Questions === //
	public int setToChange = 0;

	// === Profile Review Questions === //
	public int currentQuestion = 0;
	public bool hideAnswers = false;

	// Initialization
	void Start ()
	{
		// TODO: Remove hardcoded Question Sets
		questionSets = prototypeQuestionSets ();
		setCurrentSet (0);
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

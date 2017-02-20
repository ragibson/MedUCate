using UnityEngine;
using System.Collections;

public class QuestionSet
{

	/* 
	 * 	TODO: Remove/replace activationCode once we
	 * 	determine what this is meant to do.
	 * 
	 * 	TODO: Implement expiryTime checks once
	 * 	we determine what this is meant to do.
	 * 	
	 * 	Is expiryTime meant to delete outdated
	 * 	QuestionSets, or just disable them, etc.?
	 */

	public Question[] questions;
	public bool selected;
	public string setName;
	public string authorName;

	/*
	 * 	I have no clue what this is meant to do.
	 * 	
	 * 	The text in the profile menu indicates
	 * 	that this is supposed to be used to
	 * 	fetch a question set by ID from some
	 * 	server.
	 * 
	 * 	This should not be implemented in this
	 * 	manner, but will remain here for the sake
	 * 	of matching the original prototype until
	 * 	we replace/remove it.
	 */
	public string activationCode;

	/*
	 * 	Presumably, this is a timeout condition
	 * 	for online/sponsored question sets.
	 * 
	 * 	It's always been set to "Never" in the
	 * 	prototypes, but it probably takes the
	 * 	form YY:MM:DD?
	 */
	public string expiryTime;

	/*
	 * 	params:
	 * 		questionList: Question[]
	 */
	public QuestionSet (Question[] questionList, string name, string author, string ID)
	{
		questions = questionList;
		selected = false;
		setName = name;
		authorName = author;

		/* 
		 * 	TODO: Remove/replace activationCode once we
		 * 	determine what this is meant to do.
		 * 
		 * 	TODO: Implement expiryTime checks once
		 * 	we determine what this is meant to do.
		 * 	
		 * 	Is expiryTime meant to delete outdated
		 * 	QuestionSets, or just disable them, etc.?
		 */
		activationCode = ID;
		expiryTime = "Never";
	}

	public int numberOfQuestions ()
	{
		return questions.Length;
	}
}
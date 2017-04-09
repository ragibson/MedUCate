using UnityEngine;
using System.Collections;

public class QuestionSet
{

	public Question[] questions;
	public bool selected;
	public string setName;
	public string authorName;

	/*
	 * 	params:
	 * 		questionList: Question[]
	 * 		name: string containing the name of the QuestionSet
	 * 		author: the person/user that created the QuestionSet
	 */
	public QuestionSet (Question[] questionList, string name, string author)
	{
		questions = questionList;
		selected = false;
		setName = name;
		authorName = author;
	}

	public int numberOfQuestions ()
	{
		return questions.Length;
	}
}
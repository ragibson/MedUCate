using UnityEngine;
using System.Collections;

public class Question
{

	public string questionText;
	public string correctAnswer;
	public string[] incorrectAnswers;

	/*
	 * 	params:
	 * 		q: Text of question
	 * 		a1: Correct Answer
	 * 		a2,a3,a4: Incorrect Answers
	 */
	public Question (string q, string a1, string a2, string a3, string a4)
	{
		questionText = q;
		correctAnswer = a1;
		incorrectAnswers = new string[]{ a2, a3, a4 };
	}

	/*
	 * 	Returns a shuffled array of answers
	 */
	public string[] shuffledAnswers ()
	{
		string[] arr = new string[4];
		arr [0] = correctAnswer;
		incorrectAnswers.CopyTo (arr, 1);
		for (int i = arr.Length - 1; i > 0; i--) {
			int r = Random.Range (0, i);
			string tmp = arr [i];
			arr [i] = arr [r];
			arr [r] = tmp;
		}
		return arr;
	}
}

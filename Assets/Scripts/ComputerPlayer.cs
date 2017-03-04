﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerPlayer
{
	/*
	 * 	Computer AI is as follows:
	 * 		When moving Sword:
	 * 			50% of the time:
	 * 				Place Sword within 20% of Star
	 * 			50% of the time:
	 * 				Place Sword randomly
	 * 		When moving Shield:
	 * 			Place Shield within 20% of Star
	 * 
	 * 	Computer Difficulty:
	 * 		Easy:	Correct 50% of the time
	 * 		Medium:	Correct 75% of the time
	 * 		Hard:	Correct 90% of the time
	 * 	
	 * 	Computer Speed vs Average Answer Time:
	 * 		Slow:		8 seconds
	 * 		Moderate:	5 seconds
	 * 		Fast:		2 seconds
	 * 
	 * 	All difficulties have random answer times
	 * 	within 1 second of average.
	 */

	// === Singleplayer Quick Play === //
	public int difficulty = 0;
	public string[] difficulties = new string[] { "Easy", "Medium", "Hard" };
	public int speed = 0;
	public string[] speeds = new string[] { "Slow", "Moderate", "Fast" };

	// ===  Singleplayer Campaign  === //
	/*
	 * 	The level of the computer player here
	 * 	is just a mix of the difficulty and
	 * 	speed from above.
	 * 
	 * 	E.g.
	 * 		"Level: Medium/Fast"
	 * 		from the original prototype
	 * 		is already accounted for
	 */
	public int level = 0;

	public bool correctAnswer = false;
	public float answerTime = 0;

	// ===   THE FOLLOWING METHODS ARE ALL SETTING   === //
	// === MANIPULATION FROM THE VARIOUS GAME MENUS  === //

	public void changeDifficulty ()
	{
		difficulty += 1;
		difficulty %= difficulties.Length;
	}

	public void changeSpeed ()
	{
		speed += 1;
		speed %= speeds.Length;
	}

	public void increaseLevel ()
	{
		level += 1;
		level %= difficulties.Length * speeds.Length;
	}

	public void decreaseLevel ()
	{
		/*
		 * 	This is equivalent to subtracting 1 modulo
		 * 	difficulties.Length * speeds.Length
		 * 
		 * 	This is just since modulo of negative numbers
		 * 	does not behave as needed.
		 */
		level += difficulties.Length * speeds.Length - 1;
		level %= difficulties.Length * speeds.Length;
	}

	public string getDifficultyString ()
	{
		return difficulties [difficulty];
	}

	public string getSpeedString ()
	{
		return speeds [speed];
	}

	public string getLevelString ()
	{
		difficulty = level / difficulties.Length;
		speed = level % speeds.Length;
		return difficulties [difficulty] + "/" + speeds [speed];
	}

	public int numberOfLevels ()
	{
		return difficulties.Length * speeds.Length;
	}

	/*
	 * 	Calculates the ComputerPlayer's block movement
	 * 
	 * 	If the ComputerPlayer is moving the shield, it chooses
	 * 	to place the shield overlapping the star (currently,
	 * 	randomly chooses the center to be within 20% of the
	 * 	star block).
	 * 
	 * 	If the ComputerPlayer is moving the sword, 50% of the time,
	 * 	it chooses to place the sword overlapping the star (currently,
	 * 	randomly chooses the center to be within 20%
	 * 	of the star block).
	 * 	The other 50% of the time, the sword placement is completely
	 * 	random within the play area.
	 * 
	 * 	params:
	 * 		wonTrivia:	true if the ComputerPlayer lost the trivia round
	 * 					false otherwise
	 * 		objects: [Sword, Shield, Star]
	 * 		bounds: The RectTransform of the play area
	 */
	public void placeBlock (bool lostTrivia, RectTransform bounds, GameObject[] objects)
	{
		/*
		 * 	This assumes that all the blocks have the same width/height
		 * 	At this point, the client has emphasized this requirement
		 */
		float maxXDisplacement = bounds.rect.width * bounds.localScale.x / 2;
		float maxYDisplacement = bounds.rect.height * bounds.localScale.y / 2;

		// Within 20% of the star center
		Vector3 starOverlapPosition = new Vector3 (objects [2].transform.position.x + UnityEngine.Random.Range (-20, 20) * maxXDisplacement / 100,
			                              objects [2].transform.position.y + UnityEngine.Random.Range (-20, 20) * maxYDisplacement / 100, 0);
		// Random placement in the play area
		Vector3 randomPlacement = new Vector3 (bounds.position.x + UnityEngine.Random.Range (-100, 100) * maxXDisplacement / 100,
			                          bounds.position.y + UnityEngine.Random.Range (-100, 100) * maxYDisplacement / 100,
			                          objects [0].transform.position.z);

		if (lostTrivia) { 	// Placing the shield
			objects [1].transform.position = new Vector3 (starOverlapPosition [0], starOverlapPosition [1], objects [1].transform.position.z);
		} else { 			// Placing the sword
			bool overlappingStar = (UnityEngine.Random.Range (0, 2) == 0);
			if (overlappingStar) {
				objects [0].transform.position = new Vector3 (starOverlapPosition [0], starOverlapPosition [1], objects [0].transform.position.z);
			} else {
				objects [0].transform.position = randomPlacement;
			}
		}

		// Make sure no placement has moved blocks outside the play area
		objects [0].GetComponentInChildren<Draggable> ().forceInBounds ();
		objects [1].GetComponentInChildren<Draggable> ().forceInBounds ();
	}

	/*
	 * 	Places the sword within 50% of the star block for the tutorial
	 */
	public void placeBlockTutorial (RectTransform bounds, GameObject[] objects)
	{
		/*
		 * 	This assumes that all the blocks have the same width/height
		 * 	At this point, the client has emphasized this requirement
		 */
		float maxXDisplacement = bounds.rect.width * bounds.localScale.x / 2;
		float maxYDisplacement = bounds.rect.height * bounds.localScale.y / 2;

		// Within 50% of the star center
		Vector3 starOverlapPosition = new Vector3 (objects [2].transform.position.x + UnityEngine.Random.Range (-50, 50) * maxXDisplacement / 100,
			objects [2].transform.position.y + UnityEngine.Random.Range (-50, 50) * maxYDisplacement / 100, 0);

		objects [0].transform.position = new Vector3 (starOverlapPosition [0], starOverlapPosition [1], objects [0].transform.position.z);

		// Make sure no placement has moved blocks outside the play area
		objects [0].GetComponentInChildren<Draggable> ().forceInBounds ();
	}

	/*
	 * 	Computer Difficulty:
	 * 		Easy:	Correct 50% of the time
	 * 		Medium:	Correct 75% of the time
	 * 		Hard:	Correct 90% of the time
	 */
	bool answeredCorrectly ()
	{
		int randVal = UnityEngine.Random.Range (0, 100);
		if (difficulty == 0) { 			// Easy
			return randVal <= 50;
		} else if (difficulty == 1) { 	// Medium
			return randVal <= 75;
		} else { 						// Hard
			return randVal <= 90;
		}
	}

	/* 	
	 * 	Computer Speed vs Average Answer Time:
	 * 		Slow:		8 seconds
	 * 		Moderate:	5 seconds
	 * 		Fast:		2 seconds
	 * 
	 * 	All difficulties have random answer times
	 * 	within 1 second of average.
	 */
	float timeToAnswer ()
	{
		float timeVariance = UnityEngine.Random.Range (-100, 101) / 100.0f;
		if (speed == 0) {			// Slow
			return 8.0f + timeVariance;
		} else if (speed == 1) {	// Moderate
			return 5.0f + timeVariance;
		} else {					// Fast
			return 2.0f + timeVariance;
		}
	}

	public void updateTimeAndAnswer ()
	{
		correctAnswer = answeredCorrectly ();
		answerTime = timeToAnswer ();
	}
}
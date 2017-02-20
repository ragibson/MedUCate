using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerPlayer
{
	/*
	 * 	TODO: Implement AI
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
		int levelDifficulty = level / difficulties.Length;
		int levelSpeed = level % speeds.Length;
		return difficulties [levelDifficulty] + "/" + speeds [levelSpeed];
	}

	public int numberOfLevels ()
	{
		return difficulties.Length * speeds.Length;
	}
}
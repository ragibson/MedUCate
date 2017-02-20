using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{

	public QuestionSet selected;

	// === Singleplayer Training === //
	/*
	 * 	These shouldn't be strings, but this
	 * 	is consistent with the prototype
	 */
	public int round = 0;
	public string[] rounds = new string[] { "Five", "Ten", "Twenty" };
	public int trainingMode = 0;
	public string[] trainingModes = new string[] {"YOUR QUESTIONS IN ORDER",
		"YOUR QUESTIONS IN ANY ORDER",
		"PRACTICE THE DAILY SPONSORED QUESTIONS"
	};

	// === Multiplayer Quick Play === //
	public int multiplayerMode = 0;
	public string[] multiplayerModes = new string[] { "CHOSEN - USE YOUR (OR THEIR) QUESTIONS",
		"SPONSORED - USE THE DAILY SPONSORED QUESTIONS",
		"RANDOM - USE ANY QUESTIONS"
	};

	// === Multiplayer One Man Army === //
	public int wager = 0;
	public int[] wagers = new int[] { 1, 2, 5, 10, 20, 50, 100, 200, 500 };

	// ===   THE FOLLOWING METHODS ARE ALL SETTING  === //
	// === MANIPULATION FROM THE VARIOUS GAME MENUS === //

	public void changeRounds ()
	{
		round += 1;
		round %= rounds.Length;
	}

	public void changeTrainingType ()
	{
		trainingMode += 1;
		trainingMode %= trainingModes.Length;
	}

	public void changeMultiPlayerType ()
	{
		multiplayerMode += 1;
		multiplayerMode %= trainingModes.Length;
	}

	public void changeWager ()
	{
		wager += 1;
		wager %= wagers.Length;
	}

	public string getLengthString ()
	{
		return rounds [round];
	}

	public string getTrainingModeString ()
	{
		return trainingModes [trainingMode];
	}

	public string getMultiPlayerModeString ()
	{
		return multiplayerModes [multiplayerMode];
	}

	public string getWagerString ()
	{
		return "WIN/LOSE " + wagers [wager] + " REP";
	}
}

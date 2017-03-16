using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{

	public QuestionSet selected;

	// === Multiplayer Quick Play === //
	public int multiplayerMode = 0;
	public string[] multiplayerModes = new string[] { "CHOSEN - USE YOUR (OR THEIR) QUESTIONS",
		"SPONSORED - USE THE DAILY SPONSORED QUESTIONS",
		"RANDOM - USE ANY QUESTIONS"
	};

	// === Multiplayer One Man Army === //
	public int wagerIndex = 0;
	public int[] wagers = new int[] { 1, 2, 5, 10, 20, 50, 100, 200, 500 };

	// ===   THE FOLLOWING METHODS ARE ALL SETTING  === //
	// === MANIPULATION FROM THE VARIOUS GAME MENUS === //

	public void changeMultiPlayerType ()
	{
		multiplayerMode += 1;
		multiplayerMode %= multiplayerModes.Length;
	}

	public void changeWager ()
	{
		wagerIndex += 1;
		wagerIndex %= wagers.Length;
	}

	public int getWager ()
	{
		return wagers [wagerIndex];
	}

	public string getMultiPlayerModeString ()
	{
		return multiplayerModes [multiplayerMode];
	}

	public string getWagerString ()
	{
		return "WIN/LOSE " + wagers [wagerIndex] + " REP";
	}
}

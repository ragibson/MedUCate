using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{

	public QuestionSet selected;

	// === Multiplayer One Man Army === //
	public int wagerIndex = 0;
	public int[] wagers = new int[] { 1, 2, 5, 10, 20, 50, 100, 200, 500 };

	// ===   THE FOLLOWING METHODS ARE ALL SETTING  === //
	// === MANIPULATION FROM THE VARIOUS GAME MENUS === //

	public void changeWager ()
	{
		wagerIndex += 1;
		wagerIndex %= wagers.Length;
	}

	public int getWager ()
	{
		return wagers [wagerIndex];
	}

	public string getWagerString ()
	{
		return "WIN/LOSE " + wagers [wagerIndex] + " REP";
	}
}

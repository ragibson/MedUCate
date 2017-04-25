using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class MatchMaking : MonoBehaviour
{
	public int maxRooms = 10;
	public bool joiningGame = false;

	NetworkManager manager;
	public List<MatchInfoSnapshot> roomList = null;

	void Start ()
	{
		manager = NetworkManager.singleton;
	}

	public void startMatchMaker ()
	{
		manager.StartMatchMaker ();
	}

	public void endMatchMaker ()
	{
		manager.StopMatchMaker ();
	}

	public void startMatch (string roomName)
	{
		uint roomSize = 2;
		manager.matchMaker.CreateMatch (roomName, roomSize, true, "", "", "", 0, 0, NetworkManager.singleton.OnMatchCreate);
	}

	public void listMatches ()
	{
		manager.matchMaker.ListMatches (0, maxRooms, "", true, 0, 0, OnMatchList);
	}

	public void OnMatchList (bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
	{
		if (matches == null) {
			Debug.Log ("null Match List returned from server");
			return;
		}

		roomList = new List<MatchInfoSnapshot> ();
		roomList.Clear ();
		foreach (MatchInfoSnapshot match in matches) {
			roomList.Add (match);
		}
	}

	public string roomNameToJoin (int i)
	{
		return roomList [i].name;
	}

	public void joinGame (int i)
	{
		if (roomList.Count > i) {
			MatchInfoSnapshot info = roomList [i];
			manager.matchMaker.JoinMatch (info.networkId, "", "", "", 0, 0, manager.OnMatchJoined);
			joiningGame = true;
		}
	}
}

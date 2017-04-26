using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Sync Game States 10 times per second
[NetworkSettings (sendInterval = 0.1f)]
public class NetworkGameState : NetworkBehaviour
{

	public UIManager ui;

	[SyncVar (hook = "OnChangeShield")]
	public Vector3 shieldPos;
	[SyncVar (hook = "OnChangeSword")]
	public Vector3 swordPos;
	[SyncVar (hook = "OnChangeStar")]
	public Vector3 starPos;

	public GameObject shield;
	public GameObject sword;
	public GameObject star;

	[SyncVar]
	public float timeSinceLastSync;

	[SyncVar]
	public bool clientShieldAuthority;
	[SyncVar]
	public bool clientSwordAuthority;
	[SyncVar]
	public bool clientStarAuthority;

	[SyncVar]
	public float serverHealth;
	[SyncVar]
	public float clientHealth;
	[SyncVar]
	public int damageDealt;
	[SyncVar]
	public int damageBlocked;

	[SyncVar]
	public int roundNumber;

	[SyncVar]
	public string questionText;
	[SyncVar]
	public string answer1;
	[SyncVar]
	public string answer2;
	[SyncVar]
	public string answer3;
	[SyncVar]
	public string answer4;
	[SyncVar]
	public string correctAnswer;

	[SyncVar]
	public float clientAnswerTime;
	[SyncVar]
	public float serverAnswerTime;
	[SyncVar]
	public bool clientAnswerCorrect;
	[SyncVar]
	public bool serverAnswerCorrect;

	bool gameStarted;

	GameLogicManager gameLogic;

	// Use this for initialization
	void Start ()
	{
		ui = GameObject.Find ("UI").GetComponent<UIManager> ();

		timeSinceLastSync = 0;

		gameLogic = GameObject.Find ("GameLogicManager").GetComponent<GameLogicManager> ();

		serverHealth = gameLogic.gameHP;
		clientHealth = gameLogic.gameHP;

		if (isLocalPlayer && hasAuthority) {
			gameLogic.isServer = isServer;
			gameLogic.ourGamestate = this;
		}
		if (!isLocalPlayer) {
			gameLogic.theirGamestate = this;
		}

		if (hasAuthority) {
			ui.multiPlayerQuickPlayStartGame ();
			ui.multiPlayerQuickPlayWaitForGame ();
		}

		if (!isServer && hasAuthority) {
			ui.proceedToMultiplayerGame ();
		}

		gameStarted = false;

		clientShieldAuthority = false;
		clientSwordAuthority = false;
		clientStarAuthority = false;

		sword = ui.objects [0];
		shield = ui.objects [1];
		star = ui.objects [2];

		shieldPos = shield.GetComponent<Transform> ().position;
		swordPos = sword.GetComponent<Transform> ().position;
		starPos = star.GetComponent<Transform> ().position;

		clientAnswerTime = gameLogic.secondsPerRound;
		serverAnswerTime = gameLogic.secondsPerRound;
		clientAnswerCorrect = false;
		serverAnswerCorrect = false;
	}

	[ClientRpc]
	public void RpcShieldPosition (Vector3 pos)
	{
		shieldPos = pos;
	}

	[Command]
	public void CmdShieldPosition (Vector3 pos)
	{
		RpcShieldPosition (pos);
	}

	void OnChangeShield (Vector3 pos)
	{
		if (shield != null && !canUpdateShield ()) {
			shield.GetComponent<Transform> ().position = shieldPos;
		}
	}

	bool canUpdateShield ()
	{
		return (clientShieldAuthority && !isServer) || (!clientShieldAuthority && isServer);
	}

	[ClientRpc]
	public void RpcSwordPosition (Vector3 pos)
	{
		swordPos = pos;
	}

	[Command]
	public void CmdSwordPosition (Vector3 pos)
	{
		RpcSwordPosition (pos);
	}

	void OnChangeSword (Vector3 pos)
	{
		if (sword != null && !canUpdateSword ()) {
			sword.GetComponent<Transform> ().position = swordPos;
		}
	}

	bool canUpdateSword ()
	{
		return (clientSwordAuthority && !isServer) || (!clientSwordAuthority && isServer);
	}

	[ClientRpc]
	public void RpcStarPosition (Vector3 pos)
	{
		starPos = pos;
	}

	[Command]
	public void CmdStarPosition (Vector3 pos)
	{
		RpcStarPosition (pos);
	}

	void OnChangeStar (Vector3 pos)
	{
		if (star != null && !canUpdateStar ()) {
			star.GetComponent<Transform> ().position = starPos;
		}
	}

	bool canUpdateStar ()
	{
		return (clientStarAuthority && !isServer) || (!clientStarAuthority && isServer);
	}

	[ClientRpc]
	public void RpcUpdateQuestion (String q)
	{
		questionText = q;
	}

	[ClientRpc]
	public void RpcUpdateAnswer1 (String a1)
	{
		answer1 = a1;
	}

	[ClientRpc]
	public void RpcUpdateAnswer2 (String a2)
	{
		answer2 = a2;
	}

	[ClientRpc]
	public void RpcUpdateAnswer3 (String a3)
	{
		answer3 = a3;
	}

	[ClientRpc]
	public void RpcUpdateAnswer4 (String a4)
	{
		answer4 = a4;
	}

	[ClientRpc]
	public void RpcUpdateCorrectAnswer (String correct)
	{
		correctAnswer = correct;
	}

	[Command]
	public void CmdUpdateClientAnswerCorrect (bool correct)
	{
		RpcUpdateClientAnswerCorrect (correct);
	}

	[ClientRpc]
	public void RpcUpdateClientAnswerCorrect (bool correct)
	{
		clientAnswerCorrect = correct;
	}

	[Command]
	public void CmdUpdateServerAnswerCorrect (bool correct)
	{
		RpcUpdateServerAnswerCorrect (correct);
	}

	[ClientRpc]
	public void RpcUpdateServerAnswerCorrect (bool correct)
	{
		serverAnswerCorrect = correct;
	}

	[Command]
	public void CmdUpdateClientAnswerTime (float time)
	{
		RpcUpdateClientAnswerTime (time);
	}

	[ClientRpc]
	public void RpcUpdateClientAnswerTime (float time)
	{
		clientAnswerTime = time;
	}

	[Command]
	public void CmdUpdateServerAnswerTime (float time)
	{
		RpcUpdateServerAnswerTime (time);
	}

	[ClientRpc]
	public void RpcUpdateServerAnswerTime (float time)
	{
		serverAnswerTime = time;
	}

	[Command]
	public void CmdUpdateRoundNumber (int num)
	{
		RpcUpdateRoundNumber (num);
	}

	[ClientRpc]
	public void RpcUpdateRoundNumber (int num)
	{
		roundNumber = num;
	}

	[ClientRpc]
	public void RpcUpdateClientShieldAuthority (bool authority)
	{
		clientShieldAuthority = authority;
	}

	[ClientRpc]
	public void RpcUpdateClientSwordAuthority (bool authority)
	{
		clientSwordAuthority = authority;
	}

	[ClientRpc]
	public void RpcUpdateClientStarAuthority (bool authority)
	{
		clientStarAuthority = authority;
	}

	[ClientRpc]
	public void RpcUpdateClientHealth (float health)
	{
		clientHealth = health;
	}

	[ClientRpc]
	public void RpcUpdateServerHealth (float health)
	{
		serverHealth = health;
	}

	[ClientRpc]
	public void RpcUpdateDamageDealt (int damage)
	{
		damageDealt = damage;
	}

	[ClientRpc]
	public void RpcUpdateDamageBlocked (int damage)
	{
		damageBlocked = damage;
	}

	[Command]
	public void CmdUpdateTimeSinceLastSync(float time) {
		RpcUpdateTimeSinceLastSync (time);
	}
    
	[ClientRpc]
	public void RpcUpdateTimeSinceLastSync (float time) {
		timeSinceLastSync = time;
	}

	// Update is called once per frame
	void Update ()
	{
		if (gameLogic.currentlyNetworking ()) {
			timeSinceLastSync += Time.deltaTime;

			if (timeSinceLastSync > 5) {
				ui.fatalError ();
			}
		}

		if (!isLocalPlayer) {
			return;
		}

		if (!gameStarted && isServer && NetworkServer.connections.Count == 2) {
			gameStarted = true;
			ui.proceedToMultiplayerGame ();
		}

		if (canUpdateShield ()) {
			CmdShieldPosition (shield.GetComponent<Transform> ().position);
		}

		if (canUpdateSword ()) {
			CmdSwordPosition (sword.GetComponent<Transform> ().position);
		}

		if (canUpdateStar ()) {
			CmdStarPosition (star.GetComponent<Transform> ().position);
		}

		if (gameStarted && isServer && NetworkServer.connections.Count < 2) {
			ui.fatalError ();
		}

	}
}

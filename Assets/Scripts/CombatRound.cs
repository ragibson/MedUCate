using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatRound
{

	public float roundTime;
	private float currentTime = 0;
	private float damage;
	public float playerHealth;
	public float enemyHealth;

	public bool calculateDamage = false;

	public bool proceedToCombat = false;

	public CombatRound (float secondsPerRound, float defaultDamage, float playerHP, float enemyHP)
	{
		roundTime = secondsPerRound;
		damage = defaultDamage;
		playerHealth = playerHP;
		enemyHealth = enemyHP;
	}

	public void Update ()
	{
		currentTime += Time.deltaTime;
		if (timeRemaining () < 0) {
			calculateDamage = true;
		}
	}

	public float timeRemaining ()
	{
		return roundTime - currentTime;
	}

	/*
	 * 	Calculates damage dealt and blocked in a combat round
	 * 	Returns values through out parameters damageDealt, damageBlocked
	 */
	public void damageCalc (GameObject sword, GameObject shield, GameObject star, out int damageDealt, out int damageBlocked)
	{
		Transform sw = sword.transform;
		Transform sh = shield.transform;
		Transform st = star.transform;

		float swWidth = sw.localScale.x / 2;
		float swHeight = sw.localScale.y / 2;
		float xMin = sw.position.x - swWidth;
		float xMax = sw.position.x + swWidth;
		float yMin = sw.position.y - swHeight;
		float yMax = sw.position.y + swHeight;

		// Total number of raycasts, will be ~400
		int totalCount = 0;
		// Number of points with no overlap
		int noOverlapCount = 0;
		// Number of points that only overlap the star
		int starOnlyCount = 0;
		// Number of points that only overlap the shield
		int shieldOnlyCount = 0;
		// Number of points that overlap both star and shield
		int starShieldCount = 0;

		// We raycast in the ~20x20 grid at the center of a ~22x22 grid
		// We're using 22.1 for FP imprecision
		float xIncrement = (xMax - xMin) / 22.1f;
		float yIncrement = (yMax - yMin) / 22.1f;
		for (float x = xMin + xIncrement; x <= xMax - xIncrement; x += xIncrement) {
			for (float y = yMin + yIncrement; y <= yMax - yIncrement; y += yIncrement) {
				
				/*
				 * 	hitStar:
				 * 		- From position on sword
				 * 		- In the +z direction -- towards the star
				 * 		- Store the raycast hit info in *hitStar*
				 * 		- Stop raycast after 1 unit since we won't hit anything
				 * 
				 * 	hitShield:
				 * 		- From position on sword
				 * 		- In the -z direction -- towards the shield
				 * 		- Store the raycast hit info in *hitShield*
				 * 		- Stop raycast after 1 unit since we won't hit the shield
				 */
				RaycastHit hitStar;
				RaycastHit hitShield;
				Vector3 pos = new Vector3 (x, y, sword.transform.position.z);
				Vector3 toStar = new Vector3 (0, 0, 1);
				Vector3 toShield = new Vector3 (0, 0, -1);

				bool starHit = (Physics.Raycast (pos, toStar, out hitStar, 1f) && hitStar.collider.transform == star.transform);
				bool shieldHit = (Physics.Raycast (pos, toShield, out hitShield, 1f) && hitShield.collider.transform == shield.transform);

				if (starHit && shieldHit) {
					starShieldCount += 1;	// Star + Shield
				} else if (starHit) {
					starOnlyCount += 1;		// Star
				} else if (shieldHit) {
					shieldOnlyCount += 1;	// Shield
				} else {
					noOverlapCount += 1;
				}

				totalCount += 1;

//				if (starHit) {
//					Debug.DrawRay (pos, toStar * hitStar.distance, Color.red, 5f);
//				}
//				if (shieldHit) {
//					Debug.DrawRay (pos, toShield * hitShield.distance, Color.blue, 5f);
//				}
			}
		}

		/*
		 * 	Damage calculation is as follows:
		 * 		- The sword, with no multiplier, deals *damage* damage over its entire area
		 * 		- The shield blocks all damage where it overlaps with the sword
		 * 		- The star multiplies damage by 2x.
		 */

//		Debug.Log (noOverlapCount + "+" + starOnlyCount + "+" + shieldOnlyCount + "+" + starShieldCount + "=" + totalCount);

		damageDealt = Mathf.RoundToInt ((damage / totalCount) * (noOverlapCount + shieldOnlyCount + 2 * starOnlyCount + 2 * starShieldCount));
		damageBlocked = Mathf.RoundToInt ((damage / totalCount) * (shieldOnlyCount + 2 * starShieldCount));
	}
}

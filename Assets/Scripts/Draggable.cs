using UnityEngine;
using System.Collections;

public class Draggable : MonoBehaviour
{

	/*
	 *	Size of Draggable blocks
	 *	Should be edited in Unity editor, not script itself
	 */
	public float BLOCK_SIZE;

	/*
	 * 	The main display area of the game
	 * 	Used to maintain bounds on Draggable areas
	 */
	public RectTransform primaryDisplay;
	private float maxXDisplacement;
	private float maxYDisplacement;

	/*
	 *	Runs once on first frame of game
	 *	Used to initialize size of blocks
	 */
	void Start ()
	{
		// Scales Draggable object to BLOCK_SIZE dimensions
		this.transform.localScale = new Vector3 (BLOCK_SIZE, BLOCK_SIZE, 1);
	}

	/* 
	 *	Move this object to the mouse's position in space
	 *	We maintain z position since the original implementation
	 *	uses depth to obscure objects
	 *
	 *	Then, we force the object to remain in bounds
	 */
	void OnMouseDrag ()
	{
		Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		this.transform.position = new Vector3 (mousePos.x, mousePos.y, this.transform.position.z);
		forceInBounds ();
	}

	// Move Draggable object's location to be within the bounds of the Draggable area
	void forceInBounds ()
	{
		// Get global width and height of the main display
		maxXDisplacement = primaryDisplay.rect.width * primaryDisplay.localScale.x / 2 - this.transform.localScale.x / 2;
		maxYDisplacement = primaryDisplay.rect.height * primaryDisplay.localScale.y / 2 - this.transform.localScale.y / 2;

		Vector3 dist = this.transform.position - primaryDisplay.position;

		// Force object's 2D position to be within proper bounds
		dist.y = Mathf.Min (dist.y, maxYDisplacement);
		dist.y = Mathf.Max (dist.y, -maxYDisplacement);
		dist.x = Mathf.Min (dist.x, maxXDisplacement);
		dist.x = Mathf.Max (dist.x, -maxXDisplacement);

		this.transform.position = dist + primaryDisplay.position;
	}

}

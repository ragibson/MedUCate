using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SoundEffectManager : MonoBehaviour
{
	public AudioSource buttonSound;
	public AudioSource swordClash;
	public AudioSource winFanfare;

	void Start()
	{
		
	}

	public void buttonPressSound()
	{
		buttonSound.Play ();
	}

	public void swordClashSound() {
		swordClash.Play ();
	}

	public void winFanfareSound() {
		winFanfare.Play ();
	}
}
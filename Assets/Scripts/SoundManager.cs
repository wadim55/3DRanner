using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {


/*
*	FUNCTION:
*	This script plays and controls sounds used. Only 2D sounds are used
*	which are handled by a single SoundManager prefab.
*	
*	USED BY: This script is a part of the SoundManager prefab.
*
*/

public AudioSource[] asCharacterSounds;
public AudioSource[] asPowerupSounds;
public AudioSource[] asMenuSounds;
public AudioSource[] asMusic;
public AudioSource[] asEnemySounds;

//Sound Enums
public enum CharacterSounds
{
	Footsteps = 0,
	JumpLand = 1
}

public enum PowerupSounds
{
	CurrencyCollection = 0,
	PowerupCollection = 1
}

public enum MenuSounds
{
	ButtonTap = 0
}

public enum EnemySounds
{
	TiresSqueal = 0,
	Siren = 1
}

//Constants
private bool  bSoundEnabled = true;	//gameplay sounds
private bool  bMusicEnabled = true;//background music

//script references
private InGameScript hInGameScript;
private ControllerScript hControllerScript;

//variables
private bool  bPlayFootsteps = false;
private bool  bFootstepsPlaying = false;

/*
*	FUNCTION:	Turn the sounds On or Off
*/
public void toggleSoundEnabled ( bool state  ){
	 bSoundEnabled = state;
}

/*
*	FUNCION:	Turn the background music ON or OFF
*/
public void toggleMusicEnabled ( bool state  ){
	 bMusicEnabled = state;
	
	if (state == true)
		asMusic[0].Play();
	else
		asMusic[0].Pause();
}

public bool isSoundEnabled (){  return bSoundEnabled; }
public bool isMusicEnabled (){  return bMusicEnabled; }

void Start (){
	hControllerScript = GameObject.Find("Player").GetComponent<ControllerScript>() as ControllerScript;
	hInGameScript = GameObject.Find("Player").GetComponent<InGameScript>() as InGameScript;
	hControllerScript = GameObject.Find("Player").GetComponent<ControllerScript>() as ControllerScript;
	
	stopAllSounds();
	
	if (bMusicEnabled == true)
		asMusic[0].Play();
	else
		asMusic[0].Stop();
}

void Update (){
	StartCoroutine(toggleFootStepsSound());

	if(hInGameScript.isGamePaused()==true)
		stopSound(CharacterSounds.Footsteps);
	
	if(bPlayFootsteps==true)
	{
		//adjust footsteps pitch according to movement speed
        asCharacterSounds[(int)CharacterSounds.Footsteps].pitch = hControllerScript.getCurrentForwardSpeed()/3.0f;
		if(bFootstepsPlaying==false)
		{
			if (bSoundEnabled)
                asCharacterSounds[(int)CharacterSounds.Footsteps].Play();
			bFootstepsPlaying = true;
		}
	}
	else
	{
		if(bFootstepsPlaying==true)
		{			
			if (bSoundEnabled)
                asCharacterSounds[(int)CharacterSounds.Footsteps].Stop();
			bFootstepsPlaying = false;
		}
	}
}

/*
*	FUNCTION: Play a sound
*/
public void playSound ( CharacterSounds soundType  ){
	if (bSoundEnabled)
        asCharacterSounds[(int)soundType].Play();
}
public void playSound ( PowerupSounds soundType  ){
	if (bSoundEnabled)
        asPowerupSounds[(int)soundType].Play();
}
public void playSound ( MenuSounds soundType  ){
	if (bSoundEnabled)
        asMenuSounds[(int)soundType].Play();
}
public void playSound ( EnemySounds soundType  ){
    if (bSoundEnabled && asEnemySounds[(int)soundType].isPlaying == false)
        asEnemySounds[(int)soundType].Play();
}

/*
*	FUNCITON: Stop a sound
*/
public void stopSound ( CharacterSounds soundType  ){
    asCharacterSounds[(int)soundType].Stop();
}
public void stopSound ( PowerupSounds soundType  ){
    asPowerupSounds[(int)soundType].Stop();
}
public void stopSound ( MenuSounds soundType  ){
    asMenuSounds[(int)soundType].Stop();
}
public void stopSound ( EnemySounds soundType  ){
    asEnemySounds[(int)soundType].Stop();
}

/*
*	FUNCTION: Turn off footsetps sound if player is in the air and vice versa
*/
IEnumerator toggleFootStepsSound (){	
	yield return new WaitForEndOfFrame();
	
	if(!hControllerScript.isInAir())
		bPlayFootsteps = true;
	else
		bPlayFootsteps = false;
}

/*
*	FUNCTION: Stops all sounds except background music
*/
public void stopAllSounds ()

    {
        
        for (int i = 0; i<System.Enum.GetNames(typeof(CharacterSounds)).Length; i++)
		asCharacterSounds[i].Stop();
        for (int i=0; i<System.Enum.GetNames(typeof(PowerupSounds)).Length; i++)
		asPowerupSounds[i].Stop();
        for (int i=0; i<System.Enum.GetNames(typeof(MenuSounds)).Length; i++)
		asMenuSounds[i].Stop();
        for (int i=0; i<System.Enum.GetNames(typeof(EnemySounds)).Length; i++)
		asEnemySounds[i].Stop();
	
	bFootstepsPlaying = false;
}

/*
*	FUNCTION: Check if a sound is currently playing.
*/
public bool isPlaying ( CharacterSounds sound  ){
    if (asCharacterSounds[(int)sound].isPlaying)
		return true;
	else
		return false;
}
public bool isPlaying ( PowerupSounds sound  ){
    if (asPowerupSounds[(int)sound].isPlaying)
		return true;
	else
		return false;
}
public bool isPlaying ( MenuSounds sound  ){
    if (asMenuSounds[(int)sound].isPlaying)
		return true;
	else
		return false;
}
public bool isPlaying ( EnemySounds sound  ){
    if (asEnemySounds[(int)sound].isPlaying)
		return true;
	else
		return false;
}
}
using UnityEngine;
using System.Collections;

public class PitsMainController : MonoBehaviour {


/*
*	FUNCITON:
*	This script checks if there is a terrain layer under the player character.
*	If there isn't the player will fall and its energy will be reduced to zero.
*	
*	USED BY: This script is part of the "Player" prefab.
*
*/

private Transform tPlayer;
private bool  bPitFallingStart = false;
private float fCurrentEnergyDepletionSpeed = 10.0f;

private InGameScript hInGameScript;
private ControllerScript hControllerScript;

void Start (){
	tPlayer = GameObject.Find("Player").transform;
	bPitFallingStart = false;
	
	hInGameScript = this.GetComponent<InGameScript>() as InGameScript;
	hControllerScript = this.GetComponent<ControllerScript>() as ControllerScript;
}

void Update (){
	if(hInGameScript.isGamePaused()==true)
		return;
	
	if(bPitFallingStart)
	{
            hInGameScript.decrementEnergy(  (int)((hInGameScript.getCurrentEnergy()/10.0f) + Time.deltaTime*100));
	}
}

/*
*	FUNCTION: Reduce energy if player fell int a pit
*/
public void setPitValues (){
	bPitFallingStart = true;
		
	hControllerScript.setPitFallLerpValue(Time.time);
	hControllerScript.setPitFallForwardSpeed(hControllerScript.getCurrentForwardSpeed());
			
	print("Fell in a Pit");
}
        
public bool isFallingInPit (){ return bPitFallingStart; }
}
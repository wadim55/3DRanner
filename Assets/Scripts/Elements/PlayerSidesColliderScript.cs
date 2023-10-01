using UnityEngine;
using System.Collections;

public class PlayerSidesColliderScript : MonoBehaviour {


/*
*	FUNCTION: Control all the collisions from the sides.
*
*	USED BY: This script is part of the PlayerSidesCollider prefab.
*
*/

private bool  bSidesColliderFlag;

//script references
private InGameScript hInGameScript;
private PlayerFrontColliderScript hPlayerFrontColliderScript;
private ControllerScript hControllerScript;

void Start (){
	bSidesColliderFlag = true;
	
	hInGameScript = GameObject.Find("Player").GetComponent<InGameScript>() as InGameScript;
	hPlayerFrontColliderScript = GameObject.Find("PlayerFrontCollider").GetComponent<PlayerFrontColliderScript>() as PlayerFrontColliderScript;
	hControllerScript = GameObject.Find("Player").GetComponent<ControllerScript>() as ControllerScript;
}

/*
*	FUNCTION: Called when player bumps into an obstacle side-ways
*/
void OnCollisionEnter ( Collision collision  ){	
	if (hInGameScript.isEnergyZero())
		return;
	else if (bSidesColliderFlag == true)
	{
		hPlayerFrontColliderScript.deactivateFrontCollider();//pause front collision detection till stumble is processed
		hControllerScript.processStumble();	//handle the collision
	}
}

public bool isSidesColliderActive (){ return bSidesColliderFlag; }
public void deactivateSidesCollider (){ bSidesColliderFlag = false; }
public void activateSidesCollider (){ bSidesColliderFlag = true; }
}
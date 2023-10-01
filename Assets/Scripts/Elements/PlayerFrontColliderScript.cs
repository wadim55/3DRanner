using UnityEngine;
using System.Collections;

public class PlayerFrontColliderScript : MonoBehaviour {


/*
*	FUNCTION: Controls all frontal collisions.
*
*	USED BY: This script is part of the PlayerFrontCollider prefab.
*
*/

private bool  bFrontColliderFlag;

private PlayerSidesColliderScript hPlayerSidesColliderScript;
private InGameScript hInGameScript;

void Start (){
	bFrontColliderFlag = true;
	
	hPlayerSidesColliderScript = GameObject.Find("PlayerSidesCollider").GetComponent<PlayerSidesColliderScript>() as PlayerSidesColliderScript;
	hInGameScript = GameObject.Find("Player").GetComponent<InGameScript>() as InGameScript;
}

void OnCollisionEnter ( Collision collision  ){		
	if (bFrontColliderFlag == true)
	{
		hPlayerSidesColliderScript.deactivateSidesCollider();	//dont detect stumbles on death
		hInGameScript.collidedWithObstacle();	//play the death scene
	}
}

public bool isFrontColliderActive (){ return bFrontColliderFlag; }
public void activateFrontCollider (){ bFrontColliderFlag = true; }
public void deactivateFrontCollider (){ bFrontColliderFlag = false; }
}
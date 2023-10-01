using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour {

/*
*	FUNCTION:
*	- Controls the HUD display which includes the score and currency.
*	
*	USED BY: This script is a part of the "Player" prefab.
*
*/

private Transform tPlayer;	//player transfrom

//script references
private InGameScript hInGameScript;
private PowerupsMainController hPowerupsMainController;
private ControllerScript hControllerScript;

private TextMesh tmHUDCurrencyText;
private TextMesh tmHUDScoreText;
private Transform tHUDScoreContainerMid;
private Transform tHUDCurrencyContainerMid;

//Calculate Score
private float fPreviousDistance = 0.0f;	//mileage in the last frame
private float fCurrentDistance = 0.0f;		//mileage in the current frame
private float fCurrentTime = 0.0f;
private float fPreviousTime = 0.0f;

//HUD element Container sizes
private int iDivisorScore;
private int iDivisorCurrency;
private int iDivisorMultiplier;

void Start (){		
	tPlayer = GameObject.Find("Player").transform;
	hInGameScript = GameObject.Find("Player").GetComponent<InGameScript>() as InGameScript;	
	hControllerScript = GameObject.Find("Player").GetComponent<ControllerScript>() as ControllerScript;
	hPowerupsMainController = GameObject.Find("Player").GetComponent<PowerupsMainController>() as PowerupsMainController;

	tmHUDCurrencyText = GameObject.Find("HUDMainGroup/HUDGroup/HUDCurrencyGroup/HUD_Currency_Text").GetComponent<TextMesh>() as TextMesh;
	tmHUDScoreText = GameObject.Find("HUDMainGroup/HUDGroup/HUDScoreGroup/HUD_Score_Text").GetComponent<TextMesh>() as TextMesh;
		
	tHUDScoreContainerMid = GameObject.Find("HUDMainGroup/HUDGroup/HUDScoreGroup/HUD_Score_BG").GetComponent<Transform>() as Transform;	//	HUD Score Container	
	tHUDCurrencyContainerMid = GameObject.Find("HUDMainGroup/HUDGroup/HUDCurrencyGroup/HUD_Currency_BG").GetComponent<Transform>() as Transform;	//	HUD Currency Container
		
	//get time difference to calculate score
	fCurrentTime = Time.time;
	fPreviousTime = Time.time;
	
	fPreviousDistance = 0;
	fCurrentDistance = 0;
	fCurrentTime = 0;
	fPreviousTime = 0;
	
	iDivisorScore = 10;
	iDivisorCurrency = 10;
	iDivisorMultiplier = 10;
	
    tHUDScoreContainerMid.localScale = new Vector3(tHUDScoreContainerMid.localScale.x, tHUDScoreContainerMid.localScale.y,0.45f);
        tHUDCurrencyContainerMid.localScale = new Vector3 (tHUDCurrencyContainerMid.localScale.x,tHUDCurrencyContainerMid.localScale.y,0.45f);
	
	//call the resize Dight Container function every .5 seconds
	InvokeRepeating("resizeDigitContainer", 1, 0.5f);
	resizeDigitContainer();
}

void FixedUpdate (){	
	if(hInGameScript.isGamePaused()==true)
		return;

        StartCoroutine( UpdateHUDStats());
	
}//end of Update

/*
* 	FUNCTION: The score is calculated and added up in Level_Score variable
*	CALLED BY:	FixedUpdate()
*/
IEnumerator  UpdateHUDStats (){	
	yield return new WaitForEndOfFrame();
	
	//skip time and check the difference in milage in the duration
	if ( (fCurrentTime - fPreviousTime) >= 0.1f )
	{
		float iCurrentFrameScore= (fCurrentDistance - fPreviousDistance);
            hInGameScript.incrementLevelScore((int) iCurrentFrameScore);
		
		fPreviousDistance = fCurrentDistance;
		fCurrentDistance = hControllerScript.getCurrentMileage();
		
		fPreviousTime = fCurrentTime;
		fCurrentTime = Time.time;
	}
	else
	{
		fCurrentDistance = hControllerScript.getCurrentMileage();	//get the current mileage
		fCurrentTime = Time.time;
	}	
		
	tmHUDCurrencyText.text = hPowerupsMainController.getCurrencyUnits().ToString();	//update Currency on HUD
	tmHUDScoreText.text = hInGameScript.getLevelScore().ToString();				//update Score on HUD
}

/*
*	FUNCTION: Resize HUD Score and Currency containers according to digit count
*	CALLED BY:	Start() (invoke repeating)
*/
private void resizeDigitContainer (){
	int fScore = hInGameScript.getLevelScore();
	int fCurrency = hPowerupsMainController.getCurrencyUnits();
		
	if ( (fScore / iDivisorScore) >= 1 )
	{
		//tHUDScoreContainerMid.localScale.z += 0.4f;	//expand the Score Container Mid
        tHUDScoreContainerMid.localScale += new Vector3(0,0,0.4f);

		iDivisorScore *= 10;
	}
	
	if ( (fCurrency / iDivisorCurrency) >= 1 )
	{
		//tHUDCurrencyContainerMid.localScale.z += 0.4f;		//expand the Currency Container Mid
        tHUDCurrencyContainerMid.localScale += new Vector3(0,0,0.4f);
		iDivisorCurrency *= 10;
	}
}
}
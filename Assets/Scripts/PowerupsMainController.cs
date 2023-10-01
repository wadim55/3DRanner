using UnityEngine;
using System.Collections;

public class PowerupsMainController : MonoBehaviour {


/*
*	FUNCTION:
*	- This script activates and deactivates powerups.
*	- It defines the execution of powerups.
*	- It handles and triggers events when a currency unit is collected.
*
*/

//all the powerups used in the game
public enum PowerUps
{	
	Magnetism = 0,
	Currency = 1
}

private Transform tPlayer;	//player transform

private int iCurrencyUnits = 0;	//curency earned in a particular run
private float fMangetismRadius;	//when to pull currency if magnetism is active
private float fMagnetismDefaultRadius;	//when to pull currency
private int iPowerupCount;	//a count of types of powerups

private bool[ ] bPowerupStatus;	//if and which powerup is active
private float[] fPowerupStartTime;//the time when a powerup is started
private float[] fPowerupTotalDuration;//total time to keep the powerup active

//script references
private InGameScript hInGameScript;
private SoundManager hSoundManager;
private ControllerScript hControllerScript;

private Transform tHUDPUMeter;//the HUD powerup meter
private Transform tHUDPUMeterBar;//the bar in the powerup meter on HUD

void Start (){
	tPlayer = transform;	
	
	//powerup meter visual
	tHUDPUMeter = GameObject.Find("HUDMainGroup/HUDPUMeter").GetComponent<Transform>() as Transform;
	tHUDPUMeterBar = GameObject.Find("HUDMainGroup/HUDPUMeter/HUD_PU_Meter_Bar_Parent").GetComponent<Transform>() as Transform;
	
    iPowerupCount = System.Enum.GetNames(typeof(PowerUps)).Length-1; //PowerUps.GetValues(PowerUps).Length-1;//get the total number of powerups
	
	bPowerupStatus = new bool[ iPowerupCount];
	fPowerupStartTime = new float[iPowerupCount];	
	fPowerupTotalDuration = new float[iPowerupCount];

	hInGameScript = this.GetComponent<InGameScript>() as InGameScript;
	hControllerScript = this.GetComponent<ControllerScript>() as ControllerScript;
	hSoundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>() as SoundManager;
	
    tHUDPUMeter.transform.position -=new Vector3(0, 100,0);	//hide the powerup meter
	fMagnetismDefaultRadius = 200;
	fMangetismRadius = 200;		//default: pull currency toward the character
	iCurrencyUnits = 0;
	
	for(int i = 0; i <iPowerupCount ; i++)
	{
            
		bPowerupStatus[i] = false;
		fPowerupTotalDuration[i] = 10.0f;//active time duration of the powerups
	}
}

void FixedUpdate (){	
	//pause the powerup's time if the game is paused
	if(hInGameScript.isGamePaused()==true)
	{
		for (int j = 0; j<iPowerupCount; j++)
		{
			if (bPowerupStatus[j] == true)
				fPowerupStartTime[j] += Time.deltaTime;
		}
		return;
	}

	//count down timer for the active powerup
	for(int i = 0; i < iPowerupCount; i++)
	{
		if(bPowerupStatus[i]==true)
		{
			//reduce the meter bar
			PowerupHUDVisual( (Time.time - fPowerupStartTime[i]), fPowerupTotalDuration[i] );
			
			if(Time.time - fPowerupStartTime[i]>=fPowerupTotalDuration[i])//deactivate the PU when time runs out
			{
				deactivatePowerup(i);
			}
		}//end of if PU Active == true
	}//end of for i
}

/*
*	FUNCTION: Add collected currency or activate powerup
*	CALLED BY: PowerupScript.Update()
*/
public void collectedPowerup ( int index  ){
    if(index==(int)PowerUps.Currency)//if a currency unit is collected
	{
		iCurrencyUnits += 1;	//add 1 to the currency count
		hSoundManager.playSound(SoundManager.PowerupSounds.CurrencyCollection);//play collection sound

		return;
	}
	
	fPowerupStartTime[index] = Time.time;	//set the time when powerup collected
	activatePowerUp(index);		//activate powerup if collected
}

/*
*	FUNCTION: Enable the powerup's functionality
*	CALLED BY:	collectedPowerup()
*/
private void activatePowerUp ( int index  ){
        tHUDPUMeter.transform.position =new Vector3(tHUDPUMeter.transform.position.x, -88.6f,tHUDPUMeter.transform.position.z);//dispaly power-up meter
	bPowerupStatus[index] = true;
		
    if(index == (int) PowerUps.Magnetism)//Magnetism Powerup
	{
		fMangetismRadius =  fMagnetismDefaultRadius + 2300;
	}
}

/*
*	FUNCTION: Dactivate powerup when it time expires
*	CALLED BY: Update()
*/
public void deactivatePowerup ( int index  ){	
        tHUDPUMeter.transform.position = new Vector3(tHUDPUMeter.transform.position.x, 5000,tHUDPUMeter.transform.position.z);//hide power-up meter
	bPowerupStatus[index] = false;
	
        if(index == (int) PowerUps.Magnetism)//Magnetism Powerup
	{
		fMangetismRadius = fMagnetismDefaultRadius;
	}	
}

/*
*	FUNCTION: Deactivate all active powerups
*	CALLED BY:	InGameScript.Update()
*/
public void deactivateAllPowerups (){
    for (int i = 0; i< System.Enum.GetNames(typeof(PowerUps)).Length-1; i++)
    	{
            if (bPowerupStatus[i] == true)
    			deactivatePowerup(i);
    	}
}

/*
*	FUNCTION: Reduce the powerup meter's bar when a powerup is activated
*	CALLED BY: Update()
*/
private void PowerupHUDVisual ( float fCurrentTime ,  float fTotalTime  ){
	float iBarLength = tHUDPUMeterBar.transform.localScale.x;
	
	if (fCurrentTime <= 0)
		return;
	
	iBarLength = (fTotalTime-fCurrentTime)/fTotalTime;//calculate powerup meter bar's length
    tHUDPUMeterBar.transform.localScale =new Vector3( iBarLength,tHUDPUMeterBar.transform.localScale.y,tHUDPUMeterBar.transform.localScale.z);//set the length
}

/*
*	FUNCTION: Get the radius of magnetism effect
*/
public float getMagnetismRadius (){ return fMangetismRadius; }

/*
*	FUNCTION: Get the currency collected in current run
*/
public int getCurrencyUnits (){ return iCurrencyUnits; }

/*
*	FUNCTION: Check if any powerup is active
*	CALLED BY:	ElementsGenerator.getRandomElement()
*/
public bool isPowerupActive (){
	 for (int i = 0; i<iPowerupCount; i++)
	{
		if (bPowerupStatus[i] == true)
			return true;
	}
	
	return false;
}

/*
*	FUNCTION: Check if a particular powerup is active
*	PARAMETER 1: The powerup which needs to be checked
*	CALLED BY:	PowerupScript.Update()
*/
public bool isPowerupActive ( PowerUps ePUType  ){
    return bPowerupStatus[(int)ePUType];
}
}
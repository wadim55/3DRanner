using UnityEngine;
using System.Collections;

public class PowerupScript : MonoBehaviour {


/*
*	FUNCTION: This script defines the behavior of powerups and currency units.
*	
*	USED BY: This script is a part of every powerup and currency unit.
*
*/


public PowerupsMainController.PowerUps powerupType;
public int frequency;	//occurance frequency

private Transform tPlayer;//player transform
private int PUState = 0;
private float StartTime = 0.0f;

//script references
private InGameScript hInGameScript;
private PowerupsMainController hPowerupsMainController;

private Vector3 v3StartPosition;
private bool  bDestroyWhenFarFlag = false;
private Vector3 v3DistanceVector;
private float fCatchRadius = 200;//the radius at which Power Ups are pulled towards the character
private Vector3 v3CurrencyLerpPosition;

/*
*	FUNCTION: Make arrangements for reuse of the object
*/
public void initPowerupScript (){
	PUState = 0;
	bDestroyWhenFarFlag = false;
	transform.localScale = new Vector3(1,1,1);
	StartTime = 0.0f;
	v3DistanceVector = new Vector3(0,0,0);
	
	toggleMeshRender(true);
}

void Start (){
	tPlayer = GameObject.Find("Player").transform;
	
	hInGameScript = GameObject.Find("Player").GetComponent<InGameScript>() as InGameScript;
	hPowerupsMainController = GameObject.Find("Player").GetComponent<PowerupsMainController>() as PowerupsMainController;
}

void Update (){
	if(hInGameScript.isGamePaused()==true)
		return;
		
	if(PUState==1)//hide the powerup
	{
            if (hPowerupsMainController.isPowerupActive(PowerupsMainController.PowerUps.Magnetism) == true)	//magnetism powerup is active
		{
			//adjust the currency's height
			v3CurrencyLerpPosition = tPlayer.position;
			v3CurrencyLerpPosition.x += 2;
			v3CurrencyLerpPosition.y += 5;
			
			//pull the currency towards the player
			transform.position = Vector3.Lerp(transform.position,v3CurrencyLerpPosition,(Time.time-StartTime)/0.8f);
			transform.localScale = Vector3.Lerp(transform.localScale,new Vector3(0.1f,0.1f,0.1f),(Time.time-StartTime)/0.8f);
		}
		else//regular cases
		{			
			//pull the currency towards the player
			transform.position =  Vector3.Lerp(transform.position,tPlayer.position,(Time.time-StartTime)/0.2f);
			transform.localScale =  Vector3.Lerp(transform.localScale,new Vector3(0.01f,0.01f,0.01f),(Time.time-StartTime)/0.002f);
		}
		
		if((Time.time - StartTime)>0.2f)
		{	
			//disable currency if magnetism is activated
                if (powerupType == PowerupsMainController.PowerUps.Currency || hPowerupsMainController.isPowerupActive(PowerupsMainController.PowerUps.Magnetism) == true)			
				toggleMeshRender(false);//make currency invisible			
			else			
				this.gameObject.SetActive(false);//deactivate object			
		}
		return;
	}
	
	v3DistanceVector = transform.position - tPlayer.position;
	
	//destroy not collect currency/ powerup
	if(v3DistanceVector.sqrMagnitude<40000.0f)	
		bDestroyWhenFarFlag = true;
	
	//destroy currency or powerup if not collected
	if(bDestroyWhenFarFlag==true)
		if(v3DistanceVector.sqrMagnitude>90000.0f)
		{
            if (powerupType == PowerupsMainController.PowerUps.Currency)			
				toggleMeshRender(false);			
			else
				this.gameObject.SetActive(false);
		}

        if(powerupType==PowerupsMainController.PowerUps.Currency)//currency pull radius	
		fCatchRadius = hPowerupsMainController.getMagnetismRadius();
		
	if(v3DistanceVector.sqrMagnitude<fCatchRadius)//catch the orb
	{
		PUState = 1;//hide the orb
		StartTime = Time.time;
		
        hPowerupsMainController.collectedPowerup((int) powerupType);//tell power-up main script what has been collected		
	}
}//end of update

/*
*	FUNCTION: Make the object invisible
*/
private void toggleMeshRender ( bool bState  ){
        if (powerupType == PowerupsMainController.PowerUps.Currency)
	{
		(this.transform.Find("A_Crystal").GetComponent<MeshRenderer>() as MeshRenderer).enabled = bState;
		(this.transform.Find("Shadow").GetComponent<MeshRenderer>() as MeshRenderer).enabled = bState;
	}
        else if (powerupType == PowerupsMainController.PowerUps.Magnetism)
	{		
		(this.transform.Find("Center").GetComponent<MeshRenderer>() as MeshRenderer).enabled = bState;
	}
}
}
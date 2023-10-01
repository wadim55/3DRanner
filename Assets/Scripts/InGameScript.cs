using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class InGameScript : MonoBehaviour {

/*
*	FUNCITON:
*	- This script holds the global variables that the other scripts are dependent on.
*	- It provides interaction among scripts.
*	- This script controls game states (launch, pause, death etc.)
*
*	USED BY: This script is a part of the "Player" prefab.
*
*/

private int CurrentEnergy = 100;	//player's energy (set to zero on death)
private int iLevelScore = 0;	//current score (calculated based on distance traveled)

//script references
private MenuScript hMenuScript;
private ControllerScript hControllerScript;
private SoundManager hSoundManager;
private PowerupsMainController hPowerupsMainController;
private EnemyController hEnemyController;
private CameraController hCameraController;

private int iPauseStatus = 0;
private int iDeathStatus = 0;
private int iMenuStatus;

private bool  bGameOver = false;
private bool  bGamePaused = false;

void Start (){
	Application.targetFrameRate = 60;		//ceiling the frame rate on 60 (debug only)
	
	RenderSettings.fog = true;				//turn on fog on launch
		
	hSoundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>() as SoundManager;
	hMenuScript = GameObject.Find("MenuGroup").GetComponent<MenuScript>() as MenuScript;
	hControllerScript = this.GetComponent<ControllerScript>() as ControllerScript;
	hPowerupsMainController = this.GetComponent<PowerupsMainController>() as PowerupsMainController;
    hCameraController = Camera.main.GetComponent<CameraController>() as CameraController;
	hEnemyController = this.GetComponent<EnemyController>() as EnemyController;
	CurrentEnergy = 100;
	iPauseStatus = 0;
	iMenuStatus = 1;
	
	bGameOver = false;
	bGamePaused = true;
}//end of Start

void Update (){	
	if (iMenuStatus == 0)	//normal gameplay
		;
	else if (iMenuStatus == 1)//display main menu and pause game
	{
		hMenuScript.setMenuScriptStatus(true);
				
		bGamePaused = true;
		iMenuStatus = 2;
	}
	
	//Pause GamePlay
	if(iPauseStatus==0)	//normal gameplay
		;
	else if(iPauseStatus==1)//pause game
	{	
		hMenuScript.setMenuScriptStatus(true);		
		hMenuScript.displayPauseMenu();
		
		iPauseStatus = 2;
	}
	else if(iPauseStatus==3)//resume game
	{		
		bGamePaused = false;		
		hMenuScript.setMenuScriptStatus(false);
		
		iPauseStatus = 0;
	}
	
	if(iDeathStatus==0)	//normal gameplay
		;
	else if(iDeathStatus==1)//call death menu
	{
		hPowerupsMainController.deactivateAllPowerups();	//deactivate if a powerup is enabled
		
		iDeathStatus = 2;
	}
	else if (iDeathStatus == 2)
	{		
		hMenuScript.setMenuScriptStatus(true);
		hMenuScript.displayGameOverMenu();	//display the Game Over menu
		
		iDeathStatus = 0;
	}
	
	if (bGamePaused == true)
		return;
	
}//end of Update()

/*
*	FUNCTION: Pause the game
*	CALLED BY: ControllerScript.getClicks()
*/
public void pauseGame (){
	hControllerScript.togglePlayerAnimation(false);
	bGamePaused = true;
	iPauseStatus = 1;
	
	hSoundManager.stopAllSounds();
}

/*
*	FUNCTION: start the gameplay and display all related elements
*	CALLED BY: MenuScript.MainMenuGui()
*			   MenuScript.MissionsGui()
*/
public void launchGame (){	
	iMenuStatus = 0;
	bGamePaused = false;	
	hMenuScript.showHUDElements();	
	hControllerScript.launchGame();
	hCameraController.launchGame();
}

/*
*	FUNCTION: Display death menu and end game
*	CALLED BY:	ControllerScript.DeathScene()
*/
public void setupDeathMenu (){	
	bGameOver = true;
	bGamePaused = true;	
	iDeathStatus = 1;
}//end of Setup Death Menu

/*
*	FUNCTION: Execute a function based on button press in Pause Menu
*	CALLED BY: MenuScript.PauseMenu()
*/
    public void processClicksPauseMenu ( MenuScript.PauseMenuEvents index  ){
        if (index == MenuScript.PauseMenuEvents.MainMenu )
	{	
            SceneManager.LoadScene(0);
		
            hMenuScript.InvokeShowMenu((int)MenuScript.MenuIDs.MainMenu);
		iMenuStatus = 1;	//display main menu
	}
        else if (index == MenuScript.PauseMenuEvents.Resume)
	{
		hMenuScript.showHUDElements();
		hControllerScript.togglePlayerAnimation(true);
		iPauseStatus = 3;
	}
}

/*
*	FUNCTION: Execute a function based on button press in Death Menu
*	CALLED BY: MenuScript.GameOverMenu()
*/
    public void procesClicksDeathMenu ( MenuScript.GameOverMenuEvents index  ){
        if (index == MenuScript.GameOverMenuEvents.Play)
	{
		hMenuScript.showHUDElements();
        SceneManager.LoadScene(0);
		launchGame();
	}
        else if (index == MenuScript.GameOverMenuEvents.Back)
	{
        SceneManager.LoadScene(0);
		
            hMenuScript.InvokeShowMenu((int)MenuScript.MenuIDs.MainMenu);
		iMenuStatus = 1;	//display main menu
	}
}//end of DM_ProcessClicks

/*
*	FUNCTION: Is called when a collision occurs
*	CALLED BY:	PlayerFrontColliderScript.OnCollisionEnter
*				processStumble()
*/
public void collidedWithObstacle (){
	decrementEnergy(100);		// deduct energy after collision
	hCameraController.setCameraShakeImpulseValue(5);
}//end of Collided With Obstacle

/*
*	FUNCTION: Pause game if application closed/ switched on device
*/
void OnApplicationPause ( bool pause  ){
	Debug.Log("Application Paused : "+pause);
	if(Application.isEditor==false)
	{
		if(bGamePaused==false&&pause==false)
		{
			pauseGame();
		}
	}	
}
public bool isGamePaused (){ return bGamePaused; }
public int getLevelScore (){ return iLevelScore; }
public void incrementLevelScore ( int iValue  ){ iLevelScore += iValue; }
public int getCurrentEnergy (){ return CurrentEnergy; }
public bool isEnergyZero (){  return (CurrentEnergy <= 0 ? true : false); }
public void decrementEnergy ( int iValue  ){ CurrentEnergy -= iValue; }

}
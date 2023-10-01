using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

/*
*	FUNCTION:
*	- This script handles the menu instantiation, destruction and button event handling.
*	- Each menu item is always present in the scene. The inactive menus are hidden from the 
*	HUD Camera by setting their y position to 1000 units.
*	- To show a menu, the menu prefab's y position is set to 0 units and it appears in front of
*	the HUD Camera.
*
*	USED BY: This script is part of the MenuGroup prefab.
*
*	INFO:
*	To add a new menu, do the following in Editor:
*	- Add its name in the MenuIDs enum.
*	- Set its transform in tMenuTransforms array.
*	- Set its buttons' transforms in an array.
*	- Create its event handler. (E.g. handlerMainMenu(), handlerPauseMenu())
*	- Write implementation of each of the buttons in the handler function.
*	- Add its case in the listenerClicks() function.
*	- Add its case in the ShowMenu(...) function.
*	- Add its case in the CloseMenu(...) function.
*
*	ADDITIONAL INFO:
*	Unity's default GUI components are not used in the UI implementation to reduce 
*	performace overhead.
*
*/

//the available menus
public enum MenuIDs
{
	MainMenu = 0, 
	PauseMenu = 1,
	GameOverMenu = 2,
	InstructionsMenu = 3,
	SettingsMenu = 4
}

//events/ buttons on the pause menu
public enum PauseMenuEvents
{
	Resume = 0,
	MainMenu = 1
}

//events/ buttons on the game over menu
public enum GameOverMenuEvents
{
	Back = 0,
	Play = 1
}

private Transform tMenuGroup;//get the menu group parent
private int CurrentMenu = -1;	//current menu index
private int iTapState = 0;//state of tap on screen

private float aspectRatio = 0.0f;
private float fResolutionFactor;	//displacement of menu elements based on device aspect ratio
public float getResolutionFactor (){ return fResolutionFactor; }

private Camera HUDCamera;//the menu camera

//script references
private ControllerScript hControllerScript;
private SoundManager hSoundManagerScript;
private InGameScript hInGameScript;

private Transform[] tMenuTransforms;	//menu prefabs
//Main Menu
private Transform[] tMainMenuButtons;	//main menu buttons' transform
private int iMainMenuButtonsCount = 3;	//total number of buttons on main menu
//Pause Menu
private Transform[] tPauseButtons;	//pause menu buttons transforms
private int iPauseButtonsCount = 2;	//total number of buttons on pause menu
//Game Over Menu
private Transform[] tGameOverButtons;
private int iGameOverButtonsCount = 1;
//instructions menu
private Transform[] tInstructionsButtons;
private int iInstructionsButtonsCount = 1;
//settings menu
private Transform[] tSettingsButtons;
private int iSettingsButtonsCount = 7;

//meshrenderers of all the radio buttons in settings menu
private MeshRenderer mrSwipeControls;
private MeshRenderer mrGyroControls;
private MeshRenderer mrMusicON;
private MeshRenderer mrMusicOFF;
private MeshRenderer mrSoundON;
private MeshRenderer mrSoundOFF;

//resume game countdown
private int iResumeGameState = 0;
private float iResumeGameStartTime = 0;
private TextMesh tmPauseCountdown;	//count down numbers after resume
void Start (){
	HUDCamera = GameObject.Find("HUDMainGroup/HUDCamera").GetComponent<Camera>() as Camera;
	hControllerScript = GameObject.Find("Player").GetComponent<ControllerScript>() as ControllerScript;
	hSoundManagerScript = GameObject.Find("SoundManager").GetComponent<SoundManager>() as SoundManager;
	hInGameScript = GameObject.Find("Player").GetComponent<InGameScript>() as InGameScript;
		
	//the fResolutionFactor can be used to adjust components according to screen size
	aspectRatio = ( (Screen.height * 1.0f)/(Screen.width * 1.0f) - 1.77f);	
	fResolutionFactor = (43.0f * (aspectRatio));
		
	tMenuGroup = GameObject.Find("MenuGroup").transform;
        tMenuTransforms = new Transform[System.Enum.GetValues(typeof(MenuIDs)).Length];
	
	//main menu initialization
    tMenuTransforms[(int)MenuIDs.MainMenu] = tMenuGroup.Find("MainMenu").GetComponent<Transform>() as Transform;
	tMainMenuButtons = new Transform[iMainMenuButtonsCount];
    tMainMenuButtons[0] = tMenuTransforms[(int)MenuIDs.MainMenu].Find("Buttons/Button_TapToPlay");
    tMainMenuButtons[1] = tMenuTransforms[(int)MenuIDs.MainMenu].Find("Buttons/Button_Instructions");
    tMainMenuButtons[2] = tMenuTransforms[(int)MenuIDs.MainMenu].Find("Buttons/Button_Settings");
	
	//pause menu initialization
    tMenuTransforms[(int)MenuIDs.PauseMenu] = tMenuGroup.Find("PauseMenu").GetComponent<Transform>() as Transform;
	tPauseButtons = new Transform[iPauseButtonsCount];
    tPauseButtons[0] = tMenuTransforms[(int)MenuIDs.PauseMenu].Find("Buttons/Button_Back");
    tPauseButtons[1] = tMenuTransforms[(int)MenuIDs.PauseMenu].Find("Buttons/Button_Resume");
		
	//game over menu initialization
    tMenuTransforms[(int)MenuIDs.GameOverMenu] = tMenuGroup.Find("GameOver").GetComponent<Transform>() as Transform;
	tGameOverButtons = new Transform[iGameOverButtonsCount];
    tGameOverButtons[0] = tMenuTransforms[(int)MenuIDs.GameOverMenu].Find("Buttons/Button_Back");
	
	//instructions menu initialization
    tMenuTransforms[(int)MenuIDs.InstructionsMenu] = tMenuGroup.Find("Instructions").GetComponent<Transform>() as Transform;
	tInstructionsButtons = new Transform[iInstructionsButtonsCount];
    tInstructionsButtons[0] = tMenuTransforms[(int)MenuIDs.InstructionsMenu].Find("Buttons/Button_Back").GetComponent<Transform>() as Transform;
	
	//settings menu initialization
    tMenuTransforms[(int)MenuIDs.SettingsMenu] = tMenuGroup.Find("Settings").GetComponent<Transform>() as Transform;
	tSettingsButtons = new Transform[iSettingsButtonsCount];
    tSettingsButtons[0] = tMenuTransforms[(int)MenuIDs.SettingsMenu].Find("Buttons/Button_Back");
    tSettingsButtons[1] = tMenuTransforms[(int)MenuIDs.SettingsMenu].Find("ControlType/Button_Swipe/RadioButton_Background").GetComponent<Transform>() as Transform;
    tSettingsButtons[2] = tMenuTransforms[(int)MenuIDs.SettingsMenu].Find("ControlType/Button_Gyro/RadioButton_Background").GetComponent<Transform>() as Transform;
    tSettingsButtons[3] = tMenuTransforms[(int)MenuIDs.SettingsMenu].Find("Music/Button_ON/RadioButton_Background").GetComponent<Transform>() as Transform;
    tSettingsButtons[4] = tMenuTransforms[(int)MenuIDs.SettingsMenu].Find("Music/Button_OFF/RadioButton_Background").GetComponent<Transform>() as Transform;
    tSettingsButtons[5] = tMenuTransforms[(int)MenuIDs.SettingsMenu].Find("Sound/Button_ON/RadioButton_Background").GetComponent<Transform>() as Transform;
    tSettingsButtons[6] = tMenuTransforms[(int)MenuIDs.SettingsMenu].Find("Sound/Button_OFF/RadioButton_Background").GetComponent<Transform>() as Transform;
					
    mrSwipeControls = tMenuTransforms[(int)MenuIDs.SettingsMenu].Find("ControlType/Button_Swipe/RadioButton_Foreground").GetComponent<MeshRenderer>() as MeshRenderer;
    mrGyroControls = tMenuTransforms[(int)MenuIDs.SettingsMenu].Find("ControlType/Button_Gyro/RadioButton_Foreground").GetComponent<MeshRenderer>() as MeshRenderer;
    mrMusicON = tMenuTransforms[(int)MenuIDs.SettingsMenu].Find("Music/Button_ON/RadioButton_Foreground").GetComponent<MeshRenderer>() as MeshRenderer;
    mrMusicOFF = tMenuTransforms[(int)MenuIDs.SettingsMenu].Find("Music/Button_OFF/RadioButton_Foreground").GetComponent<MeshRenderer>() as MeshRenderer;
    mrSoundON = tMenuTransforms[(int)MenuIDs.SettingsMenu].Find("Sound/Button_ON/RadioButton_Foreground").GetComponent<MeshRenderer>() as MeshRenderer;
    mrSoundOFF = tMenuTransforms[(int)MenuIDs.SettingsMenu].Find("Sound/Button_OFF/RadioButton_Foreground").GetComponent<MeshRenderer>() as MeshRenderer;
	
	///////HUD//////
	(GameObject.Find("HUDMainGroup/HUDPauseCounter").GetComponent<MeshRenderer>() as MeshRenderer).enabled = false;
	
	//set the HUD position according to the screen resolution
	(GameObject.Find("HUDMainGroup/HUDGroup/HUDCurrencyGroup").GetComponent<Transform>() as Transform).transform.Translate(-fResolutionFactor,0,0);
	(GameObject.Find("HUDMainGroup/HUDGroup/HUDScoreGroup").GetComponent<Transform>() as Transform).transform.Translate(-fResolutionFactor,0,0);
	(GameObject.Find("HUDMainGroup/HUDGroup/HUDPause").GetComponent<Transform>() as Transform).transform.Translate(fResolutionFactor,0,0);
		
        StartCoroutine( ShowMenu((int)MenuIDs.MainMenu));	//show Main Menu on game launch
}

/*
*	FUNCTION: Show the pause menu
*	CALLED BY:	InGameScript.Update()
*/
public void displayPauseMenu (){
        StartCoroutine(ShowMenu((int)MenuIDs.PauseMenu));
}

/*
*	FUNCTION: Show the game over menu
*	CALLED BY:	InGameScript.Update()
*/
public void displayGameOverMenu (){	
        StartCoroutine(ShowMenu((int)MenuIDs.GameOverMenu));	
}

void FixedUpdate (){		
	//display countdown timer on Resume
	if (iResumeGameState == 0)
		;
	else if (iResumeGameState == 1)//display the counter
	{
		tmPauseCountdown = GameObject.Find("HUDMainGroup/HUDPauseCounter").GetComponent<TextMesh>() as TextMesh;
		(GameObject.Find("HUDMainGroup/HUDPauseCounter").GetComponent<MeshRenderer>() as MeshRenderer).enabled = true;
		
		iResumeGameStartTime = Time.time;		
		iResumeGameState = 2;
	}
	else if (iResumeGameState == 2)//count down
	{
		tmPauseCountdown.text = Mathf.Round(4 - (Time.time - iResumeGameStartTime)).ToString();
		
		if ( (Time.time - iResumeGameStartTime) >= 3)//resume the game when time expires
		{
            tmPauseCountdown.text = string.Empty;
			hInGameScript.processClicksPauseMenu(PauseMenuEvents.Resume);
			iResumeGameStartTime = 0;			
			iResumeGameState = 0;
		}
	}	
}//end of fixed update

void OnGUI (){
	listenerClicks();//listen for clicks
}

/*
*	FUNCTION: Detect taps and call the relevatn event handler.
*	CALLED BY:	The FixedUpdate() function.
*/
private RaycastHit hit;
private void listenerClicks (){
	if (Input.GetMouseButtonDown(0) && iTapState == 0)//detect taps
	{	
		iTapState = 1;
		
	}//end of if get mouse button
	else if (iTapState == 1)//call relevent handler
	{
		if (Physics.Raycast(HUDCamera.ScreenPointToRay(Input.mousePosition),out hit))//if a button has been tapped
		{
			//call the listner function of the active menu
            if (CurrentMenu == (int) MenuIDs.MainMenu)
				handlerMainMenu(hit.transform);
            else if (CurrentMenu == (int) MenuIDs.PauseMenu)
				handlerPauseMenu(hit.transform);
            else if (CurrentMenu == (int) MenuIDs.GameOverMenu)
				handlerGameOverMenu(hit.transform);
            else if (CurrentMenu == (int) MenuIDs.InstructionsMenu)
				handlerInstructionsMenu(hit.transform);
            else if (CurrentMenu == (int) MenuIDs.SettingsMenu)
				handlerSettingsMenu(hit.transform);
		}//end of if raycast
		
		iTapState = 2;
	}
	else if (iTapState == 2)//wait for user to release before detcting next tap
	{
		if (Input.GetMouseButtonUp(0))
			iTapState = 0;
	}
}//end of listner function

/*
*	FUNCTION: Handle clicks on Main Menu
*/
private void handlerMainMenu ( Transform buttonTransform  ){		
	if (tMainMenuButtons[0] == buttonTransform)//Tap to Play button
	{
        CloseMenu((int)MenuIDs.MainMenu);
		
		hInGameScript.launchGame();	//start the gameplay
		setMenuScriptStatus(false);
	}
	else if (tMainMenuButtons[1] == buttonTransform)//information button
	{
        CloseMenu((int) MenuIDs.MainMenu);
        StartCoroutine(ShowMenu((int) MenuIDs.InstructionsMenu));
        CurrentMenu = (int) MenuIDs.InstructionsMenu;
	}
	else if (tMainMenuButtons[2] == buttonTransform)//settings button
	{
        CloseMenu((int) MenuIDs.MainMenu);
        StartCoroutine(ShowMenu((int) MenuIDs.SettingsMenu));		
	}
}//end of handler main menu function

/*
*	FUNCTION: Handle clicks on pause menu.
*/
private void handlerPauseMenu ( Transform buttonTransform  ){
	if (tPauseButtons[0] == buttonTransform)//back button handler
	{
		hInGameScript.processClicksPauseMenu(PauseMenuEvents.MainMenu);
		
        CloseMenu((int) MenuIDs.PauseMenu);		
        StartCoroutine(ShowMenu((int) MenuIDs.MainMenu));
	}
	else if (tPauseButtons[1] == buttonTransform)//resume button handler
	{
        CloseMenu((int) MenuIDs.PauseMenu);
		iResumeGameState = 1;//begin the counter to resume
	}
}

/*
*	FUNCTION: Handle clicks on Game over menu.
*/
private void handlerGameOverMenu ( Transform buttonTransform  ){
	if (tGameOverButtons[0] == buttonTransform)//main menu button
	{
		hInGameScript.procesClicksDeathMenu(GameOverMenuEvents.Back);
        CloseMenu((int) MenuIDs.GameOverMenu);
        StartCoroutine(ShowMenu((int) MenuIDs.MainMenu));		
	}
	else if (tGameOverButtons[1] == buttonTransform)//play button
	{
		hInGameScript.procesClicksDeathMenu(GameOverMenuEvents.Play);		
		CloseMenu(CurrentMenu);
	}	
}

/*
*	FUNCTION: Handle the clicks on Information menu.
*/
private void handlerInstructionsMenu ( Transform buttonTransform  ){
	if (tInstructionsButtons[0] == buttonTransform)//main menu button
	{
        CloseMenu((int) MenuIDs.InstructionsMenu);
        StartCoroutine(ShowMenu((int) MenuIDs.MainMenu));		
	}	
}

/*
*	FUNCTION: Handle the clicks on Information menu.
*	CALLED BY:	listenerClicks()
*/
private void handlerSettingsMenu ( Transform buttonTransform  ){
	if (tSettingsButtons[0] == buttonTransform)//home button
	{
        CloseMenu((int) MenuIDs.SettingsMenu);
        StartCoroutine(ShowMenu((int) MenuIDs.MainMenu));
	}
	else if (tSettingsButtons[1] == buttonTransform)//swipe controls
	{		
		if (mrSwipeControls.enabled == false)
		{
			mrSwipeControls.enabled = true;
			mrGyroControls.enabled = false;
			hControllerScript.toggleSwipeControls(true);
		}		
	}
	else if (tSettingsButtons[2] == buttonTransform)//gyro controls
	{		
		if (mrGyroControls.enabled == false)
		{
			mrGyroControls.enabled = true;
			mrSwipeControls.enabled = false;
			hControllerScript.toggleSwipeControls(false);
		}		
	}
	else if (tSettingsButtons[3] == buttonTransform)//music ON radio button
	{
		if (mrMusicON.enabled == false)
		{
			mrMusicON.enabled = true;
			mrMusicOFF.enabled = false;
			hSoundManagerScript.toggleMusicEnabled(true);
		}
	}
	else if (tSettingsButtons[4] == buttonTransform)//music OFF radio button
	{
		if (mrMusicON.enabled == true)
		{
			mrMusicON.enabled = false;
			mrMusicOFF.enabled = true;
			hSoundManagerScript.toggleMusicEnabled(false);
		}
	}
	else if (tSettingsButtons[5] == buttonTransform)//music ON radio button
	{
		if (mrSoundON.enabled == false)
		{
			mrSoundON.enabled = true;
			mrSoundOFF.enabled = false;
			hSoundManagerScript.toggleSoundEnabled(true);
		}
	}
	else if (tSettingsButtons[6] == buttonTransform)//music ON radio button
	{
		if (mrSoundON.enabled == true)
		{
			mrSoundON.enabled = false;
			mrSoundOFF.enabled = true;
			hSoundManagerScript.toggleSoundEnabled(false);
		}
	}
}


public void InvokeShowMenu (int index)
{
        StartCoroutine(ShowMenu(index));
}
/*
*	FUNCTION: Set the menu to show in front of the HUD Camera
*/
 IEnumerator ShowMenu ( int index  ){
	yield return new WaitForFixedUpdate();
	
    if ((int) MenuIDs.MainMenu == index)	
            tMenuTransforms[(int) MenuIDs.MainMenu].position = new Vector3(tMenuTransforms[(int) MenuIDs.MainMenu].position.x,0,tMenuTransforms[(int) MenuIDs.MainMenu].position.z);
    else if ((int) MenuIDs.PauseMenu == index)
            tMenuTransforms[(int) MenuIDs.PauseMenu].position = new Vector3(tMenuTransforms[(int) MenuIDs.PauseMenu].position.x,0,tMenuTransforms[(int) MenuIDs.PauseMenu].position.z);
    else if ((int) MenuIDs.GameOverMenu == index)
            tMenuTransforms[(int) MenuIDs.GameOverMenu].position = new Vector3(tMenuTransforms[(int) MenuIDs.GameOverMenu].position.x,0,tMenuTransforms[(int) MenuIDs.GameOverMenu].position.z);
    else if ((int) MenuIDs.InstructionsMenu == index)
            tMenuTransforms[(int) MenuIDs.InstructionsMenu].position = new Vector3(tMenuTransforms[(int) MenuIDs.InstructionsMenu].position.x,0,tMenuTransforms[(int) MenuIDs.InstructionsMenu].position.z);
    else if ((int) MenuIDs.SettingsMenu == index)
	{
		//check which type of controls are active and 
		//set the appropriate radio button 
		if ( hControllerScript.isSwipeControlEnabled() )
		{
			mrSwipeControls.enabled = true;
			mrGyroControls.enabled = false;
		}
		else
		{
			mrSwipeControls.enabled = false;
			mrGyroControls.enabled = true;
		}
		
		//check if the music is enabled or disabled and
		//set the appropriate radio button
		if (hSoundManagerScript.isMusicEnabled())
		{
			mrMusicON.enabled = true;
			mrMusicOFF.enabled = false;
		}
		else
		{
			mrMusicON.enabled = false;
			mrMusicOFF.enabled = true;
		}
		
		//check if the sound is ON or OFF and se the
		//appropriate radio button
		if (hSoundManagerScript.isSoundEnabled())
		{
			mrSoundON.enabled = true;
			mrSoundOFF.enabled = false;
		}
		else
		{
			mrSoundON.enabled = false;
			mrSoundOFF.enabled = true;
		}
		
            tMenuTransforms[(int) MenuIDs.SettingsMenu].position = new Vector3(tMenuTransforms[(int) MenuIDs.SettingsMenu].position.x,0,tMenuTransforms[(int) MenuIDs.SettingsMenu].position.z);
	}
	
	CurrentMenu = index;
	hideHUDElements();	//hide the HUD

        hSoundManagerScript.playSound(SoundManager.MenuSounds.ButtonTap);
}

/*
*	FUNCTION: Send the menu away from the HUD Camera
*/
private void CloseMenu ( int index  ){
    if (index == (int) MenuIDs.MainMenu)		
            tMenuTransforms[(int)MenuIDs.MainMenu].position = new Vector3(tMenuTransforms[(int)MenuIDs.MainMenu].position.x,1000,tMenuTransforms[(int)MenuIDs.MainMenu].position.z);
    else if (index == (int) MenuIDs.PauseMenu)
            tMenuTransforms[(int) MenuIDs.PauseMenu].position = new Vector3(tMenuTransforms[(int)MenuIDs.PauseMenu].position.x,1000,tMenuTransforms[(int)MenuIDs.PauseMenu].position.z);
    else if (index == (int) MenuIDs.GameOverMenu)
            tMenuTransforms[(int) MenuIDs.GameOverMenu].position = new Vector3(tMenuTransforms[(int)MenuIDs.GameOverMenu].position.x,1000,tMenuTransforms[(int)MenuIDs.GameOverMenu].position.z);
    else if ((int) MenuIDs.InstructionsMenu == index)
            tMenuTransforms[(int) MenuIDs.InstructionsMenu].position = new Vector3(tMenuTransforms[(int)MenuIDs.InstructionsMenu].position.x,1000,tMenuTransforms[(int)MenuIDs.InstructionsMenu].position.z);
    else if ((int) MenuIDs.SettingsMenu == index)		
            tMenuTransforms[(int) MenuIDs.SettingsMenu].position = new Vector3(tMenuTransforms[(int)MenuIDs.SettingsMenu].position.x,1000,tMenuTransforms[(int)MenuIDs.SettingsMenu].position.z);
	
	CurrentMenu = -1;
}

public void hideHUDElements (){
        (GameObject.Find("HUDMainGroup/HUDGroup").GetComponent<Transform>() as Transform).position = new Vector3((GameObject.Find("HUDMainGroup/HUDGroup").GetComponent<Transform>() as Transform).position.x,1000,(GameObject.Find("HUDMainGroup/HUDGroup").GetComponent<Transform>() as Transform).position.z);
}

public void showHUDElements (){	
        (GameObject.Find("HUDMainGroup/HUDGroup").GetComponent<Transform>() as Transform).position = new Vector3((GameObject.Find("HUDMainGroup/HUDGroup").GetComponent<Transform>() as Transform).position.x,0,(GameObject.Find("HUDMainGroup/HUDGroup").GetComponent<Transform>() as Transform).position.z);;
}

/*
*	FUNCTION: Enable/ disable MenuScript.
*	CALLED BY: InGameScript.Update()
*	ADDITIONAL INFO: The MenuScript is disabled during gameplay for improved performance.
*/
public void setMenuScriptStatus ( bool flag  ){
	 if (flag != this.enabled)
		this.enabled = flag;
}
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour {

    public Text HPText;
    public Text AmmoText;
    public Text WorkersNumberText;

    public GameObject TextPanel; //need a reference to this so pausing/unpausing is possible during the intro too
    public GameObject MenuPanel; //while playing if the user hits escape GameState.Paused is set and the PausePanel is set to active
    public GameObject OptionsPanel; //if the user clicks the options button while the game is paused or on the main menu, the options menu is brought up*/
    public GameObject GameoverPanel;// the panel that is brought up when the level is over/life is lost
    public Text EndingText; //either "Game Over" or "Victory"

    public GameObject Crosshair; 

    //no TextPanel, Crosshair or Worker/Player display at the main menu scene (before the level is loaded)

    [HideInInspector]
    public TextPanelController TPC;
    private Dropdown _resolutionDropdown;
    private Resolution[] _resolutions;

    void Start ()
    {
        if (TextPanel != null)
            TPC = TextPanel.GetComponent<TextPanelController>();

        if (OptionsPanel != null)
        {
          
            Dropdown[] OptionsDropdowns = OptionsPanel.GetComponentsInChildren<Dropdown>();
            for (int i = 0; i < OptionsDropdowns.Length; i++)  //first get the correct dropdown from the Options Panel
            {
                if (OptionsDropdowns[i].name == "Resolution Dropdown")
                {
                    _resolutionDropdown = OptionsDropdowns[i];
                }
            }

            _resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray(); //then populate the dropdown with all the built in resolutions supported by unity
            _resolutionDropdown.ClearOptions();

            List<string> ResolutionOptions = new List<string>();
            int CurrentResolutionIndex = 0;
            for (int i = 0; i < _resolutions.Length; i++)
            {
                string Resolution = _resolutions[i].width + "x" + _resolutions[i].height;
                ResolutionOptions.Add(Resolution);

                if (_resolutions[i].width == Screen.currentResolution.width && _resolutions[i].height == Screen.currentResolution.height)
                    CurrentResolutionIndex = i;
            }

            _resolutionDropdown.AddOptions(ResolutionOptions);          //resolutions get turned into strings so they can be used as drop down options
            _resolutionDropdown.value = CurrentResolutionIndex;         //the string indexes in the list co-respond to indexes of the actual resolutions in the resolution array
            _resolutionDropdown.RefreshShownValue();                    //so when they dropdown registers an event by clicking on one of them, the integer that gets sent back co-responds to the index in the array
        }

        if (Crosshair)
            Crosshair.SetActive(false);
    }
	
	void Update ()
    {           
        if (GameManager.GM.GetState() == GameManager.GameState.Playing)       
            UpdateWorkerDisplay();
            //updates of health and ammo get called by other scripts           
    }

    public void UpdateHPText(float value)
    {
        if (value < 0)
            value = 0;
        HPText.text = value + " HP";
    }

    public void UpdateAmmoDisplay(int current, int max)
    {
        AmmoText.text = current + " / " + max;
    }

    public void UpdateWorkerDisplay()
    {
        WorkersNumberText.text = "x " + SpawnObject.GetSpawnedNumber("Worker");
    }

    public void SetPauseScreen()
    {
        if (GameManager.GM.GetState() == GameManager.GameState.Intro)
            TPC.ShouldResume = true;

        if (GameManager.GM.EffectsManager.AlarmEffectPanel.activeInHierarchy)
        {
            GameManager.GM.EffectsManager.SetAlarmEffect(false);
            GameManager.GM.EffectsManager.SetShouldResume(true);
        }


        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Crosshair.SetActive(false);

        MenuPanel.SetActive(true);     
    }

    public void UnpauseGame() // called by button
    {
        if (TPC.ShouldResume == true)
        {
            TPC.ShouldResume = false;
            GameManager.GM.SetState(GameManager.GameState.Intro);
            TextPanel.SetActive(true);
        }
        else
        {
            GameManager.GM.SetState(GameManager.GameState.Playing);
            if (GameManager.GM.EffectsManager.GetShouldResume() == true)
                GameManager.GM.EffectsManager.SetAlarmEffect(true);

            Crosshair.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        MenuPanel.SetActive(false);
    }


    public void SwitchMenu() //called by button 
    {
        MenuPanel.SetActive(!MenuPanel.activeInHierarchy);
        OptionsPanel.SetActive(!OptionsPanel.activeInHierarchy);
    }

    public void SetResolution(int resolutionIndex) //called by the events registered by the dropdown
    {
        Resolution resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void DisplayEndingScreen(string message)
    {
        GameoverPanel.SetActive(true);
        EndingText.text = message;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Crosshair.SetActive(false);
    }
}

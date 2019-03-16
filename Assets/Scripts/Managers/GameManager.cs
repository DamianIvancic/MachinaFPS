using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour {

    public UIManager UIManager;
    public ScoreManager ScoreManager;
    public EffectsManager EffectsManager;
    public AudioMixer AudioMixer;
 
	public enum GameState
	{
        Intro,
        Playing,
        Paused,       	
		GameOver
	}

    [SerializeField]
    private GameState _currentState;

    [HideInInspector]
    public static GameManager GM;

    void Awake()
	{
        if (GM == null)
        {      
            _currentState = GameState.Intro;
            GM = this;
        }
    }
		
	void Update () 
	{
       
        //all dynamic objects have a check in their update functions to run the logic only if _currentState = GameState.Playing
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.SetPauseScreen();
            _currentState = GameState.Paused;       
        }
 
	}

    public void SetState(GameState State)
    {
        _currentState = State;
    }

    public GameState GetState()
    {
        return _currentState;
    }

    public void LoadScene(string SceneName)
    {    
        SceneManager.LoadScene(SceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        Debug.Log(volume);
        AudioMixer.SetFloat("MasterVolume", volume);
    }
}


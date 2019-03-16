using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextPanelController : MonoBehaviour
{
    [HideInInspector]
    public bool ShouldResume;

    private Text _introText;
    private Text _messageText;
    private const int _numMessages = 6;
    private int _currentMessage = 1;
    private float _timer = 0f;
    private float _interval = 1.5f;


    void Start()
    {       
        Text[] texts = GetComponentsInChildren<Text>();

        for(int i=0; i<texts.Length; i++)
        {
            if (texts[i].gameObject.name == "Intro Text")
                _introText = texts[i];

            if (texts[i].gameObject.name == "Message Text")
                _messageText = texts[i];
        }
    }

    void Update()
    {
        if (GameManager.GM.GetState() == GameManager.GameState.Intro)
        {
            switch (_currentMessage)
            {
                case 1:
                    _introText.text = "It's time to express dissatisfaction with your work in a reasonable manner.";
                    break;
                case 2:
                    _introText.text = "Remember, this is a peaceful protest, killing your co-workers is prohibited.\n\n\rThe goal is just to disrupt the factory's schedule.";
                    break;
                case 3:
                    _introText.text = "You'll see your co-workers carrying boxes - try putting a few bullets into them to voice your discontent. (the boxes... not the workers)";
                    break;
                case 4:
                    _introText.text = "If you destroy 30 of those things before they manage to get 10 out, you win !";
                    break;
                case 5:
                    _introText.text = "Alright, let's get down to bussiness!\n\rFor Mother Russia, for the Proletariat!";
                    break;
                case 6:
                    _introText.text = "...btw, try to get this over with asap. Putin's got his eyes on you.";
                    break;
            }

            if (Input.anyKeyDown)
                _currentMessage++;

            if (IntroFinished())
            {
                _introText.text = null;
                gameObject.SetActive(false);
                GameManager.GM.SetState(GameManager.GameState.Playing);
                GameManager.GM.UIManager.Crosshair.SetActive(true);
            }
        }
        else
        {
            if (ShouldResume)
                gameObject.SetActive(false);
            else
            {
                _timer += Time.deltaTime;
                if (_timer > _interval)
                {
                    _timer = 0f;
                    _messageText.text = null;
                    gameObject.SetActive(false);
                }
            }      
        }
    }

    public IEnumerator SetText(string text)
    {
        if (text == _messageText.text)   
            yield return new WaitForSeconds(_interval - _timer + 0.5f);
      
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

        _introText.text = null;
        _messageText.text = text;

        yield return null;
    }

    public bool IntroFinished()
    {
        if (_currentMessage > _numMessages)
            return true;
        else
            return false;
    }

}


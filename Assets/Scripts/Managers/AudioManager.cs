using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {


    private AudioSource _audioSource;

	void Start ()
    {
        _audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(GameManager.GM.GetState() == GameManager.GameState.Playing)
        {

            if (!_audioSource.enabled)
                _audioSource.enabled = true;

            if (!_audioSource.isPlaying)
                _audioSource.Play();
        }       
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectsManager : MonoBehaviour {

    public GameObject PutinPic;
    public GameObject AlarmEffectPanel;

    [HideInInspector]
    public bool _shouldResume;

    private NationalSecurity _KGB;
    private AudioSource _alarm;

    private GameObject _player;

    void Start()
    {
        if (PutinPic)
            _KGB = PutinPic.GetComponent<NationalSecurity>();

        _player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if(AlarmEffectPanel.activeInHierarchy)
        {
            if(_KGB._spawned == false)
            {
                if (!_alarm.isPlaying)
                    AlarmEffectPanel.SetActive(false);

                if ((_player.transform.position - PutinPic.transform.position).magnitude > 20f)
                    AlarmEffectPanel.SetActive(false);
            }
            else
            {
                _alarm.loop = true;

                if(_KGB._agentCount == 0)
                    AlarmEffectPanel.SetActive(false);
            }           
        }
    }

    public void SetAlarmEffect(bool state)
    {
        AlarmEffectPanel.SetActive(state);
        if(_alarm == null)
             _alarm = GetComponentInChildren<AudioSource>();
    }

    public void SetShouldResume(bool shouldResume)
    {
        _shouldResume = shouldResume;
    }

    public bool GetShouldResume()
    {
        return _shouldResume;
    }
}

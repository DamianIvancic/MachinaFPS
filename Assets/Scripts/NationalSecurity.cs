using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NationalSecurity : MonoBehaviour {

    public GameObject GuardPrefab;
    public GameObject GuardSpawnA;
    public GameObject GuardSpawnB;

    [HideInInspector]
    public bool _spawned = false;
    [HideInInspector]
    public int _agentCount = 0;

    private AudioSource _spawnSound;
  
    void Start()
    {
        _spawnSound = GetComponent<AudioSource>();     
    }

    public void SpawnGuards()
    {
        if(!_spawned)
        {
            _spawnSound.Play();
            GameObject GuardA = Instantiate(GuardPrefab, GuardSpawnA.transform.position, Quaternion.identity, GuardSpawnA.transform);
            GameObject GuardB = Instantiate(GuardPrefab, GuardSpawnB.transform.position, Quaternion.identity, GuardSpawnB.transform);
            _spawned = true;          
        }  
    }
}

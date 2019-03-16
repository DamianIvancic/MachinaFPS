using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour {

    public GameObject SpawnPrefab;

    private float _timer = 0f;
    private float _interval = 15f;
    private static Dictionary<string, int> _spawnedObjects = new Dictionary<string, int>();
 
    void Start()
    {
        if (!_spawnedObjects.ContainsKey(SpawnPrefab.tag))
            _spawnedObjects.Add(SpawnPrefab.tag, 1);
        else
            _spawnedObjects[SpawnPrefab.tag]++;

        Instantiate(SpawnPrefab, transform.position, Quaternion.identity, transform);

    }
		
	void Update () {

        if (GameManager.GM.GetState() == GameManager.GameState.Playing)
        {
            _timer += Time.deltaTime;

            if (_timer > _interval && _spawnedObjects[SpawnPrefab.tag] < 10)
            {
                Instantiate(SpawnPrefab, transform.position, Quaternion.identity, transform);
                _spawnedObjects[SpawnPrefab.tag]++;
                _timer = 0f;
            }
        }
        else if (GameManager.GM.GetState() == GameManager.GameState.GameOver)
            _spawnedObjects.Clear();
	}

    public static int GetSpawnedNumber(string objectTag)
    {
        if (_spawnedObjects.ContainsKey(objectTag))
            return _spawnedObjects[objectTag];
        else
            return 0;
    }

    public void SpawnWorker(int number)
    {
        for(int i =0; i<number; i++)
        {
            Instantiate(SpawnPrefab, transform.position, Quaternion.identity, transform);
            _spawnedObjects[SpawnPrefab.tag]++;
        }
    }

    public static void AdjustSpawnedNumber(string objectTag, int value)
    {
        if (_spawnedObjects.ContainsKey(objectTag))
            _spawnedObjects[objectTag] += value;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {

    public GameObject ToFollow;
    private GameObject _Player; //only needed for UI objects so that their transform would always look towards the player
   
	void Start () {

        if (gameObject.tag == "UI")
        {
            _Player = GameObject.FindWithTag("Player");
    
            Vector3 Scale = transform.localScale;
            Scale.x *= -1;
            transform.localScale = Scale;
        }   
	}
	
	// Update is called once per frame
	void Update () {

        if(ToFollow != null)
        {
            Vector3 TargetPos = ToFollow.transform.position;

            if (gameObject.tag == "UI")
            {
                TargetPos.y += 1.5f;
                transform.LookAt(_Player.transform);
            }
            transform.position = TargetPos;
        }
    }

    public void SetFollowTarget(GameObject toFollow)
    {
        ToFollow = toFollow;
    }
}

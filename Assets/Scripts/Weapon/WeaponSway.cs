using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour {

    public float Intensity; //multipled input values - just how intesive the movement is going to be
    public float MaxSway; // a value to clamp the movement
    public float Smooth; //value used for Lerp

    private Vector3 InitialPosition;

    void Start()
    {
        InitialPosition = transform.localPosition;
    }
	
	void Update () {

        if (GameManager.GM.GetState() == GameManager.GameState.Playing)
        {
            float movementX = Input.GetAxis("Mouse X") * Intensity * -1; //multiply by -1 since the weapon sways in the opposite direction from the camera movement
            float movementY = Input.GetAxis("Mouse Y") * Intensity * -1;

            movementX = Mathf.Clamp(movementX, -MaxSway, MaxSway);
            movementY = Mathf.Clamp(movementY, -MaxSway, MaxSway);

            Vector3 SwayDirection = new Vector3(movementX, movementY, 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, InitialPosition + SwayDirection, Time.deltaTime * Smooth);
        }   
	}
}

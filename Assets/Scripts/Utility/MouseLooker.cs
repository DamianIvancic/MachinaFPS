using UnityEngine;
using System.Collections;

public class MouseLooker : MonoBehaviour {

	// Use this for initialization
	public float XSensitivity = 2.0f;
	public float YSensitivity = 2.0f;
    public bool smooth;
    public float smoothTime = 5.0f;

    public bool clampVerticalRotation = true;
	public float MinimumX = -90.0f;
	public float MaximumX = 90.0f;

	// internal private variables
	private Quaternion _characterTargetRot;
	private Quaternion _cameraTargetRot;
	private Transform _transform;
	private Transform _cameraTransform;

    public GameObject Weapon;

	void Start() {
		// start the game with the cursor locked
		LockCursor (true);

		// get a reference to the character's transform (which this script should be attached to)
		_transform = gameObject.transform;

		//Camera.main.transform
		//FindObjectWithTag("MainCamera").transform
		// get a reference to the main camera's transform
		_cameraTransform = Camera.main.transform;

		// get the location rotation of the character and the camera
		_characterTargetRot = _transform.localRotation;
		_cameraTargetRot = _cameraTransform.localRotation;
	}
	
	void Update() {

        if (GameManager.GM.GetState() == GameManager.GameState.Playing)
        {
            // rotate stuff based on the mouse
            LookRotation();

            // if ESCAPE key is pressed, then unlock the cursor
            if (Input.GetButtonDown("Cancel"))
            {
                LockCursor(false);
            }

            // if the player fires, then relock the cursor
            if (Input.GetButtonDown("Fire1"))
            {
                LockCursor(true);
            }
        }     
	}
	
	private void LockCursor(bool isLocked)
	{
		if (isLocked) 
		{
			// make the mouse pointer invisible
			Cursor.visible = false;

			// lock the mouse pointer within the game area
			Cursor.lockState = CursorLockMode.Locked;
		} else {
			// make the mouse pointer visible
			Cursor.visible = true;

			// unlock the mouse pointer so player can click on other windows
			Cursor.lockState = CursorLockMode.None;
		}
	}

	public void LookRotation()
	{
		//get the y and x rotation based on the Input manager
		float yRot = Input.GetAxis("Mouse X") * XSensitivity;
		float xRot = Input.GetAxis("Mouse Y") * YSensitivity;

		// calculate the rotation
		_characterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
		_cameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);

		// clamp the vertical rotation if specified
		if(clampVerticalRotation)
			_cameraTargetRot = ClampRotationAroundXAxis (_cameraTargetRot);

		// update the character and camera based on calculations
		if(smooth) // if smooth, then slerp over time
		{
			_transform.localRotation = Quaternion.Slerp (_transform.localRotation, _characterTargetRot,
			                                            smoothTime * Time.deltaTime);
			_cameraTransform.localRotation = Quaternion.Slerp (_cameraTransform.localRotation, _cameraTargetRot,
			                                         smoothTime * Time.deltaTime);
		}
		else // not smooth, so just jump
		{
			_transform.localRotation = _characterTargetRot;
			_cameraTransform.localRotation = _cameraTargetRot;
		}     
	}

	Quaternion ClampRotationAroundXAxis(Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;
		
		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);
		
		angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);
		
		q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);
		
		return q;
	}
}
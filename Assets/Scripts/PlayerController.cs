using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public float maxHP = 200f;
    public float MovementSpeed = 3.0f;

    private float _currentHP;	
	private CharacterController _characterController;
	private float _movementH;
	private float _movementV;
    
	void Awake()
	{
        _currentHP = maxHP;
	}

	void Update()
	{
        if(GameManager.GM.GetState() == GameManager.GameState.Playing)
        {
            GameManager.GM.UIManager.UpdateHPText(_currentHP);

            if (_currentHP <= 0f)
            {
                GameManager.GM.SetState(GameManager.GameState.GameOver);
                GameManager.GM.UIManager.DisplayEndingScreen("GAME OVER");
            }
            
            _movementH = Input.GetAxisRaw("Horizontal");
            _movementV = Input.GetAxisRaw("Vertical");

            Vector3 movementX = _movementH * Vector3.right;
            Vector3 movementZ = _movementV * Vector3.forward;

            Vector3 movement = movementX + movementZ;
            movement.Normalize();
            movement *= MovementSpeed * Time.deltaTime;
            movement = transform.TransformDirection(movement);

            transform.position += movement;
        }
	}

    public void ApplyDamage(float damage)
    {
        _currentHP -= damage;
    }
}

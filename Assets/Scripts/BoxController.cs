using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxController : MonoBehaviour {

    public float MaxHP = 25;
    public Image HealthBar;


    private float _currentHP;
    private bool _isBusted = false;
    private GameObject UIComponent;
    private GameObject _worker;
    private WorkerController _WCScript;


    void Start()
    {
        _currentHP = MaxHP;

        if(HealthBar)
        {
            Canvas UICanvas = HealthBar.GetComponentInParent<Canvas>();
            UIComponent = UICanvas.gameObject;
        }
    }

	void Update ()
    {
        if (GameManager.GM.GetState() == GameManager.GameState.Playing)
        {
            if (_worker)
                UpdateLifeStatus();
            else
                DropDown(); //the collider on the object is bigger than the object itself to make shooting it easier, so this is used instead of a rigidbody                     
        }
    }

    void UpdateLifeStatus()
    {
        if(HealthBar != null)
        {
            if (_currentHP > 0)
                HealthBar.fillAmount = _currentHP / MaxHP;
            else
                HealthBar.fillAmount = 0f;
        }

        if (_currentHP <= 0f)
        {
            GameManager.GM.ScoreManager.AddPlayerScore(); // ADD the value to the existing score     
            _WCScript.RedirectPath();
            DestroyCustom(15f);
        }
    }

    void DropDown()
    {
        Vector3 pos = transform.position;
        pos.y = 0.4f;
        transform.position = pos;
    }

    public void DestroyCustom()
    {
        _WCScript.SetCarryObject(null);
        _worker = null;
        if (gameObject.transform.parent != null)
            Destroy(gameObject.transform.parent.gameObject);
        else
            Destroy(gameObject);
    }

    public void DestroyCustom(float time)
    {
        Destroy(UIComponent);
        
        _WCScript.SetCarryObject(null);
        _worker = null;
        if (gameObject.transform.parent != null)
            Destroy(gameObject.transform.parent.gameObject, time);
        else
            Destroy(gameObject, time);
    }

    public void ApplyDamage(float dmg)
    {
        _currentHP -= dmg;
    }

    public void SetWorker(GameObject worker)
    {
        _worker = worker;
        if (_worker != null)
            _WCScript = _worker.GetComponent<WorkerController>();
        else
            _WCScript = null;
    }
}

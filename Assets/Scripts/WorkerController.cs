using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkerController : MonoBehaviour {


    public float MaxHP = 100;
    public Image HealthBar;
    public float Speed; // speed of movement
    public GameObject BoxPrefab; //prefab of the box which the worker "picks up" (spawns) when he gets to the pick up point
   
    private float _currentHP;
    private GameObject UIComponent; // the Healthbar's parent canvas(only needed so it can be destroyed immediately if using a destroy function with delay)
    private float _regenTimer = 0f;
    private int _speedLevel = 1;

    private GameObject _carryObject = null; //an instance of the box object, movement changes depending on whether or not the worker is carrying a box
    private BoxController _BCScript;
    private List<Transform> _pickupLocations = new List<Transform>(); //list of positions where to pick up boxes
    private Transform _storage;
    private Transform _midpoint;
    private Transform _dropoff;

    private GameObject _player;
    private Transform _target; //the target which the worker is currently moving towards  
    private Vector3 _directionToPlayer;
    private Vector3 _directionToTarget;
    
    private Animator _AC;
    private bool _flipped = false; //controls flipping orientation depending on whether the worker is going left or right

    void Start () {

        Speed = Random.Range(0.04f, 0.06f);

        _AC = GetComponent<Animator>();

        _player = GameObject.FindWithTag("Player");

        _currentHP = MaxHP;
        if (HealthBar)
        {
            Canvas UICanvas = HealthBar.GetComponentInParent<Canvas>();
            UIComponent = UICanvas.gameObject;
        }

        Transform[] Path = transform.parent.transform.parent.GetComponentsInChildren<Transform>();
        for(int i = 0; i<Path.Length; i++)
        {       
            if (Path[i].gameObject.tag == "Dropoff")
                _dropoff = Path[i];
            else if (Path[i].gameObject.tag == "Midpoint")
                _midpoint = Path[i];
            else if (Path[i].gameObject.tag == "Storage")
                _storage = Path[i];
            else if (Path[i].gameObject.tag == "Pickup")
                _pickupLocations.Add(Path[i]);
        }
        _target = _midpoint;   
	}
	
	void Update ()
    {
         if (GameManager.GM.GetState() == GameManager.GameState.Playing)
         {
             if (!_AC.GetBool("IsDead")) //run the code only if the worker is alive
             {
                UpdateTarget();
                UpdateSpeed();
                UpdateMovement();
                UpdateAnimator();
                RegenerateHP();            
                UpdateLifeStatus();
             }
             else
                GetComponent<Rigidbody>().velocity = Vector3.zero;

              transform.LookAt(_player.transform);
         }
         else //need to explicity disable animator/stop the rigidbody while the game is paused
         {
             _AC.enabled = false;
             GetComponent<Rigidbody>().velocity = Vector3.zero;
         }

    }

    void UpdateLifeStatus()
    {
        if (HealthBar != null)
        {
            if (_currentHP > 0)
                HealthBar.fillAmount = _currentHP / MaxHP;
            else
                HealthBar.fillAmount = 0f;
        }

        if (_currentHP <= 0)
        {        
            _AC.SetBool("IsDead", true);
            SpawnObject.AdjustSpawnedNumber("Worker", -1);

            GameObject Environment = GameObject.FindWithTag("Environment");
            SpawnObject[] Spawns = Environment.GetComponentsInChildren<SpawnObject>();
            for(int i =0; i<Spawns.Length; i++)
            {
                Spawns[i].SpawnWorker(1);                    
            }
            StartCoroutine(GameManager.GM.UIManager.TPC.SetText("Putin sends " + Spawns.Length + " more workers!"));
            DestroyCustom(15f);        
        }
    }

    void RegenerateHP()
    {
        if(_currentHP < MaxHP)
        {
            _regenTimer += Time.deltaTime;

            if(_regenTimer > 1f)
            {
                _currentHP++;
                _regenTimer = 0f;
            }
        }
    }

    void UpdateTarget() // go pick up a box if empty handed, deliver a box if it has been picked up
    {
        transform.LookAt(_target); // only needed here to adjust the box position relative to the direction the worker is heading in

        if (_carryObject == null)
            GetABox();
        else
        {
            AdjustBoxPosition(-0.5f);
            DeliverABox();
        }
    }

    void UpdateMovement()
    {
        /*take the vector from Worker to Player and the one from the Worker to the position he's moving towards, 
               so we can animate his movement correctly depending on the angle between them (sideways, forward movement, backward movement)*/
        _directionToPlayer = _player.transform.position;
        _directionToPlayer.y = transform.position.y;
        _directionToPlayer -= transform.position;
        _directionToTarget = _target.position;
        _directionToTarget.y = transform.position.y;
        _directionToTarget -= transform.position;
        _directionToTarget.Normalize();

        transform.position += _directionToTarget * Speed;
    }

    void UpdateSpeed()
    {
        if(GameManager.GM.ScoreManager.GetPlayerScore() >= 10 && _speedLevel == 1)
        {
            Speed += 0.01f;
            _speedLevel = 2;      
        }
        else if(GameManager.GM.ScoreManager.GetPlayerScore() >= 20 && _speedLevel == 2)
        {
            Speed += 0.01f;
            _speedLevel = 3;          
        }
    }

    void UpdateAnimator()
    {
        _AC.enabled = true;

        //flips Worker orientation depending on whether he's going left or right
        if (Vector3.SignedAngle(_directionToPlayer, _directionToTarget, Vector3.up) < 0 && !_flipped)
        {
            Vector3 Scale = transform.localScale;
            Scale.x *= -1;
            transform.localScale = Scale;
            _flipped = true;
        }
        else if (Vector3.SignedAngle(_directionToPlayer, _directionToTarget, Vector3.up) > 0 && _flipped)
        {
            Vector3 Scale = transform.localScale;
            Scale.x *= -1;
            transform.localScale = Scale;
            _flipped = false;
        }

        //updates the animator parameters and lets the logic in the transitions take care of the rest
        _AC.SetBool("IsMoving", true);
        _AC.SetFloat("AngleBetweenTargetAndPlayer", Vector3.Angle(_directionToPlayer, _directionToTarget));
    }

  

    void SpawnBox() //spawns a box from the prefab 
    {    
        if (BoxPrefab == null)
            Debug.Log("Error! No BoxPrefab to spawn.");
      
        _BCScript = Instantiate(BoxPrefab).GetComponentInChildren<BoxController>();
        _carryObject = _BCScript.gameObject;
        _BCScript.SetWorker(gameObject);

        //adjusts size of the box to fit the worker
        Vector3 Scale = new Vector3(0.5f, 0.5f, 0.5f);
        _carryObject.transform.localScale = Scale;
    }

    void AdjustBoxPosition(float Height) // adjusts the box's position relative to the player so that it makes sense visually
    {
        Vector3 BoxPosition = transform.position + transform.forward * 0.5f;
        Debug.DrawLine(transform.position, transform.position + transform.forward);
        BoxPosition.y += Height;
        _carryObject.transform.position = BoxPosition;    
    }

    void GetABox() // when the worker is empty handed, goes along the path to the storage where he "picks up" a box
    {    
        switch(_target.gameObject.tag)
        {
            case ("Midpoint"):
                Vector3 DistanceM = _target.position - transform.position;
                if (DistanceM.magnitude < 1f)
                    _target = _storage;
                break;
            case ("Storage"):
                Vector3 DistanceS = _target.position - transform.position;
                if (DistanceS.magnitude < 1f)
                {
                    int index = Random.Range(0, _pickupLocations.Count);
                    _target = _pickupLocations[index];
                }
                break;
            case ("Pickup"):
                Vector3 DistanceP = _target.position - transform.position;
                if (DistanceP.magnitude < 1f)
                {
                    SpawnBox();
                    _target = _storage;
                }
                break;
            default:
                break;
        }
    }

    void DeliverABox() // when a box has been picked up /spawned, the worker carries it along the path back to the start and if it gets there without being destroyed the workers get a point
    {
        switch(_target.gameObject.tag)
        {
            case ("Storage"):
                Vector3 DistanceS = _target.position - transform.position;
                if (DistanceS.magnitude < 1f)
                    _target = _midpoint;
                break;
            case ("Midpoint"):
                Vector3 DistanceM = _target.position - transform.position;
                if (DistanceM.magnitude < 1f)
                    _target = _dropoff;
                break;
            case ("Dropoff"):
                Vector3 DistanceD = _target.position - transform.position;
                if (DistanceD.magnitude < 1f)
                {
                    _BCScript.DestroyCustom();
                    GameManager.GM.ScoreManager.AdjustWorkerScore(+1);
                    _target = _midpoint;
                }
                break;
            default:
                break;
        }
    }

    public void DestroyCustom()
    {
        if (gameObject.transform.parent != null)
            Destroy(gameObject.transform.parent.gameObject);
        else
            Destroy(gameObject);

        if (_carryObject != null && _BCScript != null)
            _BCScript.DestroyCustom();

    }

    public void DestroyCustom(float time)
    {
        Destroy(UIComponent);

        if (gameObject.transform.parent != null)
            Destroy(gameObject.transform.parent.gameObject, time);
        else
            Destroy(gameObject, time);

        if (_carryObject != null && _BCScript != null)
            _BCScript.DestroyCustom(15f);
    }

    public void SetCarryObject(GameObject toCarry)
    {
        _carryObject = toCarry;
    }

    public void ApplyDamage(float damage)
    {
        _currentHP -= damage;
        if (_currentHP < 0)
            _currentHP = 0;      
    }

    public void RedirectPath() // called if the worker's box is destroyed before he successfully delivers it
    {
        if ((transform.position - _dropoff.position).magnitude > (_midpoint.position - _dropoff.position).magnitude)
            _target = _storage;
        else
            _target = _midpoint;
    }
}

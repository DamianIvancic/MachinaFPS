using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoldierController : MonoBehaviour //alot of the functionality from the weapon script is replicated here since npc soldiers don't actually have a weapon object
{

    public float MaxHP = 100;
    public Image HealthBar;
    public float Speed = 0.05f;

    public int MaxAmmo = 25;
    public float Damage = 10;
    public float Range = 500.0f; //weapon range
    public Transform FiringPoint; //the object from which's position the firing is projected

    private float _currentHP;
    private GameObject UIComponent;
    private bool _isAlive = true;

    private AudioSource _firingSound;
    private ParticleSystem _muzzleFlash;
    private int _currentAmmo;
    private float _fireRate = 0.1f;
    private float _fireTimer = 0.0f;
    private bool _weaponEnabled;

    private NationalSecurity _KGB;
    private GameObject _player;
    private GameObject _spawnPosition;
    private GameObject _target;
    private Vector3 _directionToTarget;

    // Use this for initialization
    void Start()
    {
        _currentHP = MaxHP;
        if (HealthBar)
        {
            Canvas UICanvas = HealthBar.GetComponentInParent<Canvas>();
            UIComponent = UICanvas.gameObject;
        }

        _currentAmmo = MaxAmmo;
        _firingSound = GetComponent<AudioSource>();
        _muzzleFlash = GetComponentInChildren<ParticleSystem>();

        _KGB = GetComponentInParent<NationalSecurity>();
        _KGB._agentCount++;

        _player = GameObject.FindWithTag("Player");
        _spawnPosition = gameObject.transform.parent.gameObject.transform.parent.gameObject; //inception

        if (_spawnPosition == null)
            Debug.Log("No Spawn!");
    }

    void Update()
    {
        if (GameManager.GM.GetState() == GameManager.GameState.Playing)
        {
            UpdateLifeStatus();

            if (_isAlive)
            {
                if (!_weaponEnabled)
                    SetWeaponEnabled(true);

                if (_fireTimer < _fireRate)
                    _fireTimer += Time.deltaTime;

                UpdateTarget();

                if (_target == _player)
                    PursuePlayer();
                else if (_target == _spawnPosition)
                    StayOnGuard();
            }
        }
        else
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;

            if (_weaponEnabled)
                SetWeaponEnabled(false);     
        }
    }

    void SetWeaponEnabled(bool enabled)
    {
        var emission = _muzzleFlash.emission;
        emission.enabled = enabled;
        _firingSound.enabled = enabled;

        _weaponEnabled = enabled;
    }

    void UpdateLifeStatus()
    {
        if (_isAlive)
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
                DestroyCustom(15f);
                _isAlive = false;
            }
        }
    }

    void UpdateTarget()
    {
        if ((_player.transform.position - _spawnPosition.transform.position).magnitude < 30f)
            _target = _player;
        else
            _target = _spawnPosition;

        transform.LookAt(_player.transform);

        _directionToTarget = _target.transform.position;
        _directionToTarget.y = transform.position.y;
        _directionToTarget -= transform.position;
    }

    void PursuePlayer()
    {
        CheckCurrentAmmo();
        Fire();

        if (_directionToTarget.magnitude > 5f)
        {
            _directionToTarget.Normalize();
            transform.position += _directionToTarget * Speed;
        }
    }

    void StayOnGuard()
    {
        CheckCurrentAmmo();

        if (_directionToTarget.magnitude > 1f)
        {
            _directionToTarget.Normalize();
            transform.position += _directionToTarget * Speed;
        }
    }

    void Fire()
    {
        if (_fireTimer < _fireRate)
            return;

        if (_currentAmmo != 0)
        {
            _muzzleFlash.Play();

            if (!(_firingSound.isPlaying))
                _firingSound.Play();

            FireRaycast();

            _currentAmmo--;
            _fireTimer = 0.0f;
        }
    }

    void FireRaycast()
    {
        RaycastHit Hit;

        if (Physics.Raycast(FiringPoint.position, FiringPoint.forward, out Hit, Range))
        {
            if (Hit.transform.gameObject.tag == "Player")
            {
                PlayerController Script = Hit.transform.gameObject.GetComponent<PlayerController>();
                Script.ApplyDamage(Damage);
            }
        }
    }

    void CheckCurrentAmmo()
    {
        if (_currentAmmo == 0)
        {
            _muzzleFlash.Stop();
            _firingSound.Stop();

            Invoke("Reload", 5f);
        }
    }

    void Reload()
    {
        _currentAmmo = MaxAmmo;
        CancelInvoke();
    }

    public void DestroyCustom()
    {
        _KGB._agentCount--;

        var emission = _muzzleFlash.emission;
        emission.enabled = false;
        _firingSound.enabled = false;

        transform.Rotate(-90, 0, 0);

        if (gameObject.transform.parent != null)
            Destroy(gameObject.transform.parent.gameObject);
        else
            Destroy(gameObject);
    }

    public void DestroyCustom(float time)
    {
        _KGB._agentCount--;

        var emission = _muzzleFlash.emission;
        emission.enabled = false;
        _firingSound.enabled = false;
        Destroy(UIComponent);

        transform.Rotate(-90, 0, 0);

        if (gameObject.transform.parent != null)
            Destroy(gameObject.transform.parent.gameObject, time);
        else
            Destroy(gameObject, time);
    }

    public void ApplyDamage(float damage)
    {
        _currentHP -= damage;
    }
}

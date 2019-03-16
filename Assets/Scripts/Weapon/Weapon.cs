using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour {

    public int MaxAmmo = 25;
    public float Damage = 10;
    public float Range = 500.0f; //weapon range
    public Transform FiringPoint; //the object from which's position the firing is projected
    
    public NationalSecurity _KGB; //reference to object that spawns guards if Putin is under attack
    public GameObject SparksEffect; //particles that spawn upon bullets hitting a surface
    public GameObject BulletHoles; //bullet holes that spawn on a surface

    private Animator _weaponAnim;
    private AudioSource _firingSound;
    private ParticleSystem _muzzleFlash;

    private int _currentAmmo;
    private float _fireRate = 0.1f;
    private float _fireTimer = 0.0f;
    private bool _enabled; //used to stop animation, particle effects and sounds during start etc.

    void Start()
    {
        _currentAmmo = MaxAmmo;

        _weaponAnim = GetComponent<Animator>();       
        _firingSound = GetComponent<AudioSource>();
        _muzzleFlash = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (GameManager.GM.GetState() == GameManager.GameState.Playing)
        {
            if (!_enabled)
                SetEnabled(true);
                     
            if (_fireTimer < _fireRate)
                _fireTimer += Time.deltaTime;

            CheckCurrentAmmo();
            NationalSecurityRaycast(); // a raycast that activates the alarm when aiming at putin

            if (_weaponAnim != null && !(_weaponAnim.GetBool("Reload")))
                Fire();
        }
        else if (_enabled)
            SetEnabled(false);
    }

    void SetEnabled(bool enabled)
    {
        _weaponAnim.enabled = enabled;
        var emission = _muzzleFlash.emission;
        emission.enabled = enabled;
        _firingSound.enabled = enabled;

        _enabled = enabled;
    }


    void CheckCurrentAmmo()
    {
        GameManager.GM.UIManager.UpdateAmmoDisplay(_currentAmmo, MaxAmmo);

        if (_currentAmmo == 0 || Input.GetKeyUp(KeyCode.R) && _weaponAnim.GetBool("Reload") == false)
        {
            if(_weaponAnim != null)
                _weaponAnim.SetBool("Reload", true);

            _muzzleFlash.Stop();
            _firingSound.Stop();
        }
    }


    void Fire() 
    {
        if (Input.GetButtonUp("Fire1"))
        {
            _weaponAnim.SetBool("Fire", false);
            _muzzleFlash.Stop();
            _firingSound.Stop();
        }

        if (_fireTimer < _fireRate)
            return;

        if(Input.GetButton("Fire1"))
        {
            _weaponAnim.SetBool("Fire", true);
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
            switch(Hit.transform.gameObject.tag)
            {
                case ("Worker"):
                    WorkerController WCScript = Hit.transform.gameObject.GetComponent<WorkerController>();
                    WCScript.ApplyDamage(Damage);
                    break;
                case ("Box"):
                    BoxController BScript = Hit.transform.gameObject.GetComponent<BoxController>();
                    BScript.ApplyDamage(Damage);
                    break;
                case ("Soldier"):
                    SoldierController SScript = Hit.transform.gameObject.GetComponent<SoldierController>();
                    SScript.ApplyDamage(Damage);
                    break;
                case ("Putin"):
                    _KGB.SpawnGuards();
                    break;
                default:
                    GameObject FiringSparks = Instantiate(SparksEffect, Hit.point, Quaternion.FromToRotation(Vector3.up, Hit.normal));
                    FiringSparks.transform.SetParent(Hit.transform);
                    GameObject SpawnedHole = Instantiate(BulletHoles, Hit.point, Quaternion.FromToRotation(Vector3.forward, Hit.normal));
                    SpawnedHole.transform.SetParent(Hit.transform);
                    Destroy(FiringSparks, 1f);
                    Destroy(SpawnedHole, 15f);
                    break;
            }
        }
    }

    void NationalSecurityRaycast()
    {
        RaycastHit Hit;

        if (Physics.Raycast(FiringPoint.position, FiringPoint.forward, out Hit, 15f))
        {
            if (Hit.transform.gameObject.tag == "Putin" && _KGB._spawned == false)         
                GameManager.GM.EffectsManager.SetAlarmEffect(true);                    
        }
    }

    public void Reload()
    {
        _currentAmmo = MaxAmmo;
    }

    public int GetCurrentAmmo()
    {
        return _currentAmmo;
    }
}

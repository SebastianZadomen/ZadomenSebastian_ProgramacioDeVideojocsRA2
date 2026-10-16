using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.InputSystem;
public class CanonRotation : MonoBehaviour,InputSystem_Actions.ICanonActions
{
    
    public Vector3 _maxRotation;
    public Vector3 _minRotation;
    private float _offset = -51.6f;
    public GameObject PivotPosition;
    public GameObject ShootPoint;
    public GameObject Bullet;
    public float ProjectileSpeed = 0;
    public float MaxSpeed;
    public float MinSpeed;
    public GameObject PotencyBar;
    private float _initialScaleX;
    private Vector2 _distanceBetweenMouseAndPlayer;
    private bool isRaising = false;
    [SerializeField] private float _multiplier = 10f;
    private InputSystem_Actions inputSystem;
  
    private void Awake()
    {
        _initialScaleX = PotencyBar.transform.localScale.x;
        inputSystem = new InputSystem_Actions();
        inputSystem.Canon.SetCallbacks(this);
        
    }
    void Update()
    {
   
        if (isRaising)
        {
            ProjectileSpeed = Time.deltaTime * _multiplier + ProjectileSpeed;
            ProjectileSpeed = Math.Clamp(ProjectileSpeed, MinSpeed, MaxSpeed);
            CalculateBarScale();
        }
        
        CalculateBarScale();

    }
    public void CalculateBarScale()
    {
        PotencyBar.transform.localScale = new Vector3(Mathf.Lerp(0, _initialScaleX, ProjectileSpeed / MaxSpeed),
            transform.localScale.y,
            transform.localScale.z);
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = context.ReadValue<Vector2>();
        Vector2 screenToPosition = Camera.main.ScreenToWorldPoint(mousePosition);
		_distanceBetweenMouseAndPlayer = screenToPosition - (Vector2)ShootPoint.transform.position;

    
        
		float angle = Mathf.Atan2(_distanceBetweenMouseAndPlayer.y, _distanceBetweenMouseAndPlayer.x) * Mathf.Rad2Deg;
        float angleFinal = Math.Clamp(angle, (float)_minRotation.z , (float)_maxRotation.z);
        PivotPosition.transform.rotation = Quaternion.Euler(0f, 0f, angleFinal + _offset);

	}

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isRaising = true;
           
        }
        if (context.canceled)
        {
            var projectile = Instantiate(Bullet, ShootPoint.transform.position, Quaternion.identity); //canviar la posició on s'instancia
            projectile.GetComponent<Rigidbody2D>().linearVelocity = _distanceBetweenMouseAndPlayer * ProjectileSpeed;
            ProjectileSpeed = 0f;
            isRaising = false;
        }
    }
    private void OnEnable()
    {
        inputSystem.Enable();
        
    }
    private void OnDisable()
    {
        inputSystem.Disable();
    }

}

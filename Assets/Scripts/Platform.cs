using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Platform : MonoBehaviour
{
    private Transform _transform;
    private Rigidbody2D _rb;
    private BouncyBall _ball;
    [SerializeField]
    private BouncyBall _ballToSpawn;
    [SerializeField]
    private GlobalGameManager _globalGameManager;

    private DefaultInputActions _input;
    private SpriteRenderer _renderer;
    private BoxCollider2D _collider;

    public float scale = 3;
    private bool _isActive;
    
    private void Start()
    {
        _transform = GetComponent<Transform>();
        _rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
        
        _input = new DefaultInputActions();
        _input.Disable();
        _input.PlatformInputMap.SpawnBall.canceled += ReloadBall;
        _input.PlatformInputMap.Test.performed += Test;
        
        _isActive = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_isActive && FindObjectOfType<BouncyBall>() == null)
        {
            SpawnBall();
        }
    }
    
    private void FixedUpdate()
    {
        if (_isActive)
        {
            if (Camera.main != null)
            {
                var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _rb.velocity = new Vector2(mouseWorldPos.x - _transform.position.x, 0);
                _rb.velocity /= Time.fixedDeltaTime;
            }

            if (_ball != null)
            {
                _ball.transform.position = new Vector3(_transform.position.x, _transform.position.y + 20, 0);
                //_ball.SetVelocity(_rb.velocity);
            }

            Vector2 newVal = Vector2.Lerp(_renderer.size, new Vector2(scale * 32, 16), 0.1f);

            if (Mathf.Abs(newVal.x - scale * 32) < 1.0)
            {
                newVal.x = scale * 32;
            }

            _renderer.size = newVal;
            _collider.size = newVal;
        }

    }

    private void ReloadBall(InputAction.CallbackContext ctx)
    {
        if (_ball != null)
        {
            ReleaseBall();
        }
    }

    public void SpawnBall()
    {
        Vector3 spawnPos = new Vector3(_transform.position.x, _transform.position.y + 20, 0);
        Quaternion spawnRot = Quaternion.identity;
        _ball = Instantiate(_ballToSpawn, spawnPos, spawnRot);
        _ball.globalGameManager = _globalGameManager;
    }

    public void ReleaseBall()
    {
        if (_ball != null)
        {
            _ball.ReleaseFromPlatform();
            _ball = null;
        }
    }

    public void Test(InputAction.CallbackContext ctx)
    {
        if (scale == 3)
            scale = 1;
        else
        {
            scale = 3;
        }
    }

    public void OnGamePause()
    {
        _input.Disable();
        _isActive = false;
    }

    public void OnGameUnpause()
    {
        _input.Enable();
        _isActive = true;
    }
    
}

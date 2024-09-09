using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using Random = UnityEngine.Random;

public class BouncyBall : MonoBehaviour
{
    private Rigidbody2D _rb;
    
    [SerializeField]
    private float _startingSpeed;
    public GlobalGameManager globalGameManager;
    private Vector2 velocityBuffer;
    private float velocityMax;
    private float velocityMultiplier;

    public Vector2 Velocity
    {
        get => _rb.velocity;
        private set => _rb.velocity = value;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        velocityMax = _startingSpeed;
        velocityMultiplier = 1.05f;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        switch (collision.gameObject.name)
        {
            case "DestructibleBlock":
                DestructibleBlock block = collision.gameObject.GetComponent<DestructibleBlock>();
                Destroy(collision.gameObject);
                globalGameManager.OnBlockDestroyed();
                velocityMax *= velocityMultiplier;

                if (block.myBonus == 1)
                {
                    Platform platform = FindObjectOfType<Platform>();
                    platform.scale = Math.Clamp(platform.scale - 2, 1, 5);
                }
                else if (block.myBonus == 2)
                {
                    Platform platform = FindObjectOfType<Platform>();
                    platform.scale = Math.Clamp(platform.scale + 2, 1, 5);
                }      
                break;
        }
        Velocity = Velocity.normalized * velocityMax;
    }
    
    void Update()
    {
        if (transform.position.y < -192 || transform.position.y > 192 || transform.position.x < -256 || transform.position.x > 256)
        {
            Destroy(gameObject);
            globalGameManager.LooseLife();
        }
    }

    public void ReleaseFromPlatform()
    {
        float angle = Random.Range(45f, 135f) * Mathf.Deg2Rad;
        float x = Mathf.Cos(angle) * _startingSpeed;
        float y = Mathf.Sin(angle) * _startingSpeed;
        _rb.velocity = new Vector2(x, y);
    }

    public void SetVelocity(Vector2 velocity)
    {
        _rb.velocity = velocity;
    }

    public void OnGamePause()
    {
        velocityBuffer = Velocity;
        Velocity = Vector2.zero;
    }
    
    public void OnGameUnpause()
    {
        Velocity = velocityBuffer;
        velocityBuffer = Vector2.zero;
    }
    
}

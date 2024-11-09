using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class KernelProjectile : MonoBehaviour
{
    private const byte LAYER_GROUND = 6;
    
    private const byte ACTION_JUMP = 0;
    private const byte ACTION_DASH = 1;
    
    private const float KERNEL_SPEED = 15f;
    private const float KERNEL_PLACEMENT_BOUNCE_SPEED = 2f;

    private const float PROJECTILE_LIFETIME = 5f;
    
    public byte actionType;
    public bool isFacingRight;

    private float _lifetimeTimer = 0f;

    [SerializeField] private Rigidbody2D _rigidbody2D;

    [SerializeField] private GameObject _kernelPlacedObject;
    
    void Start()
    {
        switch (actionType)
        {
            case ACTION_JUMP:
            {
                float xVelocity = Random.Range(-KERNEL_SPEED / 2f, KERNEL_SPEED / 2f);
                
                _rigidbody2D.velocity = new Vector2(xVelocity, -KERNEL_SPEED);

                break;
            }
            case ACTION_DASH:
            {
                float xVelocity = KERNEL_SPEED * (isFacingRight ? -1f : 1f);
                
                
                break;
            }
            default:
                Debug.LogWarning("invalid action type passed to kernel object");
                Destroy(gameObject);
                return;
        }
    }
    
    void Update()
    {
        _lifetimeTimer += Time.deltaTime;
        
        if(_lifetimeTimer >= PROJECTILE_LIFETIME)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LAYER_GROUND)
        {
            GameObject kernelPlaced = GameObject.Instantiate(_kernelPlacedObject);
            Rigidbody2D kprb2D = kernelPlaced.GetComponent<Rigidbody2D>();

            kernelPlaced.transform.position = gameObject.transform.position;

            kprb2D.velocity = new Vector2(0f, KERNEL_PLACEMENT_BOUNCE_SPEED);
            
            Destroy(gameObject);
        }
    }
}

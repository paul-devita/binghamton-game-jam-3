using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{

    // CONSTANTS
    private const float GROUNDCHECK_RADIUS = 0.2f;

    private const ushort DEFAULT_KERNEL_COUNT = 1000;
    private const float DOUBLE_JUMP_KERNEL_MULTIPLIER = 0.275f;
    private const float DASH_POWER_KERNEL_MULTIPLIER = 0.25f;
    private const float DASH_TIME_KERNEL_MULTIPLIER = 0.1f;
    
    private const float JUMPING_POWER = 24f;
    private const float DOUBLE_JUMP_BASE_POWER = 15f;
    
    private const float SPEED = 14f;

    private const float DASH_POWER = 25f;
    private const float DASH_COOLDOWN = 0f;
    private const float DASH_TIME = 0.3f;

    private const ushort MAX_KERNEL_USAGE = 3;
    private const ushort MIN_KERNEL_USAGE = 1;

    private const byte ACTION_JUMP = 0;
    private const byte ACTION_DASH = 1;
    
    private const string PICKUP_KERNELS_TAG = "pickupKernel";

    
    // general vars
    private float _horizontalInput;

    private bool _isFacingRight = true;
    
    private bool _canDoubleJump = true;

    private bool _isDashing = false;
    private bool _canDash = true;
    
    private bool _isStationary = true;


    // serialize fields
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Animator _animator;

    [SerializeField] private Transform _kernalProjectileSpawnLocation;
    [SerializeField] private GameObject _kernelProjectile;
    [SerializeField] private LayerMask _placedObjectsLayer;

    [SerializeField] private int _kernelCount = 12;
    [SerializeField] private short _kernelsInUse;
    

    //Accessors
    public int getKernelCount()
    {
        return _kernelCount;
    }

    public short getKernelsInUse()
    {
        return _kernelsInUse;
    }
    
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    
    void Update()
    {
        if (_isDashing) return;
        
        //Get Horizontal Input
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        
        //Change Kernel Usage
        if (Input.GetKeyDown(KeyCode.E) && _kernelsInUse < MAX_KERNEL_USAGE)
        {
            _kernelsInUse++;
        }
        else if (Input.GetKeyDown(KeyCode.Q) && _kernelsInUse > MIN_KERNEL_USAGE)
        {
            _kernelsInUse--;
        }

        //Jumping Behavior
        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, JUMPING_POWER);
            _canDoubleJump = true;

            _animator.SetBool("is jumping", true);
        }
        if (Input.GetButtonUp("Jump") && _rigidbody2D.velocity.y > 0f)
        {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _rigidbody2D.velocity.y * 0.5f);
        }
        
        //Double Jump Behavior
        if (Input.GetButtonDown("Jump") && !isGrounded() && _canDoubleJump && _kernelCount > _kernelsInUse)
        {
            _animator.SetBool("is surprised", true);

            _canDoubleJump = false;

            _kernelCount -= _kernelsInUse;
            
            for(int i = 0; i < _kernelsInUse; i++)
                createKernelProjectile(ACTION_JUMP, _isFacingRight);

            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x,
                DOUBLE_JUMP_BASE_POWER * (1f + (_kernelsInUse - 1) * DOUBLE_JUMP_KERNEL_MULTIPLIER));
        }

        //Dashing Behavior
        if (Input.GetKeyDown(KeyCode.LeftShift) && _canDash && !isGrounded() && _kernelCount > _kernelsInUse)
        {
            _animator.SetBool("is surprised", true);
            StartCoroutine(dash());
        }
        
        changeDirection();
    }


    // physics calculation calls (called at regular interval)
    private void FixedUpdate()
    {
        if (_isDashing) return;
        
        _rigidbody2D.velocity = new Vector2(_horizontalInput * SPEED, _rigidbody2D.velocity.y);
        
        _animator.SetFloat("x velocity", Math.Abs(_rigidbody2D.velocity.x));
        _animator.SetFloat("y velocity", _rigidbody2D.velocity.y);
    }

    // returns if the player is grounded or not based on hitbox
    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, GROUNDCHECK_RADIUS, _groundLayer);
    }
    
    // flips the player direction based on user input (also flips sprites)
    private void changeDirection()
    {
        if (_isFacingRight && _horizontalInput < 0f || !_isFacingRight && _horizontalInput > 0f)
        {
            _isFacingRight = !_isFacingRight;
            _isStationary = false;

            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    // creates a kernel projectile with an action and direction
    private void createKernelProjectile(byte action, bool isFacingRight)
    {
        GameObject kernel = GameObject.Instantiate(_kernelProjectile);

        kernel.transform.position = _kernalProjectileSpawnLocation.position;

        KernelProjectile kernelProjectile = kernel.GetComponent<KernelProjectile>();

        kernelProjectile.isFacingRight = isFacingRight;
        kernelProjectile.actionType = action;
    }

    // player dash. rotates player and shoots kernals
    private IEnumerator dash()
    {
        float oldGravity = _rigidbody2D.gravityScale;
        float dashDuration = DASH_TIME * (1 + (_kernelsInUse - 1) * DASH_TIME_KERNEL_MULTIPLIER);
        
        _canDash = false;
        _isDashing = true;

        _kernelCount -= _kernelsInUse;
        
        for(int i = 0; i < _kernelsInUse; i++)
            createKernelProjectile(ACTION_DASH, _isFacingRight);
        
        _rigidbody2D.gravityScale = 0f;
        _rigidbody2D.velocity = new Vector2(transform.localScale.x * (DASH_POWER * (1 + (_kernelsInUse - 1) * DASH_POWER_KERNEL_MULTIPLIER)), 0f);

        StartCoroutine(dashRotation(dashDuration));
        
        yield return new WaitForSeconds(dashDuration);

        _isDashing = false;
        _rigidbody2D.gravityScale = oldGravity;
        transform.eulerAngles = Vector3.zero;
        yield return new WaitForSeconds(DASH_COOLDOWN);

        _canDash = true;
    }


    // called when player trigger collides with an object. Player trigger is currently the capsule 2D collider
    void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.layer == (int) Mathf.Log(_groundLayer, 2f))
        {
            _animator.SetBool("is jumping", false);
            _animator.SetBool("is surprised", false);
        }
        else if(collision.gameObject.layer == (int) Mathf.Log(_placedObjectsLayer, 2f))
        {
            switch (collision.gameObject.tag)
            {
                case PICKUP_KERNELS_TAG:
                {
                    GameObject obj = collision.gameObject;
                    
                    if(_kernelCount < DEFAULT_KERNEL_COUNT)
                        _kernelCount++;

                    Destroy(obj);
                    
                    break;
                }
            }
        }
    }

    // rotates the player during the dash animation
    private IEnumerator dashRotation(float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float newZ = Mathf.Lerp(0, 360, t / duration);

            transform.eulerAngles = new Vector3(transform.localRotation.x, transform.localRotation.y,
                newZ * (_isFacingRight ? -1 : 1));

            yield return null;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{

    // CONSTANTS
    private const float GROUNDCHECK_RADIUS = 0.2f;

    private const ushort DEFAULT_KERNEL_COUNT = 12;
    private const float DOUBLE_JUMP_KERNEL_MULTIPLIER = 0.275f;
    private const float DASH_POWER_KERNEL_MULTIPLIER = 0.25f;
    private const float DASH_TIME_KERNEL_MULTIPLIER = 0.1f;
    
    private const float JUMPING_POWER = 24f;
    private const float DOUBLE_JUMP_BASE_POWER = 24f;
    
    private const float NORMAL_SPEED = 10f;
    private const float DOUBLE_JUMP_SPEED = NORMAL_SPEED / 2;
    private float SPEED = NORMAL_SPEED;

    private const float DASH_POWER = 30f;
    private const float DASH_COOLDOWN = 0f;
    private const float DASH_TIME = 0.5f;

    private const ushort MAX_KERNEL_USAGE = 3;
    private const ushort MIN_KERNEL_USAGE = 1;

    private const byte ACTION_JUMP = 0;
    private const byte ACTION_DASH = 1;
    
    private const string PICKUP_KERNELS_TAG = "pickupKernel";

    private const float LOCUST_DAMAGE_TIME = 4f;

    
    // general vars
    private float _horizontalInput;

    private float _locustTimer;

    private bool _isFacingRight = true;
    
    private bool _canDoubleJump = true;

    private bool _isDashing = false;
    private bool _canDash = true;

    private bool _isDead;


    // serialize fields
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Animator _animator;

    [SerializeField] private Transform _kernalProjectileSpawnLocation;
    [SerializeField] private GameObject _kernelProjectile;
    [SerializeField] private LayerMask _placedObjectsLayer;

    [SerializeField] private LayerMask _locustsLayer;
    [SerializeField] private UI _ui;

    [SerializeField] private GameObject _deathScreen;

    [SerializeField] private int _kernelCount;
    [SerializeField] private short _kernelsInUse;
    

    //Accessors
    public int getKernelCount() {
        return _kernelCount;
    }
    public short getKernelsInUse() {
        return _kernelsInUse;
    }


    void Start()
    {
        _kernelCount = DEFAULT_KERNEL_COUNT;
        _kernelsInUse = 1;
        _animator = GetComponent<Animator>();
    }

    
    void Update()
    {
        if (_isDead) return;

        if (_kernelCount <= 0)
        {
            _isDead = true;
            StartCoroutine(deathRoutine());
        }

        if (_isDashing) return;
        
        //Get Horizontal Input
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        
        //Change Kernel Usage
        if (Input.GetKeyDown(KeyCode.E) && _kernelsInUse < MAX_KERNEL_USAGE)
        {
            _kernelsInUse++;
            
            _ui.updateKernelUsage(_kernelsInUse);
        }
        else if (Input.GetKeyDown(KeyCode.Q) && _kernelsInUse > MIN_KERNEL_USAGE)
        {
            _kernelsInUse--;
            
            _ui.updateKernelUsage(_kernelsInUse);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            _kernelCount--;
            
            _ui.takeDamage(_kernelCount);
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

            if (_rigidbody2D.velocity.x < 0) {
                _rigidbody2D.velocity = new Vector2(-DOUBLE_JUMP_SPEED, _rigidbody2D.velocity.y);
            } else {
                _rigidbody2D.velocity = new Vector2(DOUBLE_JUMP_SPEED, _rigidbody2D.velocity.y);
            }
            SPEED = DOUBLE_JUMP_SPEED;

            _canDoubleJump = false;

            _kernelCount -= _kernelsInUse;
            
            _ui.updateKernelCount(_kernelCount);
            
            for(int i = 0; i < _kernelsInUse; i++)
                createKernelProjectile(ACTION_JUMP, _isFacingRight);
            

            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x,
                DOUBLE_JUMP_BASE_POWER * (1f + (_kernelsInUse - 1) * DOUBLE_JUMP_KERNEL_MULTIPLIER));
        }

        //Dashing Behavior
        if (Input.GetKeyDown(KeyCode.LeftShift) && _canDash && !isGrounded() && _kernelCount > _kernelsInUse)
        {
            _animator.SetBool("is surprised", true);
            
            _kernelCount -= _kernelsInUse;
        
            _ui.updateKernelCount(_kernelCount);
            
            StartCoroutine(dash());
        }
        
        changeDirection();
    }


    // physics calculation calls (called at regular interval)
    private void FixedUpdate()
    {
        if (_isDead) return;
        if (_isDashing) return;
        
        if (!isGrounded() && Math.Abs(_rigidbody2D.velocity.x) <= SPEED && _horizontalInput != 0)
        {
            float resistance = 0.5f;
            float velocity = _rigidbody2D.velocity.x + (_horizontalInput * resistance);
            if (Math.Abs(velocity) > SPEED)
            {
                velocity = _horizontalInput * SPEED;
            }
            _rigidbody2D.velocity = new Vector2(velocity, _rigidbody2D.velocity.y);
        }
        else if(isGrounded())
        {
            _rigidbody2D.velocity = new Vector2(_horizontalInput * SPEED, _rigidbody2D.velocity.y);
        }
        
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
        
        for(int i = 0; i < _kernelsInUse; i++)
            createKernelProjectile(ACTION_DASH, _isFacingRight);
        
        _rigidbody2D.gravityScale = 0f;
        _rigidbody2D.velocity = new Vector2(transform.localScale.x * (DASH_POWER * (1 + (_kernelsInUse - 1) * DASH_POWER_KERNEL_MULTIPLIER)), 0f);

        StartCoroutine(dashRotation(dashDuration));
        
        yield return new WaitForSeconds(dashDuration);

        _isDashing = false;
        _rigidbody2D.gravityScale = oldGravity;
        transform.eulerAngles = Vector3.zero;

        if (_rigidbody2D.velocity.x < 0) {
            _rigidbody2D.velocity = new Vector2(-NORMAL_SPEED, _rigidbody2D.velocity.y);
        } else {
            _rigidbody2D.velocity = new Vector2(NORMAL_SPEED, _rigidbody2D.velocity.y);
        }

        yield return new WaitForSeconds(DASH_COOLDOWN);

        _canDash = true;
    }


    // called when player trigger collides with an object. Player trigger is currently the capsule 2D collider
    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.layer == (int) Mathf.Log(_groundLayer, 2f))
        {
            if (_rigidbody2D.velocity.y <= 0)
            {
                _animator.SetBool("is jumping", false);
                _animator.SetBool("is surprised", false);
                SPEED = NORMAL_SPEED;
            }
        }
        else if(other.gameObject.layer == (int) Mathf.Log(_placedObjectsLayer, 2f))
        {
            switch (other.gameObject.tag)
            {
                case PICKUP_KERNELS_TAG:
                {
                    GameObject obj = other.gameObject;

                    if (_kernelCount < DEFAULT_KERNEL_COUNT)
                    {
                        _kernelCount++;
                        _ui.updateKernelCount(_kernelCount);
                    }

                    Destroy(obj);
                    
                    break;
                }
            }
        }
        else if (other.gameObject.layer == (int)Mathf.Log(_locustsLayer, 2f))
        {
            _locustTimer = 0f;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == (int)Mathf.Log(_locustsLayer, 2f))
        {
            Debug.LogWarning(_locustTimer);
            
            _locustTimer += Time.deltaTime;

            if (_locustTimer >= LOCUST_DAMAGE_TIME)
            {
                _locustTimer = 0f;

                _kernelCount--;

                _ui.takeDamage(_kernelCount);
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

    private IEnumerator deathRoutine()
    {
        const int MASK_INDEX = 1;
        
        const float P1_DURATION = 0.5f;
        const float P2_DURATION = 0.25f;

        const float SUSPEND_DURATION = 1f;

        const float P1_END_SIZE = 30f;

        //Stop the player movement
        _rigidbody2D.gravityScale = 0f;
        _rigidbody2D.velocity = Vector2.zero;
        
        //Play Animation

        GameObject deathScreen = Instantiate(_deathScreen, gameObject.transform);

        deathScreen.transform.position = gameObject.transform.position;

        Transform maskTransform = deathScreen.transform.GetChild(MASK_INDEX).transform;

        float initScale = maskTransform.localScale.x;

        for (float t = 0; t < P1_DURATION; t += Time.deltaTime)
        {
            float maskScale = Mathf.Lerp(initScale, P1_END_SIZE, t / P1_DURATION);

            maskTransform.localScale = new Vector3(maskScale, maskScale, 0f);

            yield return null;
        }

        yield return new WaitForSeconds(SUSPEND_DURATION);
        
        initScale = maskTransform.localScale.x;

        for (float t = 0f; t < P2_DURATION; t += Time.deltaTime)
        {
            float maskScale = Mathf.Lerp(initScale, 0f, t / P2_DURATION);

            maskTransform.localScale = new Vector3(maskScale, maskScale, 0f);
            
            yield return null;
        }
        
        Destroy(maskTransform.gameObject);
    }
}

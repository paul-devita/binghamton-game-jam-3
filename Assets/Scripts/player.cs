using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    private const float GROUNDCHECK_RADIUS = 0.2f;

    private const ushort DEFAULT_KERNEL_COUNT = 12;
    private const float DOUBLE_JUMP_KERNEL_MULTIPLIER = 0.275f;
    private const float DASH_POWER_KERNEL_MULTIPLIER = 0.25f;
    private const float DASH_TIME_KERNEL_MULTIPLIER = 0.1f;
    
    private const float JUMPING_POWER = 16f;
    private const float DOUBLE_JUMP_BASE_POWER = 10f;
    
    private const float SPEED = 8f;

    private const float DASH_POWER = 24f;
    private const float DASH_COOLDOWN = 2f;
    private const float DASH_TIME = 0.3f;

    private const ushort MAX_KERNEL_USAGE = 3;
    private const ushort MIN_KERNEL_USAGE = 1;

    private const byte ACTION_JUMP = 0;
    private const byte ACTION_DASH = 1;

    [SerializeField] private ushort _kernelCount;
    [SerializeField] private ushort _kernelsInUse;
    
    private float _horizontalInput;

    private bool _isFacingRight = true;
    
    private bool _canDoubleJump = true;

    private bool _isDashing = false;
    private bool _canDash = true;
    
    private bool _isStationary = true;

    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Animator _animator;

    [SerializeField] private Transform _kernalProjectileSpawnLocation;
    [SerializeField] private GameObject _kernelProjectile;
    
    void Start()
    {
        _kernelCount = DEFAULT_KERNEL_COUNT;
        _kernelsInUse = 1;
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
        }
        if (Input.GetButtonUp("Jump") && _rigidbody2D.velocity.y > 0f)
        {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _rigidbody2D.velocity.y * 0.5f);
        }
        
        //Double Jump Behavior
        if (Input.GetButtonDown("Jump") && !isGrounded() && _canDoubleJump && _kernelCount > _kernelsInUse)
        {
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
            StartCoroutine(dash());
        }
        
        changeDirection();
    }

    private void FixedUpdate()
    {
        if (_isDashing) return;
        
        _rigidbody2D.velocity = new Vector2(_horizontalInput * SPEED, _rigidbody2D.velocity.y);
        
        _animator.SetFloat("x velocity", Math.Abs(_rigidbody2D.velocity.x));
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, GROUNDCHECK_RADIUS, _groundLayer);
    }
    
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

    private void createKernelProjectile(byte action, bool isFacingRight)
    {
        GameObject kernel = GameObject.Instantiate(_kernelProjectile);

        kernel.transform.position = _kernalProjectileSpawnLocation.position;

        KernelProjectile kernelProjectile = kernel.GetComponent<KernelProjectile>();

        kernelProjectile.isFacingRight = isFacingRight;
        kernelProjectile.actionType = action;
    }

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
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
    private const float DASH_TIME = 0.15f;

    private const ushort MAX_KERNEL_USAGE = 3;
    private const ushort MIN_KERNEL_USAGE = 1;

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
    
    void Start()
    {
        _kernelCount = DEFAULT_KERNEL_COUNT;
        _kernelsInUse = 1;
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
        if (Input.GetButtonDown("Jump") && !isGrounded() && _canDoubleJump && _kernelCount > _kernelsInUse)
        {
            _canDoubleJump = false;

            _kernelCount -= _kernelsInUse;

            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x,
                DOUBLE_JUMP_BASE_POWER * (1f + (_kernelsInUse - 1) * DOUBLE_JUMP_KERNEL_MULTIPLIER));
        }

        if (Input.GetButtonUp("Jump") && _rigidbody2D.velocity.y > 0f)
        {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _rigidbody2D.velocity.y * 0.5f);
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
        else if (_horizontalInput == 0f)
        {
            _isStationary = true;
        }
    }

    private IEnumerator dash()
    {
        _canDash = false;
        _isDashing = true;

        _kernelCount -= _kernelsInUse;

        float oldGravity = _rigidbody2D.gravityScale;
        _rigidbody2D.gravityScale = 0f;
        _rigidbody2D.velocity = new Vector2(transform.localScale.x * (DASH_POWER * (1 + (_kernelsInUse - 1) * DASH_POWER_KERNEL_MULTIPLIER)), 0f);
        yield return new WaitForSeconds(DASH_TIME * (1 + (_kernelsInUse - 1) * DASH_TIME_KERNEL_MULTIPLIER));

        _isDashing = false;
        _rigidbody2D.gravityScale = oldGravity;
        yield return new WaitForSeconds(DASH_COOLDOWN);

        _canDash = true;
    }
}

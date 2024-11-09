using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    private const float GROUNDCHECK_RADIUS = 0.2f;
    
    private float _horizontalInput;
    private float _speed = 8f;
    private float _jumpingPower = 16f;

    private bool _isFacingRight = true;
    private bool _isStationary = true;

    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    
    void Start()
    {
        print("hello");
    }

    
    void Update()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _jumpingPower);
        }

        if (Input.GetButtonUp("Jump") && _rigidbody2D.velocity.y > 0f)
        {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _rigidbody2D.velocity.y * 0.5f);
        }
        
        changeDirection();
    }

    private void FixedUpdate()
    {
        _rigidbody2D.velocity = new Vector2(_horizontalInput * _speed, _rigidbody2D.velocity.y);
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
}

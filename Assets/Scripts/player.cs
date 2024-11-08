using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
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

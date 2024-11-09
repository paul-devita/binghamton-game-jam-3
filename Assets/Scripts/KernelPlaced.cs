using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KernelPlaced : MonoBehaviour
{
    private const float GROUNDCHECK_RADIUS = 0.1f;
    
    private const float FALLING_LIFETIME = 5f;
    private const float OVERALL_LIFETIME = 20f;
    
    private float _fallingTimer;
    private float _lifetimeTimer;
    
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Animator _animator;
    
    void Start()
    {
        _animator = GetComponent<Animator>();
        float anim = Random.Range(0.0f, 5.0f);
        _animator.SetFloat("pop animation", anim);
    }
    
    void Update()
    {
        _lifetimeTimer += Time.deltaTime;

        if (isGrounded())
        {
            _fallingTimer = 0f;
        }
        else
        {
            _fallingTimer += Time.deltaTime;

            if (_fallingTimer >= FALLING_LIFETIME)
            {
                Destroy(gameObject);
                return;
            }
        }

        if (_lifetimeTimer >= OVERALL_LIFETIME)
        {
            Destroy(gameObject);
        }
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, GROUNDCHECK_RADIUS, _groundLayer);
    }
}

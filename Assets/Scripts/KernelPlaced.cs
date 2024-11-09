using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KernelPlaced : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        float anim = Random.Range(0.0f, 5.0f);
        _animator.SetFloat("pop animation", anim);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

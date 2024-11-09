using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class leaf : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject.transform.position.y - other.gameObject.transform.localScale.y / 2.0f > gameObject.transform.position.y)
        {
            _animator.SetBool("holding player", true);
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.tag == "Player")
        {
            _animator.SetBool("holding player", false);
        }
    }
}

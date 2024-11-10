using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class locusts : MonoBehaviour
{
    private const float LOCUST_SPEED = 0.01f;

    [SerializeField] private Rigidbody2D _rigidbody2D; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + LOCUST_SPEED, 0f);
    }

    public void Restart() {
        transform.position = new Vector2(transform.position.x, -37.5f);
    }
}

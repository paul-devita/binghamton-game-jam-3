using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class locusts : MonoBehaviour
{
    private const float LOCUST_SPEED = 0.001f;

    [SerializeField] private Rigidbody2D _rigidbody2D; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y + LOCUST_SPEED);
    }

    public void Restart() {
        transform.position = new Vector2(transform.position.x, -37.5f);
    }
}

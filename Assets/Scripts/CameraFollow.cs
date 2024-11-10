using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float _follow_speed = 2.0f;
    private float _y_offset = 3.0f;
    [SerializeField] private Transform _target;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float target_x = _target.position.x;
        float target_y = _target.position.y;
        Vector3 new_pos = transform.position;

        new_pos = new Vector3(target_x, target_y + _y_offset, -10f);
        /*
        if (transform.position.y > 2 && transform.position.x >= -60 && transform.position.x <= 60) {
            new_pos = new Vector3(_target.position.x, _target.position.y + _y_offset, -10.0f);
        } else if (transform.position.y > 2) {
            new_pos = new Vector3(transform.position.x, _target.position.y + _y_offset, -10.0f);
        } else if (transform.position.x >= -60 && transform.position.x <= 60) {
            new_pos = new Vector3(_target.position.x, transform.position.y, -10.0f);
        }
        */
        transform.position = Vector3.Slerp(transform.position, new_pos, _follow_speed * Time.deltaTime);

        if (transform.position.x < -60) {
            transform.position = new Vector3(-60, transform.position.y, transform.position.z);
        } else if (transform.position.x > 60) {
            transform.position = new Vector3(60, transform.position.y, transform.position.z);
        }
        if (transform.position.y < 2) {
            transform.position = new Vector3(transform.position.x, 2, transform.position.z);
        }
    }
}

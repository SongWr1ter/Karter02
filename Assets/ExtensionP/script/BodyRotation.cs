using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyRotation : MonoBehaviour
{
    public Transform target;
    public float speed;
    private Vector3 direction;

    private float angle;

    // Update is called once per frame
    void Update()
    {
        direction = target.position - transform.position;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;//tan 唯一确定一个弧度?yes,but start on x's positive side(right side)
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.localRotation = Quaternion.Slerp(transform.rotation, q, speed * Time.deltaTime);
    }
}

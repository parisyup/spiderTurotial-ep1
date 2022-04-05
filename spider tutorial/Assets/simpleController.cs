using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleController : MonoBehaviour
{
    public Rigidbody cntroller;
    public Transform cam;

    public float speed = 6;
    Vector3 velocity;

    float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    void Update()
    {

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f && cntroller.velocity.magnitude < 4)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            cntroller.AddForce(moveDir.normalized * speed * Time.deltaTime);
        }
    }
}

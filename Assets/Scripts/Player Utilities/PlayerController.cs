using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float rotationspeed = 60;
    public float FlyingSpeed = 20;

    public CharacterController Controller;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Pitch rotates the camera around its local Right axis
        transform.Rotate(Vector3.left * Time.deltaTime * Input.GetAxis("Mouse Y") * rotationspeed);

        //Yaw rotates the camera around its local Up axis
        transform.Rotate(Vector3.up * Time.deltaTime * Input.GetAxis("Mouse X") * rotationspeed);
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Controller.Move(transform.forward * FlyingSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            Controller.Move(-transform.forward * FlyingSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            Controller.Move(-transform.right * FlyingSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            Controller.Move(transform.right * FlyingSpeed * Time.deltaTime);
        }
    }
}
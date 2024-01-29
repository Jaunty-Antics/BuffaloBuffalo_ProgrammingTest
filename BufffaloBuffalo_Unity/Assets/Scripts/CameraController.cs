using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CharacterController Controller;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        transform.Rotate(-Input.GetAxis("Mouse Y") * 70f * Time.deltaTime, 0, 0, Space.Self);
        transform.Rotate(0, Input.GetAxis("Mouse X") * 70f * Time.deltaTime, 0, Space.World);

        Controller.Move( transform.TransformDirection( new Vector3( Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * 10f) );
    }
}

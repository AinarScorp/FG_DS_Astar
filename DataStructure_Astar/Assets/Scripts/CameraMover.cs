using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMover : MonoBehaviour
{
    [SerializeField] float startingSpeed = 2.0f;
    [SerializeField] float speedIncrease = 1;
    [SerializeField] float cameraSizeChangeSpeed = 2;
    
    float currentSpeed;
    Camera thisCamera;

    void Awake()
    {
        thisCamera = GetComponent<Camera>();
        thisCamera.orthographic = true;
    }

    void Start()
    {
        currentSpeed = startingSpeed;
    }

    void Update()
    {
        MouseScroll();
        Move();
    }

    void Move()
    {
        float verticalAxis = Input.GetAxis("Vertical");
        float horizontalAxis = Input.GetAxis("Horizontal");

        if (verticalAxis == 0 && horizontalAxis == 0)
        {
            return;
        }

        float xChange = horizontalAxis;
        float yChange = verticalAxis;

        Vector3 newPosition = new Vector3(xChange, yChange).normalized;
        transform.position += newPosition * (currentSpeed * Time.deltaTime);
    }

    void MouseScroll()
    {
        float scrollDelta = Input.mouseScrollDelta.y;

        if (scrollDelta == 0)
        {
            return;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            ChangeCameraSize(scrollDelta);
        }
        else
        {
            ChangeSpeed(scrollDelta);
        }
    }

    void ChangeCameraSize(float scrollDelta)
    {
        thisCamera.orthographicSize += -scrollDelta * cameraSizeChangeSpeed * Time.deltaTime;
    }

    void ChangeSpeed(float scrollDelta)
    {
        currentSpeed += scrollDelta * speedIncrease * Time.deltaTime;
    }
}

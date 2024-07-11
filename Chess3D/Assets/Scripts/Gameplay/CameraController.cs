﻿using Cinemachine;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera _camera;
    [SerializeField] private Transform worldTarget;
    public Vector3 target; // Đối tượng mà camera sẽ xoay quanh
    public float distance = 15.0f; // Khoảng cách từ camera đến đối tượng
    public float xSpeed = 250.0f; // Tốc độ xoay theo trục x
    public float ySpeed = 120.0f; // Tốc độ xoay theo trục y

    public float yMinLimit = -20f; // Giới hạn góc xoay tối thiểu theo trục y
    public float yMaxLimit = 80f; // Giới hạn góc xoay tối đa theo trục y

    public float smoothTime = 0.2f; // Thời gian để camera giảm dần
    private float velocityX = 0.0f;
    private float velocityY = 0.0f;

    private float x = 0.0f;
    private float y = 0.0f;

    
    private float targetX;
    private float targetY;
    private float targetDistance;
    

    [SerializeField] Transform chess;
    [SerializeField] float zoomSpeed = 50f;

    //For zoom
    private float zoom;
    private float zoomMax = 6;
    private float zoomMin = 2;
    private float zoomVelocity = 0f;
    private float smoothZoomTime = 0.25f;
    //
    private float moveVelocityX = 0f;
    private float moveVelocityY = 0f;
    private float moveMax = 1f;
    private float moveMin = 0f;
    private float smoothMoveTime = 0.25f;
    [SerializeField] private float moveSpeedX = 10f;
    [SerializeField] private float moveSpeedY = 10f;

    private bool isLocked = false;
   
    public void Setup(Vector3 center, float distance)
    {
        
        _camera = GetComponent<CinemachineVirtualCamera>();
        transform.rotation = Quaternion.Euler(new Vector3(20, 40, 0));
        worldTarget.position = center;
        this.distance = distance;
        ChangeToDefaultCamera();

    }

    public void ChangeToDefaultCamera()
    {
        ChangeFollow(worldTarget);
    }
    // Method to change camera follower
    public void ChangeFollow(Transform followTarget)
    {
        _camera.Follow = followTarget;
        Vector3 center = followTarget.transform.position;
        
        target = center;
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        targetX = x;
        targetY = y;
        targetDistance = distance;

        // 
        Cursor.lockState = CursorLockMode.None;
        //
        zoom = Camera.main.orthographicSize;
    }

    void LateUpdate()
    {
        if (_camera != null && !isLocked)
        {
            HandleMoveCamera();
            HandleZoomCamera();
            HandleSwipeCamera();
        }
        
    }

    // Hàm để giới hạn góc quay
    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    private void HandleSwipeCamera()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) // Kiểm tra xem nút chuột trái có được nhấn không
        {
            targetX += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            targetY -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            targetY = ClampAngle(targetY, yMinLimit, yMaxLimit);
        }

        x = Mathf.SmoothDamp(x, targetX, ref velocityX, smoothTime);
        y = Mathf.SmoothDamp(y, targetY, ref velocityY, smoothTime);

        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 position = rotation * new Vector3(0.0f, 0.0f, -targetDistance) + target;

        transform.rotation = rotation;
        transform.position = position;
    }
    private void HandleZoomCamera()
    {
        if (Input.touchCount == 2 && !EventSystem.current.IsPointerOverGameObject())
        {
         
            Touch touchFirst = Input.GetTouch(0);
            Touch touchSecond = Input.GetTouch(1);

            Vector2 touchFirstPrePos = touchFirst.position - touchFirst.deltaPosition;
            Vector2 touchSecondPrePos = touchSecond.position - touchSecond.deltaPosition;

            Vector2 touchFirstVector = touchFirst.position - touchFirstPrePos;
            Vector2 touchSecondVector = touchSecond.position - touchSecondPrePos;
            if (Vector2.Angle(touchFirstVector, touchSecondVector) < 45f)
            {
                Debug.Log("MoveCamera");
            }
            else
            {
                float preMagnitude = (touchFirstPrePos - touchSecondPrePos).magnitude;
                float currentMagnitude = (touchFirst.position - touchSecond.position).magnitude;
                float diff = currentMagnitude - preMagnitude;

                Zoom(diff * zoomSpeed);
            }

            //


            
        }
        #if UNITY_EDITOR
        Zoom(Input.GetAxis("Mouse ScrollWheel")*zoomSpeed);
        #endif
    }
    private void Zoom(float inc)
    {
    
        zoom -= inc;
        zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);
        _camera.m_Lens.OrthographicSize = Mathf.SmoothDamp(_camera.m_Lens.OrthographicSize, zoom,  ref zoomVelocity, smoothZoomTime);
    }

    private void HandleMoveCamera()
    {
#if UNITY_EDITOR

        float x = Input.GetAxis("Horizontal") * moveSpeedX * 0.02f;
        float y = Input.GetAxis("Vertical") * moveSpeedY * 0.02f;
        Move(x, y);
        
#endif
    }

    private void Move(float x, float y)
    {
        CinemachineFramingTransposer framing = _camera.GetCinemachineComponent<CinemachineFramingTransposer>();

        float targetX = framing.m_ScreenX - x;
        float targetY = framing.m_ScreenY + y;

        targetX = Mathf.Clamp(targetX, moveMin, moveMax);
        targetY = Mathf.Clamp(targetY, moveMin, moveMax);

        float smoothX = Mathf.SmoothDamp(framing.m_ScreenX, targetX, ref moveVelocityX, smoothMoveTime);
        float smoothY = Mathf.SmoothDamp(framing.m_ScreenY, targetY, ref moveVelocityY, smoothMoveTime);

        framing.m_ScreenX = smoothX;
        framing.m_ScreenY = smoothY;
    }
    public void Lock()
    {
        isLocked = true;
    }

    public void Unlock()
    {
        isLocked = false;
    }
}

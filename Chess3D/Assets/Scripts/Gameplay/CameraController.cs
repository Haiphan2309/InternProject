using Cinemachine;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _camera;
    [SerializeField] private Transform worldTarget;
    private CinemachineFramingTransposer framing;
    //
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
    private Vector3 touchStart;
    private float moveVelocityX = 0f;
    private float moveVelocityY = 0f;
    private float moveMax = 1f;
    private float moveMin = 0f;
    private float smoothMoveTime = 0.25f;
    private float pinchMoveAngle = 90f;
    [SerializeField] private float moveSpeedX = 8f;
    [SerializeField] private float moveSpeedY = 8f;

    private bool isLocked = false;
    private enum CameraMode { Swipe, Move};
    private CameraMode cameraMode = CameraMode.Swipe;

    
    public void Setup(Vector3 center, float distance)
    {
        
        _camera = GetComponent<CinemachineVirtualCamera>();
        framing = _camera.GetCinemachineComponent<CinemachineFramingTransposer>();
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
        ChangeToDefaultMove();


    }
    public void ChangeToDefaultMove()
    {
        framing.m_ScreenX = 0.5f;
        framing.m_ScreenY = 0.5f;
    }
    void LateUpdate()
    {
        if (_camera != null && !isLocked)
        {
            HandleZoomCamera();
            if (cameraMode == CameraMode.Swipe)
            {
                HandleSwipeCamera();
            }
            else
            {
                HandleMoveCamera();
            }
            
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
        if (Input.GetMouseButton(0)) Debug.Log("Mouse button down");
        Debug.Log("Touch count: " + Input.touchCount.ToString());
        if ((Input.touchCount == 1 || Input.GetMouseButton(0)) && !EventSystem.current.IsPointerOverGameObject()) // Kiểm tra xem nút chuột trái có được nhấn không
        {
        
            targetX += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            targetY -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            targetY = ClampAngle(targetY, yMinLimit, yMaxLimit);
        }
        
        x = Mathf.SmoothDamp(x, targetX, ref velocityX, smoothTime);
        y = Mathf.SmoothDamp(y, targetY, ref velocityY, smoothTime);
       


         Quaternion rotation = Quaternion.Euler(y, x, 0);
         //Debug.Log(rotation * new Vector3(0.0f, 0.0f, -targetDistance) + " SADAS  " + targetDistance.ToString());
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
            //if (cameraMode == CameraMode.Move) // Move Camera
            //{
            //    Vector2 moveVector = Vector2.Lerp(touchFirstVector, touchSecondVector, 0.5f).normalized;
            //    Move(moveVector.x, moveVector.y);
            //}
           // else // Zoom Camera
           // {
                float preMagnitude = (touchFirstPrePos - touchSecondPrePos).magnitude;
                float currentMagnitude = (touchFirst.position - touchSecond.position).magnitude;
                float diff = currentMagnitude - preMagnitude;

                Zoom(diff * zoomSpeed);
            //}

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
        Debug.Log("Move");
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Camera.main.ViewportToScreenPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - Camera.main.ViewportToScreenPoint(Input.mousePosition);
            direction = direction.normalized;
            if (EventSystem.current.IsPointerOverGameObject()) direction = Vector3.zero;
            Move(direction.x * moveSpeedX * 0.02f, direction.y * moveSpeedY * 0.02f);
        }
    }

    private void Move(float x, float y)
    {
        

        float targetX = framing.m_ScreenX + x;
        float targetY = framing.m_ScreenY - y;

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

    public void ChangeCameraMode()
    {
        if (cameraMode == CameraMode.Move)
        {
            cameraMode = CameraMode.Swipe;
        }
        else
        {
            cameraMode = CameraMode.Move;
        }
  
    }

    
}

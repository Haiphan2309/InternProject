using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 target; // Đối tượng mà camera sẽ xoay quanh
    public float distance = 10.0f; // Khoảng cách từ camera đến đối tượng
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

    //For zoom
    private float zoomMax = 60;
    private float zoomMin = 16;

    public void Setup(Vector3 center, float distance)
    {
        this.distance = distance;
        target = center;
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        targetX = x;
        targetY = y;
        targetDistance = distance;

        // Đảm bảo rằng con trỏ chuột không bị khóa (tùy chọn)
        Cursor.lockState = CursorLockMode.None;
    }

    void LateUpdate()
{
        HandleZoomCamera();
        if (Input.GetMouseButton(0)) // Kiểm tra xem nút chuột trái có được nhấn không
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

    // Hàm để giới hạn góc quay
    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    private void HandleZoomCamera()
    {
        //if (Input.touchCount == 2)
        //{
        //    Debug.Log("Zoom");
        //    Touch touchFirst = Input.GetTouch(0);
        //    Touch touchSecond = Input.GetTouch(1);

        //    Vector2 touchFirstPrePos = touchFirst.position - touchFirst.deltaPosition;
        //    Vector2 touchSecondPrePos = touchSecond.position - touchSecond.deltaPosition;

        //    float preMagnitude = (touchFirstPrePos - touchSecondPrePos).magnitude;
        //    float currentMagnitude = (touchFirst.position - touchSecond.position).magnitude;

        //    float diff = currentMagnitude - preMagnitude;

        //    zoom(diff * 0.01f);
        //}

        zoom(Input.GetAxis("Mouse ScrollWheel")*10f);
    }
    private void zoom(float inc)
    {
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - inc, zoomMin, zoomMax);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GDC.Managers;

namespace GDC.Home
{
    public class Patrol : MonoBehaviour
    {
        public Transform graphic;
        Vector2 pivot;
        float moveSpeed;
        float waitTime;
        bool IsMoving 
        {
            get => isMoving;
            set
            {
                if (isMoving == value)
                {
                    return;
                }
                isMoving = value;
                if (isMoving == true)
                {
                    ON_MOVE?.Invoke();
                }
                else
                {
                    ON_STOP?.Invoke();
                }
            }
        }
        bool isMoving = false;
        public event System.Action ON_MOVE;
        public event System.Action ON_STOP;
        
        void Start()
        {
            SetupPatrol();
        }
        void SetupPatrol()
        {
            this.pivot = new Vector2(Random.Range(ConfigManager.Instance.PatrolConfig.MinPivot.x, ConfigManager.Instance.PatrolConfig.MaxPivot.x), Random.Range(ConfigManager.Instance.PatrolConfig.MinPivot.y, ConfigManager.Instance.PatrolConfig.MaxPivot.y));
            this.moveSpeed = Random.Range(ConfigManager.Instance.PatrolConfig.MinMoveSpeed, ConfigManager.Instance.PatrolConfig.MaxMoveSpeed);
            this.waitTime = Random.Range(ConfigManager.Instance.PatrolConfig.MinWaitingTime, ConfigManager.Instance.PatrolConfig.MaxWaitingTime);
            this.IsMoving = true;
        }
        void Update()
        {
            transform.position = Vector2.MoveTowards(transform.position, this.pivot, this.moveSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, this.pivot) < this.moveSpeed * Time.deltaTime * 1.5f)
            {
                this.IsMoving = false;
                if (waitTime > 0)
                {
                    waitTime -= Time.deltaTime;
                    return;
                }
                else
                {
                    SetupPatrol();
                }
            }
            if (transform.position.x < this.pivot.x)
            {
                FaceRight();
            }
            else
            {
                FaceLeft();
            }
        }
        void FaceLeft()
        {
            Vector3 scale = this.graphic.localScale;
            scale.x = -Mathf.Abs(scale.x);

            this.graphic.localScale = scale;
        }
        void FaceRight()
        {
            Vector3 scale = this.graphic.localScale;
            scale.x = Mathf.Abs(scale.x);

            this.graphic.localScale = scale;
        }
    }
}

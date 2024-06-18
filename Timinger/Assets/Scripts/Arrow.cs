using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Arrow : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float percentage = 0f;
    public float seconds = 1f;
    public float divider = 60f;
    public float repetitions = 1f;

    public Vector2 staticMousePosition = Vector2.zero;
    private float staticAngle = 0f;

    private void Update()
    {
        if(TimerManager.manager.dragging)
        {
            Vector3 dynamicMousePosition = Input.mousePosition;
            staticMousePosition = dynamicMousePosition;
            Vector3 direction = dynamicMousePosition - UIManager.manager.clockFace.transform.position;
            float dynamicAngle = Mathf.Atan2(direction.normalized.x,
                direction.normalized.y) * Mathf.Rad2Deg;
            float angleDifference = dynamicAngle - staticAngle;
            angleDifference = Mathf.Abs(angleDifference) > 180f ? 
                (360f - Mathf.Abs(angleDifference)) * Mathf.Sign(angleDifference) : angleDifference;
            TimerManager.manager.currentTime += seconds * divider * (angleDifference / 360f);

            staticMousePosition = dynamicMousePosition;
            staticAngle = dynamicAngle;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        TimerManager.manager.dragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        TimerManager.manager.dragging = false;
    }
}

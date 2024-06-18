using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScalable : MonoBehaviour
{
    [SerializeField] private Vector2 referenceDimensions = new Vector2(572f, 322f);
    [SerializeField] private bool relativeScale = false;
    [SerializeField] private bool relativePosition = false;
    [SerializeField] private bool uiObject = true;
    [SerializeField] private bool isEnabled = true;
    [SerializeField] private float extension = 1f;
    private Vector2 ratio = Vector2.one;

    private void Start()
    {
        AdjustRatio();
    }

    public void AdjustRatio()
    {
        if(isEnabled)
        {
            Vector2 cameraDimensions = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);
            Vector2 staticRatio = new Vector2(cameraDimensions.x / referenceDimensions.x, cameraDimensions.y / referenceDimensions.y);
            ratio = staticRatio;

            //ratio = UIManager.manager.GetCameraRatio();
            float minValue = Mathf.Min(ratio.x, ratio.y);
            float maxValue = Mathf.Min(cameraDimensions.x / referenceDimensions.y, staticRatio.y);
            ratio = relativeScale ? ratio : new Vector2(minValue, minValue);
            Vector2 scaleRatio = uiObject ? ratio : new Vector2(maxValue, maxValue);
            Vector3 relPosition = relativePosition ? new Vector3(transform.localPosition.x * staticRatio.x, transform.localPosition.y * staticRatio.y, transform.localPosition.z) : new Vector3(transform.localPosition.x * scaleRatio.x, transform.localPosition.y * scaleRatio.y, transform.localPosition.z * 1f);
            transform.localPosition = relPosition;
            transform.localScale = new Vector3(transform.localScale.x * scaleRatio.x, transform.localScale.y * scaleRatio.y, transform.localScale.z * 1f) * extension;
        }
    }
}

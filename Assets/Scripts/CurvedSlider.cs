using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedSlider : MonoBehaviour
{
    [SerializeField]
    private Transform handle;
    private float handleRadius;

    [SerializeField]
    private float sliderPerecent;

    public float SliderPercent {
        get { return sliderPerecent;}
    }

    // Start is called before the first frame update
    void Start()
    {
        if (handle == null) {
            return;
        }
        handleRadius = (handle.position - transform.position).magnitude;
        float initAngle = 135f;
        UpdateHandlePosition(initAngle);
        UpdateSliderPercent(initAngle);

    }


    private void Update() {
        
    }

    public void OnHandleDrag() {
        Vector3 touchPosition = Input.mousePosition;
        Vector2 direction = touchPosition - transform.position;
        float touchAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        touchAngle = touchAngle < 0 ? 360f + touchAngle : touchAngle;
        Debug.Log("[OnHandleDrag] touchAngle: " + touchAngle);
        UpdateHandlePosition(touchAngle);
        UpdateSliderPercent(touchAngle);
    }

    private void UpdateHandlePosition(float touchAngle) {
        float clampAngle = Mathf.Clamp(touchAngle, 45f, 225f);
        float dx = handleRadius * Mathf.Cos(clampAngle * Mathf.Deg2Rad);
        float dy = handleRadius * Mathf.Sin(clampAngle * Mathf.Deg2Rad);
        handle.position = transform.position + new Vector3(dx, dy, 0);
        Debug.Log("[UpdateHandlePosition] center transform: " + transform.position);
        Debug.Log("[UpdateHandlePosition] r: " + handleRadius +  "dx, dy: " + dx + ", " + dy);
    }

    private void UpdateSliderPercent(float touchAngle) {
        float localAngle = Mathf.Clamp(touchAngle - 135, -90, 90);
        sliderPerecent = (localAngle + 90f) / 180f;
    }
}

using UnityEngine.InputSystem;
using UnityEngine;

public class HexMapCamera : MonoBehaviour
{
    public float stickMinZoom, stickMaxZoom;

	float zoom = 1f;
	
	Transform swivel, stick;

    void Awake()
    {
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
    }
	void Update()
	{
		float zoomDelta = Mouse.current.scroll.ReadValue().y;
		if (zoomDelta != 0f)
		{
			AdjustZoom(zoomDelta);
		}
	}

	void AdjustZoom(float delta)
	{
		zoom = Mathf.Clamp01(zoom + delta);

		float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
		stick.localPosition = new Vector3(0f, 0f, distance);
	}
}
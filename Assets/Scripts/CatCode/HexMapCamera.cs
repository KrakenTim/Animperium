using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;

public class HexMapCamera : MonoBehaviour
{
    /*
    #region Not in tutorial
    static HexMapCamera instance;

    public static Vector3 LocalPosition => instance.transform.localPosition;

    public static float RotationAngle => instance.rotationAngle;
    private const float DefaultMoveTime = 0.5f;

    #endregion Not in tutorial
    */

    public float zoomSensitivity;

    public float stickMinZoom, stickMaxZoom;

    public float swivelMinZoom, swivelMaxZoom;

    //	public float moveSpeed;
    public float moveSpeedMinZoom, moveSpeedMaxZoom;

    public float rotationSpeed;

    float zoom = 0f;

    float rotationAngle;

    Transform swivel, stick;

    [Tooltip("Manual Assignment Of Grid Required")]
    public HexGrid grid;

    static HexMapCamera instance; public static bool Locked
    {
        set
        {
            instance.enabled = !value;
        }
    }

    void Awake()
    {
        #region Not in tutorial
        instance = this;
        #endregion Not in tutorial

        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
    }
    void OnEnable()
    {
        instance = this;
    }
    #region Not in tutorial
    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }
    #endregion Not in tutorial

    void Update()
    {
        float zoomDelta = Mouse.current.scroll.ReadValue().y * zoomSensitivity;
        if (zoomDelta != 0f)
        {
            AdjustZoom(zoomDelta);
        }

        float rotationDelta = Input.GetAxis("Rotation");
        if (rotationDelta != 0f)
        {
            AdjustRotation(rotationDelta);
        }

        float xDelta = Input.GetAxis("Horizontal"); // Altes system
        float zDelta = Input.GetAxis("Vertical"); // Altes system
        if (xDelta != 0f || zDelta != 0f)
        {
            AdjustPosition(xDelta, zDelta);
        }
    }

    void AdjustPosition(float xDelta, float zDelta)
    {
        Vector3 direction = transform.localRotation * new Vector3(xDelta, 0f, zDelta).normalized;
        float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
        float distance = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) * damping * Time.deltaTime;

        Vector3 position = transform.localPosition;
        position += direction * distance;
        transform.localPosition = ClampPosition(position);
    }

    void AdjustZoom(float delta)
    {
        zoom = Mathf.Clamp01(zoom + delta);

        float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
        stick.localPosition = new Vector3(0f, 0f, distance);

        float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
        swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    /*#region Not in tutorial
    public static CameraValues GetCurrentCameraValues()
    {
        CameraValues result = new CameraValues();

        result.localPosition = LocalPosition;
        result.rotationY = RotationAngle;
        result.zoom01 = instance.zoom;

        return result;
    }

    public static void SetCameraValues(CameraValues newValues)
    {
        SetPosition(newValues.localPosition);
        instance.SetRotation(newValues.rotationY);
        instance.SetZoom(newValues.zoom01);
    }

    public static void SetPosition(Vector3 position)
    {
        if (instance)
            instance.transform.localPosition = instance.ClampPosition(position);
    }

    void SetRotation(float yRotation)
    {
        if (instance)
        {
            instance.rotationAngle = yRotation;
            instance.AdjustRotation(0f);
        }
    }

    void SetZoom(float zoom01)
    {
        if (instance)
        {
            instance.zoom = zoom01;
            instance.AdjustZoom(0f);
        }
    }

    #endregion Not in tutorial
    */
    Vector3 ClampPosition(Vector3 position)
    {
        float xMax = (grid.cellCountX - 0.5f) * (2f * HexMetrics.innerRadius);
        position.x = Mathf.Clamp(position.x, 0f, xMax);

        float zMax = (grid.cellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
        position.z = Mathf.Clamp(position.z, 0f, zMax);

        return position;
    }

    void AdjustRotation(float delta)
    {
        rotationAngle += delta * rotationSpeed * Time.deltaTime;
        if (rotationAngle < 0f)
        {
            rotationAngle += 360f;
        }
        else if (rotationAngle >= 360f)
        {
            rotationAngle -= 360f;
        }
        transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
    }

    public static void ValidatePosition()
    {
        instance.AdjustPosition(0f, 0f);
    }
}
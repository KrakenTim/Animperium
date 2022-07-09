using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HexMapCamera : MonoBehaviour
{
    #region Not in tutorial

    public static Vector3 Position => instance.transform.position;

    public static float RotationAngle => instance.rotationAngle;
    private const float DefaultMoveTime = 0.5f;

    private HexGridLayer usedGridLayer;
    public static HexGridLayer GridLayer => instance.usedGridLayer;

    #endregion Not in tutorial   

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
    public HexGrid usedGrid;

    static HexMapCamera instance;
    public static HexMapCamera Instance { get { return instance; } }

    public UnityEvent<HexGridLayer> OnSwapToGrid = new UnityEvent<HexGridLayer>();

    [SerializeField] Vector2 mouseMoveBorder = new Vector2(0.025f, 0.05f);

    private Camera actualCamera;

    public static bool Locked
    {
        get
        {
            return !instance.enabled;
        }

        set
        {
            instance.enabled = !value;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
    }
    private void Start()
    {
        if (usedGrid == null)
            usedGrid = FindObjectOfType<HexGrid>();

        actualCamera = stick.GetComponentInChildren<Camera>();
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
        if (!Application.isFocused) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            SwapUsedGrid();
        else if (Keyboard.current.nKey.wasPressedThisFrame)
            SetRotation(0);

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

        UpdateMovement();
    }

    /// <summary>
    /// reads keyboard and mouse input and position to decide if the camera should be moved.
    /// </summary>
    private void UpdateMovement()
    {
        float xDelta = Input.GetAxis("Horizontal"); // Altes system
        float zDelta = Input.GetAxis("Vertical"); // Altes system
        if (xDelta != 0f || zDelta != 0f)
        {
            AdjustPosition(xDelta, zDelta);
            return;
        }

        Vector2 mousePosition = actualCamera.ScreenToViewportPoint(Mouse.current.position.ReadValue());
        
        if (InScreen(mousePosition) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (mousePosition.x < mouseMoveBorder.x)
                xDelta = -(1f - mousePosition.x / mouseMoveBorder.x);
            else if (mousePosition.x > 1f - mouseMoveBorder.x)
                xDelta = 1f - (1f - mousePosition.x) / mouseMoveBorder.x;

            if (mousePosition.y < mouseMoveBorder.y)
                zDelta = -(1f - mousePosition.y / mouseMoveBorder.y);
            else if (mousePosition.y > 1f - mouseMoveBorder.y)
                zDelta = 1f - (1f - mousePosition.y) / mouseMoveBorder.y;

            if (xDelta != 0f || zDelta != 0f)
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

    #region Not in tutorial
    public static CameraValues GetCurrentCameraValues()
    {
        CameraValues result = new CameraValues();

        result.localPosition = instance.transform.localPosition;
        result.rotationY = RotationAngle;
        result.zoom01 = instance.zoom;
        result.layer = GridLayer;

        return result;
    }

    public static void SetCameraValues(CameraValues newValues)
    {
        SwapToGrid(newValues.layer);

        SetPosition(newValues.localPosition);
        instance.SetRotation(newValues.rotationY);
        instance.SetZoom(newValues.zoom01);
    }

    public static void SetToCenter()
    {
        HexCell[] grid = instance.usedGrid.GetAllCells();

        Vector3 centerPosition;

        if (grid.Length % 2 == 1) //uneven number of cells, use middle
            centerPosition = grid[grid.Length / 2].transform.position;
        else // even number of cells, use average of both cells in the middle
            centerPosition = (grid[grid.Length / 2].transform.position + grid[grid.Length / 2 + 1].transform.position) / 2f;

        SetPosition(centerPosition);
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

    public static void SwapUsedGrid()
    {
        if (!GameManager.InGame) return;

        if (instance.usedGrid == HexGridManager.Current.Surface)
            SwapToUnderGround();
        else if (instance.usedGrid == HexGridManager.Current.Underground)
            SwapToSurface();
    }

    public static void SwapToGrid(HexGridLayer layer)
    {
        if (layer == HexGridLayer.Surface)
            SwapToSurface();
        else
            SwapToUnderGround();
    }

    public static void SwapToSurface()
    {
        if (instance.usedGrid == HexGridManager.Current.Surface) return;

        Instance.OnSwapToGrid.Invoke(HexGridLayer.Surface);
        instance.usedGridLayer = HexGridLayer.Surface;
        instance.SwapToGrid(HexGridManager.Current.Surface);
    }

    public static void SwapToUnderGround()
    {
        if (instance.usedGrid == HexGridManager.Current.Underground) return;

        Instance.OnSwapToGrid.Invoke(HexGridLayer.Underground);
        instance.usedGridLayer = HexGridLayer.Underground;
        instance.SwapToGrid(HexGridManager.Current.Underground);
    }

    private void SwapToGrid(HexGrid newGrid)
    {
        HexGrid oldGrid = instance.usedGrid;
        usedGrid = newGrid;

        SetPosition(instance.transform.localPosition + newGrid.transform.position - oldGrid.transform.position);
    }

    #endregion Not in tutorial

    Vector3 ClampPosition(Vector3 position)
    {
        Vector4 area = usedGrid.WorldArea();

        //float xMax = (grid.cellCountX - 0.5f) * (2f * HexMetrics.innerRadius);
        //position.x = Mathf.Clamp(position.x, 0f, xMax);
        position.x = Mathf.Clamp(position.x, area.x, area.y);

        //float zMax = (grid.cellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
        //position.z = Mathf.Clamp(position.z, 0f, zMax);
        position.z = Mathf.Clamp(position.z, area.z, area.w);

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
        if (instance)
            instance.AdjustPosition(0f, 0f);
    }

    /// <summary>
    /// returns true if the given viewport position is in the screen
    /// </summary>
    public static bool InScreen(Vector2 viewportPosition)
    {
        return viewportPosition.x >= 0f && viewportPosition.x <= 1f && viewportPosition.y >= 0f && viewportPosition.y <= 1f;
    }
}

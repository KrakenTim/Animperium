using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class CharacterMovement : MonoBehaviour
{
    public Tilemap map;
    public UnityEvent<Vector3> PointerClick;
    public Camera camera;
    private RaycastHit hit;

   // private NavMeshAgent agent;

    private string groundTag = "Ground";

    public float speed = 5f;
    private Vector3 target;
    void Start()
    {
        // agent = GetComponent<NavMeshAgent>();
        target = transform.position;
        destination = transform.position;
        mouseInput.Mouse.MouseClick.performed += _ => MouseClick();
    }

    void Update()
    {
        Mouse ms = InputSystem.GetDevice<Mouse>();
        
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Used old Input System
           // target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = transform.position.z;
        }

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.time);

       /*  DetectMouseClick();

        if (Vector3.Distance(transform.position, destination) > 0.1f)
            transform.position = Vector3.MoveTowards(transform.position, destination, movementSpeed * Time.deltaTime);


        Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag(groundTag))
            {
                transform.position = hit.point;
            }
        } */

    }

   /* private void DetectMouseClick()
    { 
        if (Mouse.current.leftButton.wasPressedThisFrame)
        
         //  Vermerk benutzt altes inoput system
           Vector3 mousePos = Input.mousePosition;
           PointerClick?.Invoke(mousePos);

    } 
   */
    
        [SerializeField] private float movementSpeed;
        MouseAndKeyboard mouseInput;
        private Vector3 destination;

        private void Awake()
        {
            mouseInput = new MouseAndKeyboard();
        }

        private void OnEnabled()
        {
            mouseInput.Enable();
        }

        private void OnDisable()
        {
            mouseInput.Disable();
        }

        // Start is called before the first frame update
       /* void Start()
        {
            destination = transform.position;
            mouseInput.Mouse.MouseClick.performed += _ => MouseClick();
        } */

        private void MouseClick()
        {
            Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3Int gridPosition = map.WorldToCell(mousePosition);

            if (map.HasTile(gridPosition))
            {
                destination = mousePosition;
            }
        }

        // Update is called once per frame
      /*  void Update()
        {
            if (Vector3.Distance(transform.position, destination) > 0.1f)
            transform.position = Vector3.MoveTowards(transform.position, destination, movementSpeed * Time.deltaTime);
        } */
    
}

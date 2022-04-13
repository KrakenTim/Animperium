using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class Move : MonoBehaviour
{
    public float speed = 20f;
    bool moving;
    Vector3 lastClickedPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            lastClickedPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Debug.Log("Last Position" + lastClickedPos);
            moving = true;
        }

        if (moving && (Vector3)transform.position != lastClickedPos)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, lastClickedPos, step);
        }
        else
        {
            moving = false;
        }
    }
}

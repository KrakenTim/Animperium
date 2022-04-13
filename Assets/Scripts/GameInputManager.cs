using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInputManager : MonoBehaviour
{
    static GameInputManager instance;

    [SerializeField] PlayerPawn currentlySelectedPawn;


    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        if (instance = this)
            instance = null;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeColorRandomizer : MonoBehaviour
{
    public float GreenMin;
    public float GreenMax;

    private MeshRenderer [] meshColliders;

    void Awake()
    {
        meshColliders = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshColliders.Length; i++)
        {
            meshColliders[i].material.SetColor("_LeafColor", new Color(1f/256f*128f, 1f / 256f * Random.Range(GreenMin, GreenMax), 0f));
        }
    }
}
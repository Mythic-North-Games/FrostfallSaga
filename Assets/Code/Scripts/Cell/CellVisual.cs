using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellVisual : MonoBehaviour
{
    [SerializeField] private Material HighlightMaterial;
    
    private Material _oldMaterial;

    private void Start()
    {
        _oldMaterial = GetComponent<Renderer>().material;
    }

    private void OnMouseEnter()
    {
        GetComponent<Renderer>().material = HighlightMaterial;
    }

    private void OnMouseExit()
    {
        GetComponent<Renderer>().material = _oldMaterial;
    }
}

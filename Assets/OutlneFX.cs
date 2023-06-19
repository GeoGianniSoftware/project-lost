using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlneFX : MonoBehaviour
{
    [Min(1)]
    
    public float size;
    public Color color;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rescale = new Vector3(.25f * size, 1, .25f * size);
        if (transform.localScale != rescale)
            transform.localScale = rescale;
        if(GetComponent<MeshRenderer>().material.color != color) {
            Material test = new Material(GetComponent<MeshRenderer>().material);
            test.color = color;
            GetComponent<MeshRenderer>().material = test;
        }
    }
}

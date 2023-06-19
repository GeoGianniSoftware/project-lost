using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePad : MonoBehaviour
{
    public GameObject resourcePrefab;
    public Vector3 spawnOffset;
    public float refreshRadius;
    GameObject resourceRef;
    // Start is called before the first frame update
    void Start()
    {
        SpawnResource();
    }

    // Update is called once per frame
    void Update()
    {
        if(resourceRef != null && Vector3.Distance(transform.position, resourceRef.transform.position) > refreshRadius) {
            resourceRef = null;
        }
        else if(resourceRef == null) {
            SpawnResource();
        }
        if(resourceRef != null && resourceRef.transform.position.y < transform.position.y) {
            Destroy(resourceRef);
        }
    }

    void SpawnResource() {
        resourceRef = Instantiate(resourcePrefab, transform.position + spawnOffset, Quaternion.identity);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [field: SerializeField] public bool wallCollided { get; private set; }
    [field: SerializeField] public bool fishCatch { get; private set; }
    [field: SerializeField] public bool pullable { get; private set; }
    [field: SerializeField] public bool bad { get; private set; }
    [field: SerializeField] public Vector3 hookHit { get; private set; }
    [field: SerializeField] public GameObject hitObj { get; private set; }


    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject);
        if (collision.gameObject.tag == "Terrain")
        {
            wallCollided = true;
            hookHit = transform.position;
        }
        else if (collision.gameObject.tag == "Fish")
        {
            fishCatch = true;
            hitObj = collision.gameObject;
        }
        else if (collision.gameObject.tag == "Pullable")
        {
            pullable = true;
            hitObj = collision.gameObject;
        }
        else
        {
            bad = true;
        }
    }
    public void ResetHook()
    {
        wallCollided = false;
        fishCatch = false;
        bad = false;
        pullable = false;
        hitObj = null;
    }


}

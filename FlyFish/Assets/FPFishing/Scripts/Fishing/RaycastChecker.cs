using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaycastChecker : MonoBehaviour
{
    [SerializeField] private Text distanceDisplay;
    [SerializeField] private LayerMask layer;
    [SerializeField] private FishingPlayerCharacterController fpcc;
    private Camera cam;
   
    void Start()
    {
        cam = GetComponent<Camera>();
        fpcc = GameObject.Find("Player").GetComponent<FishingPlayerCharacterController>();
    }

    void Update()
    {
        switch (fpcc.state)
        {
            case FishingPlayerCharacterController.PlayerState.Normal:
                if (Physics.Raycast(transform.position, cam.transform.forward, out RaycastHit hit, layer))
                {
                    Debug.Log(hit.transform.gameObject.name);
                    if ( hit.collider.transform.gameObject.layer == 9 || 
                         hit.collider.transform.gameObject.layer == 8)
                    {
                        float distance = Vector3.Distance(hit.transform.position, transform.position);
                        distanceDisplay.text = distance.ToString("F1") + " m";
                    }
                    else
                        distanceDisplay.text = "";

                    if (Vector3.Distance(transform.position, hit.transform.position)<3)
                    {

                    }

                }
                else
                    distanceDisplay.text = "";

                break;
        }
    }

    public void Pickup()
    {

    }


}

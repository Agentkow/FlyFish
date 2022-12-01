using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistanceChecker : MonoBehaviour
{
    [SerializeField] private Text distanceDisplay;
    [SerializeField] private LayerMask layer;
    [SerializeField] private FishingPlayerCharacterController fpcc;
    private Camera cam;
   
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        switch (fpcc.state)
        {
            case FishingPlayerCharacterController.PlayerState.Normal:
                if (Physics.Raycast(transform.position, cam.transform.forward, out RaycastHit hit, layer))
                {
                    if (hit.transform.gameObject.layer <10)
                    {

                        float distance = Vector3.Distance(hit.transform.position, transform.position);
                        distanceDisplay.text = distance.ToString("F1") + " m";
                    }
                    else
                        distanceDisplay.text = "";

                }

                break;
        }

        
    }
}

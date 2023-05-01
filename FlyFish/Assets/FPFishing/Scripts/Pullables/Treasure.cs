using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
#pragma warning disable 0414
    [SerializeField] private float value = 100f;

    public void Initialize()
    {
        
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.tag == "Player")
        {
            if (collision.gameObject.GetComponent<FishingPlayerCharacterController>())
            {

            }
            gameObject.SetActive(false);
        }
    }
}

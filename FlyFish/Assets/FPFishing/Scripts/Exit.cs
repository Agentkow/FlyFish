using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    [SerializeField] private bool exitOpen;
    [SerializeField] private float timeToWait = 120f;
    [SerializeField] private TransitionEffect te;
    [SerializeField] private GameObject marker;

    // Start is called before the first frame update
    void Awake()
    {
        exitOpen = false;
        StartCoroutine(ExitAvailable());
        marker.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<FishingPlayerCharacterController>() !=null)
            if (exitOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                GameManager.gm.GoToHub();
                te.LoadLevel("Hub");
            }
    }

    IEnumerator ExitAvailable()
    {
        yield return new WaitForSeconds(timeToWait);
        marker.SetActive(true);
        exitOpen = true;
    }
}

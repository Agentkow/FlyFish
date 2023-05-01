using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakSpark : MonoBehaviour
{
    [SerializeField] private ParticleSystem breakSpark;
    public static GameObject Create(Vector3 position) //spawn damage popup from GameAssets
    {
        if (ObjectPool.pool.GetDamageObject() != null)
        {
            GameObject popup = ObjectPool.pool.GetBreakObject();
            popup.transform.position = position;

            popup.SetActive(true);
            return popup;
        }
        return null;

    }

    public void PlayEffect()
    {
        breakSpark.Play();
        gameObject.SetActive(false);

    }
}

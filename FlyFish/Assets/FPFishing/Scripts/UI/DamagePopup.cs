using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro damageText;
    [SerializeField] private float timeMax = 5;
    [SerializeField] private Color textColor;
    private float disappearTimer;
    public static GameObject Create (Vector3 position, float damageAmount, float rate) //spawn damage popup from GameAssets
    {
        GameObject popup = ObjectPool.pool.GetDamageObject();
        popup.transform.position = position;
        DamagePopup damagePopup = popup.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount, rate);
        
        popup.SetActive(true);
        return popup;
    }

    public void Setup(float damageAmount, float damageRate) // set up damage popup
    {
        disappearTimer = timeMax;
        damageText.text = damageAmount.ToString("F0");
        if (damageAmount<=damageRate) // weak damage
        {
            damageText.fontSize = 5;
            textColor = new Color(255, 255, 255);
        }
        else if(damageAmount>damageRate && damageAmount<(damageRate*2.5)) // normal damage
        {
            damageText.fontSize = 8;
            textColor = new Color(255, 190, 0);
        }
        else if(damageAmount>damageRate*2.5) // hook break damage
        {
            damageText.fontSize = 10;
            textColor = new Color(255, 0, 0);
        }

        damageText.color = textColor;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform.position);
        transform.position += new Vector3(0, 10f)*Time.deltaTime;
        damageText.fontSize = Vector3.Distance(transform.position, Camera.main.transform.position);
        disappearTimer -= Time.deltaTime;
        if (disappearTimer<0)
        {
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            damageText.color = textColor;
            if (textColor.a<0)
            {
                disappearTimer = timeMax;
                gameObject.SetActive(false);
            }
        }
    }
}

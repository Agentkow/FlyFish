using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimation : MonoBehaviour
{
    [SerializeField] private float openTime;
    [SerializeField] private float closeTime;
    // Start is called before the first frame update
    private void Start() => transform.localScale = Vector2.zero;

    public void Open()=> transform.LeanScale(Vector2.one, openTime).setEaseInCirc();

    public void Close() => transform.LeanScale(Vector2.zero, closeTime).setEaseInCirc();

}

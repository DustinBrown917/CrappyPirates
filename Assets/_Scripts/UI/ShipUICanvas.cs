using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class ShipUICanvas : MonoBehaviour
{
    private static Canvas canvas_ = null;
    public static Canvas Canvas { get => canvas_; }


    private void Awake()
    {
        if(canvas_ == null) {
            canvas_ = GetComponent<Canvas>();
        }
    }

    private void OnDestroy()
    {
        if(canvas_ == GetComponent<Canvas>()) {
            canvas_ = null;
        }
    }
}

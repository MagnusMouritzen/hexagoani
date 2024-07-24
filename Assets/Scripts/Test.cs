using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteAlways]
public class Test : MonoBehaviour {
    public Vector2 direction = Vector2.one;
    public float scaling = 1;
    public Transform child;
    public bool make;

    private void Update() {
        if (!make) return;
        make = false;
        transform.eulerAngles = new Vector3(0, 0, Math.Sign(direction.y) * Mathf.Rad2Deg * Mathf.Acos(direction.normalized.x));
        child.eulerAngles = Vector3.zero;
        transform.localScale = new Vector3(scaling, 1, 1);
    }
}

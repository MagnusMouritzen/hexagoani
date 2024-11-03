using System;
using UnityEngine;

/// <summary>
/// Controls camera and background size to match.
/// </summary>
public class CameraController : MonoBehaviour {
    [SerializeField] private new Camera camera;
    [SerializeField] private Transform background;
    [SerializeField, Tooltip("Scale relative to camera orthographic size.")] private float backgroundScale;

    public void SetCameraSize(float size) {
        camera.orthographicSize = size;
        SetBackgroundSize();
    }
    
    private void SetBackgroundSize() {
        float size = camera.orthographicSize * backgroundScale;
        background.localScale = new Vector3(size, size, 1f);
    }

    private void OnValidate() {
        SetBackgroundSize();
    }
}

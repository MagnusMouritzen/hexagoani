using System;
using UnityEngine;

/// <summary>
/// This squeezes the object along a chosen direction.
/// Quick and dirty, would probably be better as a shader.
/// The graphics should be the child of an empty object, which should be the child of this object.
/// </summary>
public class Squeezer : MonoBehaviour {
    [SerializeField] private Transform child;

    /// <summary>
    /// Sets the direction to squeeze in.
    /// </summary>
    /// <param name="direction">Direction to squeeze, does not need to be normalised.</param>
    public void SetDirection(Vector2 direction) {
        // cos(v) = A * B / (|A| * |B|), B is (1,0).
        // The sign is needed to take negative y into account.
        transform.eulerAngles = new Vector3(0, 0, Math.Sign(direction.y) * Mathf.Rad2Deg * Mathf.Acos(direction.normalized.x));
        child.eulerAngles = Vector3.zero;
    }
    
    public void ResetDirection() {
        transform.eulerAngles = Vector3.zero;
        transform.eulerAngles = Vector3.zero;
    }

    /// <summary>
    /// Stretches the graphics with the chosen scale. Do this after setting Direction.
    /// </summary>
    /// <param name="squeeze">Addition to base scale. Keep >= -1.</param>
    public void Squeeze(Vector2 squeeze) {
        transform.localScale = Vector3.one + (Vector3)squeeze;
    }
}

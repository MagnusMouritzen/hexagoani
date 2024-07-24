using UnityEngine;

/// <summary>
/// Gathers data for the pieces in a flyweight manner.
/// </summary>
[CreateAssetMenu(fileName = "Settings", menuName = "Settings")]
public class Settings : ScriptableObject {
    public float speed = 5f;
    public Sprite[] sprites;
    public Color[] colors;
    public float[] fontSizes;
}

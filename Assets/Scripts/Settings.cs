using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Settings")]
public class Settings : ScriptableObject {
    public float speed = 5f;
    public Sprite[] sprites;
    public Color[] colors;
    public float[] fontSizes;
}

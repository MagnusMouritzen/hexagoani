using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour {
    [SerializeField] private TextMeshPro text;
    [SerializeField] private Settings settings;
    [SerializeField] private SpriteRenderer circle;
    [SerializeField] private SpriteRenderer outline;
    
    private Vector3 _destination;
    private Vector3 _start;
    private bool _isMoving;
    private float _timer;

    public int Stage { get; private set; } = -1;

    private void Start() {
        IncreaseStage();
    }

    private void Update() {
        if (!_isMoving) return;
        _timer += Time.deltaTime * settings.speed;
        if (_timer >= 1f) {
            _timer = 1f;
            _isMoving = false;
        }
        transform.position = Vector3.Lerp(_start, _destination, _timer);
    }

    public void MoveTo(Vector3 pos) {
        _isMoving = true;
        _destination = pos;
        _start = transform.position;
        _timer = 0f;
    }

    public void IncreaseStage()
    {
        Stage++;
        transform.name = (Pow(3, Stage)).ToString();
        text.text = (Pow(3, Stage)).ToString();
        if (settings.sprites.Length != 0) circle.sprite = settings.sprites[Stage % settings.sprites.Length];
        text.color = settings.colors[Stage % settings.colors.Length];
        outline.color = settings.colors[Stage % settings.colors.Length];
        text.fontSize = settings.fontSizes[Math.Min(Stage, settings.fontSizes.Length - 1)];
    }

    private static int Pow(int a, int b) {
        if (b == 0) {
            return 1;
        }
        return a * Pow(a, b - 1);
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}

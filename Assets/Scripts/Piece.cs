using System;
using TMPro;
using UnityEngine;

/// <summary>
/// The visual representation of a piece on the board.
/// </summary>
public class Piece : MonoBehaviour {
    [Header("Properties")] 
    [SerializeField] private Vector2 squeeze = Vector2.one;
    
    [Header("Setup")]
    [SerializeField] private TextMeshPro text;
    [SerializeField] private Settings settings;
    [SerializeField] private SpriteRenderer circle;
    [SerializeField] private SpriteRenderer outline;
    [SerializeField] private Squeezer squeezer;
    
    private Vector3 _destination;
    private Vector3 _start;
    private bool _isMoving;
    private float _timer;
    private float _relativeDist;

    /// <summary>
    /// The "level" of the piece. Cubic root of Value.
    /// </summary>
    public int Stage { get; private set; } = 0;

    /// <summary>
    /// The number on the piece. Third power of Stage.
    /// </summary>
    public int Value { get; private set; } = 1;

    private void Start() {
        UpdateDisplay();
    }

    private void Update() {
        if (!_isMoving) return;
        UpdateTimer();
        Move();
        Squeeze();
    }

    private void UpdateTimer() {
        _timer += Time.deltaTime * settings.speed;
        if (_timer >= 1f) {
            _timer = 1f;
            _isMoving = false;
        }
    }

    private void Move() {
        float t = -(Mathf.Cos(Mathf.PI * _timer) - 1) / 2;
        transform.position = Vector3.Lerp(_start, _destination, t);
    }

    private void Squeeze() {
        float t = Mathf.Sin(Mathf.PI * _timer);  // Derivative of Move's t (almost). Goes from 0 to 1 to 0.
        squeezer.Squeeze(t * _relativeDist * squeeze);
    }

    /// <summary>
    /// Gives the piece a target to move to.
    /// </summary>
    /// <param name="pos">Destination.</param>
    /// <param name="relativeDist">How far the movement is, relative to the longest possible distance. 0-1.</param>
    public void MoveTo(Vector3 pos, float relativeDist) {
        _isMoving = true;
        _destination = pos;
        _start = transform.position;
        _timer = 0f;
        _relativeDist = relativeDist;
        squeezer.SetDirection(pos - transform.position);
    }

    /// <summary>
    /// Increases the stage of the piece.
    /// </summary>
    public void IncreaseStage()
    {
        Stage++;
        Value *= 3;
        UpdateDisplay();
    }

    private void UpdateDisplay() {
        transform.name = Value.ToString();
        text.text = Value.ToString();
        if (settings.sprites.Length != 0) circle.sprite = settings.sprites[Stage % settings.sprites.Length];
        text.color = settings.colors[Stage % settings.colors.Length];
        outline.color = settings.colors[Stage % settings.colors.Length];
        text.fontSize = settings.fontSizes[Math.Min(Stage, settings.fontSizes.Length - 1)];
    }

    /// <summary>
    /// Removes the piece from the game.
    /// </summary>
    public void Remove()
    {
        Destroy(gameObject);
    }
}

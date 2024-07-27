using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    [SerializeField] private GridManager gridManager = null;
    [SerializeField] private InputManager inputManager = null;
    [SerializeField] private Piece piecePrefab = null;
    [SerializeField] private Settings settings = null;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private Camera camera;
    [SerializeField] private TMP_Text scoreText;

    private Hexagon<Piece> _hexagon;
    private PieceInstructor _pieceInstructor;
    private bool _inputStage;
    private int _pieces;

    private int _score;
    private int Score {
        get => _score;
        set {
            _score = value;
            scoreText.text = _score.ToString();
        }
    }

    private void Awake() {
        inputManager.MovementRegistered += OnMovementRegistered;
    }

    public void CleanUpGame() {
        for (int i = 0; i < _hexagon.Size; i++) {
            if (_hexagon[i] != null) _pieceInstructor.AddPieceToRemove(_hexagon[i]);
        }
        _pieceInstructor.ExecuteRemove();
    }

    public void StartNewGame(int size) {
        gridManager.GenerateHexagon(size);
        _hexagon = new Hexagon<Piece>(size);
        _pieceInstructor = new PieceInstructor(_hexagon.Size, settings);
        GeneratePiece();
        camera.orthographicSize = gridManager.BoardHeight;
        _inputStage = true;
    }

    private void OnMovementRegistered(Movement movement) {
        if (!_inputStage) return;
        StartCoroutine(ExecuteTurn(movement));
    }

    /// <summary>
    /// Performs the entire move, during which no new moves can be registered.
    /// </summary>
    /// <param name="movement">The movement to perform</param>
    /// <returns></returns>
    private IEnumerator ExecuteTurn(Movement movement) {
        _inputStage = false;
        (HexAxis row, HexAxis col, int dir) = CalculateMove(movement);
        if (PerformMoveLogic(row, col, dir)) {  // True if the move resulted in a change.
            PlayMoveSounds(movement);
            yield return new WaitForSeconds(_pieceInstructor.WaitForMove());
            yield return new WaitForSeconds(_pieceInstructor.ExecuteIncrease());
            yield return new WaitForSeconds(_pieceInstructor.ExecuteRemove());
            yield return new WaitForSeconds(GeneratePiece());
        }
        _inputStage = true;
    }

    /// <summary>
    /// Turns a movement command into input for PerformMoveLogic.
    /// </summary>
    /// <param name="movement">Movement command.</param>
    /// <returns>Input for PerformMoveLogic.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static (HexAxis, HexAxis, int) CalculateMove(Movement movement)
    {
        return movement switch
        {
            Movement.UpRight => (HexAxis.I, HexAxis.J, -1),
            Movement.Right => (HexAxis.H, HexAxis.I, -1),
            Movement.DownRight => (HexAxis.J, HexAxis.I, -1),
            Movement.DownLeft => (HexAxis.I, HexAxis.J, 1),
            Movement.Left => (HexAxis.H, HexAxis.I, 1),
            Movement.UpLeft => (HexAxis.J, HexAxis.I, 1),
            _ => throw new ArgumentOutOfRangeException(nameof(movement), movement, "Must have a value given by the enum.")
        };
    }

    /// <summary>
    /// Performs the movement for all pieces and updates the hexagon appropriately. Also records all the changes in the PieceInstructor.
    /// </summary>
    /// <param name="rowAxis">Orthogonal to the movement. So for each value along this axis, there is a row. The pieces move in a direction along their respective rows.</param>
    /// <param name="columnAxis">One of the other axis.</param>
    /// <param name="direction">The direction along the column axis from end to start of movement. So opposite of movement.</param>
    /// <returns>If the move actually did anything.</returns>
    private bool PerformMoveLogic(HexAxis rowAxis, HexAxis columnAxis, int direction) {
        bool changeHappened = false;
        // Go through every tile in the hexagon.
        HexCoordinates c = new() { AAxis = rowAxis, BAxis = columnAxis };  // The current tile.
        for (c.A = 0; c.A < _hexagon.Diameter; c.A++) {  // Go through all rows.
            int start = direction == 1 ? _hexagon.RowMin(c) : _hexagon.RowMax(c);
            int end = direction == 1 ? _hexagon.RowMax(c) + 1 : _hexagon.RowMin(c) - 1;
            HexCoordinates lastSeen = c;  // The last piece that was seen on this row (in its final position after movement).
            lastSeen.B = -1;
            HexCoordinates otherMatching = lastSeen;
            for (c.B = start; c.B != end; c.B += direction) {  // Go through all tiles in row, starting at the end of the movement.
                Piece cur = _hexagon[c];
                if (cur == null) {  // Nothing is done for empty tiles.
                    continue;
                }
                if (lastSeen.B == -1) {  // If no piece has been seen yet, this piece should move to the edge.
                    lastSeen.B = start;
                    if (MovePieceLogic(c, lastSeen, cur)) changeHappened = true;
                }
                else if (cur.Stage == _hexagon[lastSeen].Stage) {  // If a piece has been seen before, and they are the same number.
                    if (otherMatching.B != -1) {  // We now have three identical pieces, so they should merge.
                        changeHappened = true;
                        Score += cur.Value * 3;
                        Piece temp = _hexagon[otherMatching];
                        RemovePiece(temp);
                        RemovePiece(_hexagon[lastSeen]);
                        IncreasePiece(cur);
                        MovePieceLogic(otherMatching, lastSeen, temp);
                        MovePieceLogic(c, lastSeen, cur);
                        otherMatching.B = -1;
                    }
                    else {  // If it's only those two matching, remember this one and keep lastSeen as is (to reduce operations and simplify).
                        otherMatching.B = c.B;
                    }
                }
                else {  // If a piece has been seen before, but they are of different numbers.
                    if (otherMatching.B != -1) {  // If the two that came before matched, move the second of the two since it no longer has chance to merge.
                        lastSeen.B += direction;
                        if (MovePieceLogic(otherMatching, lastSeen)) changeHappened = true;
                    }
                    // Move last seen and the piece.
                    lastSeen.B += direction;
                    if (MovePieceLogic(c, lastSeen, cur)) changeHappened = true;
                    otherMatching.B = -1;
                }
            }
            if (otherMatching.B != -1) {  // After the row is done, if there was a hope of merging at the end, move the final piece.
                lastSeen.B += direction;
                if (MovePieceLogic(otherMatching, lastSeen)) changeHappened = true;
            }
        }
        return changeHappened;
    }

    /// <summary>
    /// Moves piece in the hexagon structure and informs the piece instructor of the movement.
    /// </summary>
    /// <param name="start">The initial position of the piece.</param>
    /// <param name="end">The position to move the piece to.</param>
    /// <returns>Whether the piece actually moved, i.e. if the start and end are different.</returns>
    private bool MovePieceLogic(HexCoordinates start, HexCoordinates end)
    {
        return MovePieceLogic(start, end, _hexagon[start]);
    }
    
    /// <summary>
    /// Moves piece in the hexagon structure and informs the piece instructor of the movement.
    /// </summary>
    /// <param name="start">The initial position of the piece.</param>
    /// <param name="end">The position to move the piece to.</param>
    /// <param name="piece">The piece to move.</param>
    /// <returns>Whether the piece actually moved, i.e. if the start and end are different.</returns>
    private bool MovePieceLogic(HexCoordinates start, HexCoordinates end, Piece piece)
    {
        if (start.AAxis != end.AAxis || start.BAxis != end.BAxis) Debug.LogError("The axis of the coordinates given to MovePieceLogic should be the same.");  // This is important for the following comparison and the calculation of relativeDist.
        if (start.Values == end.Values) return false;
        _hexagon[start] = null;
        _hexagon[end] = piece;
        float relativeDist = Math.Max(Math.Abs(start.A - end.A), Math.Abs(start.B - end.B)) / (float)_hexagon.Diameter;
        _pieceInstructor.MovePiece(piece, gridManager.GetTilePosition(end), relativeDist);
        return true;
    }

    /// <summary>
    /// Marks piece to have stage increased.
    /// </summary>
    /// <param name="piece"></param>
    private void IncreasePiece(Piece piece) {
        _pieceInstructor.AddPieceToIncrease(piece);
    }

    /// <summary>
    /// Marks piece to be removed.
    /// </summary>
    /// <param name="piece"></param>
    private void RemovePiece(Piece piece) {
        _pieceInstructor.AddPieceToRemove(piece);
        _pieces--;
    }

    /// <summary>
    /// Creates a new piece at a random point.
    /// </summary>
    /// <returns>The time this operation should "pause" the game.</returns>
    private float GeneratePiece() {
        if (_pieces == _hexagon.Size) {
            return 0f;
        }
        int randomPosition;
        do {
            randomPosition = Random.Range(0, _hexagon.Size);
        } while (_hexagon[randomPosition] != null);
        CreatePiece(randomPosition);
        return 0.1f;
    }
    
    /// <summary>
    /// Instantiates a new piece and adds it to the hexagon.
    /// </summary>
    /// <param name="k">The position of the piece.</param>
    /// <returns>The new piece.</returns>
    private Piece CreatePiece(int k) {
        Piece piece = Instantiate(piecePrefab, gridManager.GetTilePosition(k), Quaternion.identity);
        _hexagon[k] = piece;
        _pieces++;
        int rng = Random.Range(1, 6);
        if (rng == 1) {
            _pieceInstructor.InstantPieceIncrease(piece);            
        }
        return piece;
    }

    private void PlayMoveSounds(Movement movement) {
        audioManager.PlayMovementSound(movement);
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [SerializeField] private int size = 1;
    [SerializeField] private GridManager gridManager = null;
    [SerializeField] private InputManager inputManager = null;
    [SerializeField] private GameObject piecePrefab = null;
    [SerializeField] private Settings settings = null;

    private Hexagon<Piece> _hexagon;
    private PieceInstructor _pieceInstructor;
    private bool _inputStage;
    private int _pieces;
    private bool _lost;
    private static bool _hasPlayedMusic = false;

    private void Awake() {
        gridManager.GenerateHexagon(size);
        _hexagon = new Hexagon<Piece>(size);
        _pieceInstructor = new PieceInstructor(_hexagon.Size, settings);
        inputManager.MovementRegistered += OnMovementRegistered;
    }

    private void Start() {
        if (!_hasPlayedMusic) {
            GetComponent<AudioSource>().Play();
            _hasPlayedMusic = true;
        }
        GeneratePiece();
        _inputStage = true;
    }

    public void StartNewGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        if (CalculateMove(movement)) {
            yield return new WaitForSeconds(_pieceInstructor.ExecuteMove());
            yield return new WaitForSeconds(_pieceInstructor.ExecuteIncrease());
            yield return new WaitForSeconds(_pieceInstructor.ExecuteRemove());
            yield return new WaitForSeconds(GeneratePiece());
        }
        if (!_lost) {
            _inputStage = true;
        }
    }

    private bool CalculateMove(Movement movement)
    {
        return movement switch
        {
            Movement.UpRight => PerformMoveLogic(HexAxis.I, HexAxis.J, -1),
            Movement.Right => PerformMoveLogic(HexAxis.H, HexAxis.I, -1),
            Movement.DownRight => PerformMoveLogic(HexAxis.J, HexAxis.I, -1),
            Movement.DownLeft => PerformMoveLogic(HexAxis.I, HexAxis.J, 1),
            Movement.Left => PerformMoveLogic(HexAxis.H, HexAxis.I, 1),
            Movement.UpLeft => PerformMoveLogic(HexAxis.J, HexAxis.I, 1),
            _ => false
        };
    }

    private bool PerformMoveLogic(HexAxis rowAxis, HexAxis columnAxis, int direction) {
        bool changeHappened = false;
        HexCoordinates c = new() { AAxis = rowAxis, BAxis = columnAxis };
        for (c.A = 0; c.A < _hexagon.Diameter; c.A++) {
            int start = direction == 1 ? _hexagon.RowMin(c) : _hexagon.RowMax(c);
            int end = direction == 1 ? _hexagon.RowMax(c) + 1 : _hexagon.RowMin(c) - 1;
            HexCoordinates lastSeen = c;
            lastSeen.B = -1;
            HexCoordinates otherMatching = lastSeen;
            for (c.B = start; c.B != end; c.B += direction) {
                Piece cur = _hexagon[c];
                if (cur == null) {
                    continue;
                }
                if (lastSeen.B == -1) {
                    lastSeen.B = start;
                    if (MovePieceLogic(c, lastSeen, cur)) changeHappened = true;
                }
                else if (cur.Stage == _hexagon[lastSeen].Stage) {
                    if (otherMatching.B != -1) {
                        Piece temp = _hexagon[otherMatching];
                        RemovePiece(temp);
                        RemovePiece(_hexagon[lastSeen]);
                        IncreasePiece(cur);
                        if (MovePieceLogic(otherMatching, lastSeen, temp)) changeHappened = true;
                        if (MovePieceLogic(c, lastSeen, cur)) changeHappened = true;
                        otherMatching.B = -1;
                    }
                    else {
                        otherMatching.B = c.B;
                    }
                }
                else {
                    if (otherMatching.B != -1) {
                        lastSeen.B += direction;
                        if (MovePieceLogic(otherMatching, lastSeen)) changeHappened = true;
                    }
                    lastSeen.B += direction;
                    if (MovePieceLogic(c, lastSeen, cur)) changeHappened = true;
                    otherMatching.B = -1;
                }
            }
            if (otherMatching.B != -1) {
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
        if (start == end) return false;
        _hexagon[start] = null;
        _hexagon[end] = piece;
        _pieceInstructor.AddPieceToMove(piece, gridManager.GetTilePosition(end));
        return true;
    }

    private void IncreasePiece(Piece piece) {
        _pieceInstructor.AddPieceToIncrease(piece);
    }

    private void RemovePiece(Piece piece) {
        _pieceInstructor.AddPieceToRemove(piece);
        _pieces--;
    }

    private float GeneratePiece() {
        if (_pieces == _hexagon.Size) {
            Lose();
            return 0f;
        }
        int randomPosition;
        do {
            randomPosition = Random.Range(0, _hexagon.Size);
        } while (_hexagon[randomPosition] != null);
        CreatePiece(randomPosition);
        return 0.1f;
    }
    
    private Piece CreatePiece(int k) {
        Piece piece = Instantiate(piecePrefab, gridManager.GetTilePosition(k), Quaternion.identity).GetComponent<Piece>();
        _hexagon[k] = piece;
        _pieces++;
        int rng = Random.Range(1, 6);
        if (rng == 1) {
            _pieceInstructor.InstantPieceIncrease(piece);            
        }
        return piece;
    }

    private void Lose() {
        _lost = true;
    }
}
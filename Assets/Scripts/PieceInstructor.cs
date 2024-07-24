using UnityEngine;

public class PieceInstructor {
    private readonly Piece[] _piecesToIncrease;
    private int _increaseIndex;
    private readonly Piece[] _piecesToRemove;
    private int _removeIndex;
    private Settings _settings;
    
    public PieceInstructor(int maxAmount, Settings settings) {
        _settings = settings;
        _piecesToIncrease = new Piece[maxAmount];
        _piecesToRemove = new Piece[maxAmount];
        _increaseIndex = 0;
        _removeIndex = 0;
    }

    public float ExecuteMove() {
        return 1 / _settings.speed;
    }

    public float ExecuteIncrease() {
        for (int i = 0; i < _increaseIndex; i++) {
            _piecesToIncrease[i].IncreaseStage();
        }
        _increaseIndex = 0;
        return 0f;
    }

    public float ExecuteRemove() {
        for (int i = 0; i < _removeIndex; i++) {
            _piecesToRemove[i].Remove();
        }
        _removeIndex = 0;
        return 0f;
    }
    
    public void AddPieceToMove(Piece piece, Vector3 destination, float relativeDist) {
        piece.MoveTo(destination, relativeDist);
    }

    public void AddPieceToIncrease(Piece piece) {
        _piecesToIncrease[_increaseIndex++] = piece;
    }

    public void AddPieceToRemove(Piece piece) {
        _piecesToRemove[_removeIndex++] = piece;
    }

    public void InstantPieceIncrease(Piece piece) {
        piece.IncreaseStage();
    }
}

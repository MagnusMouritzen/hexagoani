using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the visual part of the game by manipulating all the pieces on the board.
/// </summary>
public class PieceInstructor {
    private readonly Piece[] _piecesToIncrease;
    private int _increaseIndex;
    private readonly Piece[] _piecesToRemove;
    private int _removeIndex;
    private readonly Settings _settings;
    
    public PieceInstructor(int maxAmount, Settings settings) {
        _settings = settings;
        _piecesToIncrease = new Piece[maxAmount];
        _piecesToRemove = new Piece[maxAmount];
        _increaseIndex = 0;
        _removeIndex = 0;
    }

    /// <summary>
    /// Returns the amount of time the piece movement will take. The pieces should have already been set into motion.
    /// </summary>
    /// <returns>Time in seconds for the movement to complete.</returns>
    public float WaitForMove() {
        return 1 / _settings.speed;
    }

    /// <summary>
    /// Increase the stage of all pieces marked for increase.
    /// </summary>
    /// <returns>The time the increase will take.</returns>
    public float ExecuteIncrease() {
        for (int i = 0; i < _increaseIndex; i++) {
            _piecesToIncrease[i].IncreaseStage();
        }
        _increaseIndex = 0;
        return 0f;
    }

    /// <summary>
    /// Removes all pieces marked for removal.
    /// </summary>
    /// <returns>The time the removal will take.</returns>
    public float ExecuteRemove() {
        for (int i = 0; i < _removeIndex; i++) {
            _piecesToRemove[i].Remove();
        }
        _removeIndex = 0;
        return 0f;
    }
    
    /// <summary>
    /// Gives a piece the command to move. This starts immediately, but will take time to complete.
    /// </summary>
    /// <param name="piece">The piece to move.</param>
    /// <param name="destination">The end of the movement.</param>
    /// <param name="relativeDist">The distance to travel relative to the longest possible distance.</param>
    public void MovePiece(Piece piece, Vector3 destination, float relativeDist) {
        piece.MoveTo(destination, relativeDist);
    }

    /// <summary>
    /// Marks piece for stage increase.
    /// </summary>
    /// <param name="piece">The piece to increase.</param>
    public void AddPieceToIncrease(Piece piece) {
        _piecesToIncrease[_increaseIndex++] = piece;
    }

    /// <summary>
    /// Marks piece for removal.
    /// </summary>
    /// <param name="piece">The piece to remove.</param>
    public void AddPieceToRemove(Piece piece) {
        _piecesToRemove[_removeIndex++] = piece;
    }

    /// <summary>
    /// Increase the stage of a piece immediately.
    /// </summary>
    /// <param name="piece">The piece to increase.</param>
    public void InstantPieceIncrease(Piece piece) {
        piece.IncreaseStage();
    }
}

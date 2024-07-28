using System;
using System.Collections.Generic;
using System.Linq;
using OK.Utility;
using UnityEngine;

/// <summary>
/// Reads and broadcasts input.
/// TODO: Convert to new input system.
/// </summary>
public class InputManager : MonoBehaviour {
    [SerializeField] private EnumDataContainer<MovementInputSelection, Movement> movementInputs;
    //public EnumDataContainer<int[], Movement> test;

    public delegate void MovementRegisteredEvenHandler(Movement movement);

    public event MovementRegisteredEvenHandler MovementRegistered;
    
    private void Update() {
        CheckMovementInput(Movement.UpLeft);
        CheckMovementInput(Movement.UpRight);
        CheckMovementInput(Movement.DownLeft);
        CheckMovementInput(Movement.DownRight);
        CheckMovementInput(Movement.Left);
        CheckMovementInput(Movement.Right);
    }

    private bool CheckMovementInput(Movement movement) {
        bool hasInput = movementInputs[movement].IsPressed();
        if (hasInput) SendInput(movement);
        return hasInput;
    }

    private void SendInput(Movement movement) {
        OnMovementRegistered(movement);
    }

    protected virtual void OnMovementRegistered(Movement movement) {
        MovementRegistered?.Invoke(movement);
    }

    [Serializable]
    private class MovementInputSelection {
        [SerializeField] private MovementInput[] options;

        public bool IsPressed() {
            return options.Any(movementInput => movementInput.IsPressed());
        }
    }

    [Serializable]
    private class MovementInput {
        [SerializeField] private KeyCode key;
        [SerializeField] private Some<KeyCode> prereqKey;

        public bool IsPressed() {
            if (!Input.GetKeyDown(key)) return false;
            if (prereqKey.isSome && !Input.GetKey(prereqKey.Value)) return false;
            return true;
        }
    }
}
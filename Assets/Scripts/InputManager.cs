using System;
using UnityEngine;

/// <summary>
/// Reads and broadcasts input.
/// </summary>
public class InputManager : MonoBehaviour {
    [SerializeField] private KeyCode up = KeyCode.UpArrow;
    [SerializeField] private KeyCode down = KeyCode.DownArrow;
    [SerializeField] private KeyCode right = KeyCode.RightArrow;
    [SerializeField] private KeyCode left = KeyCode.LeftArrow;

    [SerializeField] private KeyCode upRight = KeyCode.Keypad9;
    [SerializeField] private KeyCode altRight = KeyCode.Keypad6;
    [SerializeField] private KeyCode downRight = KeyCode.Keypad3;
    [SerializeField] private KeyCode upLeft = KeyCode.Keypad7;
    [SerializeField] private KeyCode altLeft = KeyCode.Keypad4;
    [SerializeField] private KeyCode downLeft = KeyCode.Keypad1;

    public delegate void MovementRegisteredEvenHandler(Movement movement);

    public event MovementRegisteredEvenHandler MovementRegistered;
    

    // TODO: Convert to new input system.
    private void Update() {
        if (Input.GetKey(up)) {
            if (Input.GetKeyDown(right)) {
                SendInput(Movement.UpRight);
            } else if (Input.GetKeyDown(left)) {
                SendInput(Movement.UpLeft);
            }
        } else if (Input.GetKey(down)) {
            if (Input.GetKeyDown(right)) {
                SendInput(Movement.DownRight);
            } else if (Input.GetKeyDown(left)) {
                SendInput(Movement.DownLeft);
            }
        } else if (Input.GetKeyDown(right)) {
            SendInput(Movement.Right);
        } else if (Input.GetKeyDown(left)) {
            SendInput(Movement.Left);
        } else {
            if (Input.GetKeyDown(upRight)) {
                SendInput(Movement.UpRight);
            } else if (Input.GetKeyDown(altRight)) {
                SendInput(Movement.Right);
            } else if (Input.GetKeyDown(downRight)) {
                SendInput(Movement.DownRight);
            } else if (Input.GetKeyDown(upLeft)) {
                SendInput(Movement.UpLeft);
            } else if (Input.GetKeyDown(altLeft)) {
                SendInput(Movement.Left);
            } else if (Input.GetKeyDown(downLeft)) {
                SendInput(Movement.DownLeft);
            }
        }
    }

    private void SendInput(Movement movement) {
        OnMovementRegistered(movement);
    }

    protected virtual void OnMovementRegistered(Movement movement) {
        MovementRegistered?.Invoke(movement);
    }
}
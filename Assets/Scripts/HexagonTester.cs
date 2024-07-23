using UnityEngine;

/// <summary>
/// A hacky unit test for Hexagon.
/// </summary>
public class HexagonTester : MonoBehaviour{
    private Hexagon<int> _hexagon;

    private void Start() {
        Debug.Log("Starting test...");
        _hexagon = new Hexagon<int>(5);
        Debug.Log("Size: " + _hexagon.Size);
        Debug.Log("Diameter: " + _hexagon.Diameter);
        if (!ReadWrite()) {
            Debug.Log("Failed ReadWrite");
            ReadWrite(true);
        }
        if (!ReadCoordinates()) {
            Debug.Log("Failed ReadCoordinates");
            ReadCoordinates(true);
        }
        //ConvertCoordinates();
        if (!KToHI()) {
            Debug.Log("Failed KToIJ");
            KToHI(true);
        }
    }

    private bool ReadWrite(bool v=false) {
        for (int k = 0; k < _hexagon.Size; k++) {
            _hexagon[k] = k;
        }
        for (int k = 0; k < _hexagon.Size; k++) {
            if (_hexagon[k] != k) {
                if (v) {
                    Debug.Log("Failed reading " + k + ". Got " + _hexagon[k]);
                } else {
                    return false;                    
                }
            }
        }
        return true;
    }

    private bool ReadCoordinates(bool v = false) {
        int k = 0;
        HexCoordinates c = new() { A = 0, B = 0, AAxis = HexAxis.H, BAxis = HexAxis.I };
        for (c.A = 0; c.A < _hexagon.Diameter; c.A++) {
            for (c.B = _hexagon.RowMin(c); c.B <= _hexagon.RowMax(c); c.B++) {
                if (_hexagon[c] != k++) {
                    if (v) {
                        Debug.Log($"Failed reading k {k-1} c({c.A}, {c.B}). Got {_hexagon[c]}");
                    } else {
                        return false;
                    }
                }
            }
        }
        if (v && k != _hexagon.Size) {
            Debug.Log("Failed reaching enough iterations. k = " + k);
        }
        return k == _hexagon.Size;
    }

    private void ConvertCoordinates() {
        HexAxis[] aS = { HexAxis.H, HexAxis.I, HexAxis.I, HexAxis.J, HexAxis.J };
        HexAxis[] bS = { HexAxis.J, HexAxis.J, HexAxis.H, HexAxis.H, HexAxis.I };
        for (int i = 0; i < 5; i++) {
            HexCoordinates c = new() {AAxis = aS[i], BAxis = bS[i] };
            Debug.Log("");
            Debug.Log($"Now testing aAxis {c.AAxis} and bAxis {c.BAxis}");
            for (c.A = 0; c.A < _hexagon.Diameter; c.A++) {
                for (c.B = _hexagon.RowMin(c); c.B <= _hexagon.RowMax(c); c.B++) {
                    Debug.Log($"({c.A}, {c.B}) is {_hexagon[c]}");
                }
            }
        }
    }

    private bool KToHI(bool v = false) {
        for (int k = 0; k < _hexagon.Size; k++) {
            if (k != _hexagon[_hexagon.KToHI(k)]) {
                if (v) {
                    Debug.Log($"For k = {k}, got ({_hexagon.KToHI(k).A}, {_hexagon.KToHI(k).B})");
                } else {
                    return false;                    
                }
            }
        }
        return true;
    }
}

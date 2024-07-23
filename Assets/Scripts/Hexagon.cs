using System;

/// <summary>
/// Represents a hexagonal grid of data.
/// Each tile can be represented in multiple ways. The K-axis is single dimensional and simply iterates the tiles.
/// The iteration starts at the top row at the left, goes towards the right until it starts over at the left of the next row.
/// Alternatively, two of the following three axis can be used to identify a tile. Each axis counts the "rows" in a direction.
/// I - | Up to down.
/// J - \ Top left to bottom right.
/// H - / Bottom left to top right.
/// </summary>
public class Hexagon<T> {
    private readonly T[] _data;  // One data point for each tile.
    private readonly int _layers;  // The amount of tiles from the centre and outwards, including centre; radius.
    private readonly int _missingCorner;  // If the corners were filled such that the hexagon would become a square, how many additional tiles would be in each corner?
    public readonly int Diameter;  // Longest distance across.

    public T this[int k] {
        get => _data[k];
        set => _data[k] = value;
    }

    public T this[HexCoordinates c]
    {
        get => _data[ToK(c)];
        set => _data[ToK(c)] = value;
    }

    public int Size => _data.Length;
    
    public Hexagon(int layers) {
        if (layers < 0) {
            throw new ArgumentOutOfRangeException(nameof(layers));
        }

        _layers = layers;
        Diameter = 2 * layers - 1;
        _missingCorner = layers * (layers - 1) / 2;
        _data = new T[3 * layers * (layers - 1) + 1];
    }
    
    /// <summary>
    /// Gets the one-dimensional K coordinate from two-dimensional coordinates.
    /// </summary>
    /// <param name="c">The two-dimensional coordinates on two of the three axis.</param>
    /// <returns>The K coordinate.</returns>
    public int ToK(HexCoordinates c) => IJToK(ToIJCoordinates(c));

    /// <summary>
    /// Tile coordinates to world space Y.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public float ToY(HexCoordinates c) => IJToY(ToIJCoordinates(c));
    
    /// <summary>
    /// Tile coordinates to world space X.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public float ToX(HexCoordinates c) => IJToX(ToIJCoordinates(c));

    public int RowMin(int row, HexAxis rowAxis, HexAxis indexAxis) {
        if (rowAxis == HexAxis.J || indexAxis == HexAxis.J) {
            return row < _layers ? 0 : row - _layers + 1;
        }

        return row < _layers ? _layers - row - 1 : 0;
    }
    
    public int RowMin(HexCoordinates c, bool aIsRow=true) {
        int row = aIsRow ? c.A : c.B;
        if (c.AAxis == HexAxis.J || c.BAxis == HexAxis.J) {
            return row < _layers ? 0 : row - _layers + 1;
        }

        return row < _layers ? _layers - row - 1 : 0;
    }

    public int RowMax(HexCoordinates c, bool aIsRow=true) {
        int row = aIsRow ? c.A : c.B;
        if (c.AAxis == HexAxis.J || c.BAxis == HexAxis.J) {
            return row >= _layers ? Diameter - 1 : row + _layers - 1;
        }

        return row >= _layers ? Diameter + _layers - row - 2 : Diameter - 1;
    }

    public int GetThirdCoordinate(HexCoordinates c){
        if (c.AAxis == HexAxis.J) return c.A - c.B + _layers - 1;
        if (c.BAxis == HexAxis.J) return c.B - c.A + _layers - 1;
        return c.A + c.B - _layers + 1;
    }
    
    public HexCoordinates KToIJ(int k) {
        int start = 0;
        int flip = 1;
        if (k > Size / 2) {
            start = Diameter - 1;
            flip = -1;
            k = Size - k - 1;
        }
        HexCoordinates c = new() { B = 0, AAxis = HexAxis.I, BAxis = HexAxis.J };
        // TODO: Burn this to ashes. Blow it to atoms. Put it in a rocket and launch it into space. Throw it into an active volcano.
        // A mathematical approach is likely slower than an iterative.
        float n = _layers;
        c.A = (int)(0.5f - n + Math.Sqrt(4 * n * n + 8 * k - 4 * n + 1) / 2);
        c.B = k - IJToK(c);
        c.A = start + flip * c.A;
        c.B = start + flip * c.B;
        return c;
    }
    
    private float IJToY(HexCoordinates c) {
        EnsureIJ(c);
        return -c.A * 0.8660254f;
    }

    private float IJToX(HexCoordinates c) {
        EnsureIJ(c);
        return c.B + 0.5f * (_layers - c.A - 1);
    }

    private int IJToK(HexCoordinates c) {
        EnsureIJ(c);
        int start = 0;
        int flip = 1;
        if (c.A >= _layers) {
            start = Size - 1;
            flip = -1;
            c.A = Diameter - 1 - c.A;
            c.B = Diameter - 1 - c.B;
        }
        return start + flip * ((c.A + _layers - 1) * (c.A + _layers) / 2 - _missingCorner + c.B);
    }

    private HexCoordinates ToIJCoordinates(HexCoordinates c) {
        if (c.AAxis == HexAxis.H) {
            c.A = GetThirdCoordinate(c);
            c.AAxis -= c.BAxis + 1;
        } else if (c.BAxis == HexAxis.H) {
            c.B = GetThirdCoordinate(c);
            c.BAxis -= c.AAxis + 1;
        }

        if (c.AAxis > c.BAxis) {
            (c.A, c.B) = (c.B, c.A);
            c.AAxis = HexAxis.I;
            c.BAxis = HexAxis.J;
        }
        return c;
    }

    /// <summary>
    /// Ensures that Hex coordinates are I and J.
    /// </summary>
    /// <param name="c">Hex coordinates to check.</param>
    /// <exception cref="ArgumentException"></exception>
    private static void EnsureIJ(HexCoordinates c) {
        if (c.AAxis != HexAxis.I || c.BAxis != HexAxis.J) {
            throw new ArgumentException("HexCoordinates must be I,J.");
        }
    }
}

/// <summary>
/// The two-dimensional axis for use with Hexagon.
/// </summary>
public enum HexAxis {
    I,
    J,
    H
}

/// <summary>
/// Two-dimensional coordinates for use with Hexagon.
/// </summary>
public struct HexCoordinates {
    public int A, B;
    public HexAxis AAxis, BAxis;

    /// <summary>
    /// Check if two HexCoordinates are completely identical.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool IsFullyEqual(HexCoordinates a, HexCoordinates b) => a.A == b.A && a.B == b.B && a.AAxis == b.BAxis && a.BAxis == b.BAxis;

    /// <summary>
    /// Check if two HexCoordinates have identical A and B.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool IsEqualValue(HexCoordinates a, HexCoordinates b) => a.A == b.A && a.B == b.B;

    public override string ToString()
    {
        return $"({AAxis}: {A}, {BAxis}: {B})";
    }
}

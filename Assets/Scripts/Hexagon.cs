using System;

/// <summary>
/// Represents a hexagonal grid of data.
/// Each tile can be represented in multiple ways. The K-axis is single dimensional and simply iterates the tiles.
/// The iteration starts at the top row at the left, goes towards the right until it starts over at the left of the next row.
/// Alternatively, two of the following three axis can be used to identify a tile. Each axis counts the "rows" in a direction.
/// H - | Up to down.
/// I - \ Top left to bottom right.
/// J - / Bottom left to top right.
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
    public int ToK(HexCoordinates c) => HIToK(ToHICoordinateValues(c));

    /// <summary>
    /// Tile coordinates to world space Y.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public float ToY(HexCoordinates c) => HIToY(ToHICoordinateValues(c));
    
    /// <summary>
    /// Tile coordinates to world space X.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public float ToX(HexCoordinates c) => HIToX(ToHICoordinateValues(c));
    
    /// <summary>
    /// Given some coordinates, where one is considered the "row", what is the lowest value the other axis can have while keeping the row value constant?
    /// </summary>
    /// <param name="c">Hex coordinates</param>
    /// <param name="aIsRow">If true, the first axis is considered the row, otherwise the other one is.</param>
    /// <returns>The lowest value the non-row value can have on that row.</returns>
    public int RowMin(HexCoordinates c, bool aIsRow=true) {
        int row = aIsRow ? c.A : c.B;
        if (c.AAxis == HexAxis.I || c.BAxis == HexAxis.I) {
            return row < _layers ? 0 : row - _layers + 1;
        }

        return row < _layers ? _layers - row - 1 : 0;
    }

    /// <summary>
    /// Given some coordinates, where one is considered the "row", what is the highest value the other axis can have while keeping the row value constant?
    /// </summary>
    /// <param name="c">Hex coordinates</param>
    /// <param name="aIsRow">If true, the first axis is considered the row, otherwise the other one is.</param>
    /// <returns>The highest value the non-row value can have on that row.</returns>
    public int RowMax(HexCoordinates c, bool aIsRow=true) {
        int row = aIsRow ? c.A : c.B;
        if (c.AAxis == HexAxis.I || c.BAxis == HexAxis.I) {
            return row >= _layers ? Diameter - 1 : row + _layers - 1;
        }

        return row >= _layers ? Diameter + _layers - row - 2 : Diameter - 1;
    }

    /// <summary>
    /// Given coordinates with two axis, get the value of the third axis.
    /// </summary>
    /// <param name="c">Hex coordinates.</param>
    /// <returns>Value of third axis.</returns>
    public int GetThirdCoordinate(HexCoordinates c){
        if (c.AAxis == HexAxis.I) return c.A - c.B + _layers - 1;
        if (c.BAxis == HexAxis.I) return c.B - c.A + _layers - 1;
        return c.A + c.B - _layers + 1;
    }
    
    /// <summary>
    /// Transforms a coordinate in the one-dimensional K-axis to two dimensions (HI). 
    /// </summary>
    /// <param name="k">K coordinate.</param>
    /// <returns>HI coordinates</returns>
    public HexCoordinates KToHI(int k) {
        int start = 0;
        int flip = 1;
        if (k > Size / 2) {
            start = Diameter - 1;
            flip = -1;
            k = Size - k - 1;
        }
        HexCoordinates c = new() { B = 0, AAxis = HexAxis.H, BAxis = HexAxis.I };
        // TODO: Burn this to ashes. Blow it to atoms. Put it in a rocket and launch it into space. Throw it into an active volcano.
        // A mathematical approach is likely slower than an iterative.
        float n = _layers;
        c.A = (int)(0.5f - n + Math.Sqrt(4 * n * n + 8 * k - 4 * n + 1) / 2);
        c.B = k - HIToK(c.Values);
        c.A = start + flip * c.A;
        c.B = start + flip * c.B;
        return c;
    }
    
    /// <summary>
    /// Coordinates to Y in world space.
    /// </summary>
    /// <param name="c">Hex coordinate values in HI.</param>
    /// <returns>Y</returns>
    private float HIToY(HexCoordinateValues c) {
        return -c.A * 0.8660254f;
    }

    /// <summary>
    /// Coordinates to X in world space.
    /// </summary>
    /// <param name="c">Hex coordinate values in HI.</param>
    /// <returns>X</returns>
    private float HIToX(HexCoordinateValues c) {
        return c.B + 0.5f * (_layers - c.A - 1);
    }

    /// <summary>
    /// HI coordinates to K-axis.
    /// </summary>
    /// <param name="c">Hex coordinates.</param>
    /// <returns>K coordinate.</returns>
    private int HIToK(HexCoordinateValues c) {
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

    /// <summary>
    /// Converts coordinates to HI axis.
    /// </summary>
    /// <param name="c">The coordinates in original form.</param>
    /// <returns>The HI-version of the coordinate values.</returns>
    private HexCoordinateValues ToHICoordinateValues(HexCoordinates c) {
        if (c.AAxis == HexAxis.J) {
            c.A = GetThirdCoordinate(c);
            c.AAxis -= c.BAxis + 1;
        } else if (c.BAxis == HexAxis.J) {
            c.B = GetThirdCoordinate(c);
            c.BAxis -= c.AAxis + 1;
        }

        if (c.AAxis > c.BAxis) {
            (c.A, c.B) = (c.B, c.A);
            c.AAxis = HexAxis.H;
            c.BAxis = HexAxis.I;
        }
        return c.Values;
    }
}

/// <summary>
/// Two-dimensional coordinates for use with Hexagon.
/// The axis of A and B are determined by AAxis and BAxis.
/// </summary>
public struct HexCoordinates
{
    public HexCoordinateValues Values;

    public int A
    {
        get => Values.A;
        set => Values.A = value;
    }
    public int B
    {
        get => Values.B;
        set => Values.B = value;
    }
    public HexAxis AAxis, BAxis;

    public static bool operator ==(HexCoordinates one, HexCoordinates two) => one.Values == two.Values && one.AAxis == two.AAxis && one.BAxis == two.BAxis;
    
    public static bool operator !=(HexCoordinates one, HexCoordinates two) => !(one == two);

    public override string ToString() {
        return $"({AAxis}: {A}, {BAxis}: {B})";
    }
}

/// <summary>
/// The two-dimensional axis for use with Hexagon.
/// </summary>
public enum HexAxis {
    H,
    I,
    J
}

/// <summary>
/// Coordinates on unspecified axis.
/// </summary>
public struct HexCoordinateValues {
    public int A, B;

    public static bool operator ==(HexCoordinateValues one, HexCoordinateValues two) => one.A == two.A && one.B == two.B;

    public static bool operator !=(HexCoordinateValues one, HexCoordinateValues two) => !(one == two);
}

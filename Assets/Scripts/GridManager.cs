using UnityEngine;

/// <summary>
/// Manages the visual board.
/// </summary>
public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab = null;
    [SerializeField] private float spacing = 1f;
    
    private Hexagon<Transform> _hexagon;

    /// <summary>
    /// Generates the board.
    /// </summary>
    /// <param name="layers">Hexagon size.</param>
    public void GenerateHexagon(int layers)
    {
        _hexagon = new Hexagon<Transform>(layers);
        HexCoordinates c = new() { A = layers - 1, B = _hexagon.Diameter - 1, AAxis = HexAxis.H, BAxis = HexAxis.I };
        float xOffset = -_hexagon.ToX(c) / 2;
        c.A = _hexagon.Diameter - 1;
        float yOffset = -_hexagon.ToY(c) / 2;
        
        for (c.A = 0; c.A < _hexagon.Diameter; c.A++)
        {
            for (c.B = _hexagon.RowMin(c, true); c.B <= _hexagon.RowMax(c, true); c.B++)
            {
                Transform tile = Instantiate(tilePrefab, new Vector3(_hexagon.ToX(c) + xOffset, _hexagon.ToY(c) + yOffset, 0f) * spacing, Quaternion.identity, transform).transform;
                _hexagon[c] = tile;
                tile.GetComponent<SpriteRenderer>().color = new Color((float)c.A / _hexagon.Diameter, (float)c.B / _hexagon.Diameter, (float)_hexagon.GetThirdCoordinate(c) / _hexagon.Diameter);
                tile.name = $"Tile ({c.A}, {c.B}, {_hexagon.GetThirdCoordinate(c)}) ({_hexagon.ToK(c)})";
            }
        }
    }

    /// <summary>
    /// Gets world position of a tile.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public Vector3 GetTilePosition(HexCoordinates c) => _hexagon[c].position;
    
    /// <summary>
    /// Gets world position of a tile.
    /// </summary>
    /// <param name="k">One dimensional coordinate.</param>
    /// <returns></returns>
    public Vector3 GetTilePosition(int k) => _hexagon[k].position;
}
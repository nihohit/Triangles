using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public enum Player { Blue, Red };

public class Grid : MonoBehaviour {
  public enum GridShape { Triangle, Hexagon };

  public GridShape shape;
  public int length;
  public GameObject currentPlayerText;
  public GameObject newGameButton;

  private readonly Dictionary<Vector2Int, Triangle> map = new Dictionary<Vector2Int, Triangle>();
  private Player currentPlayer = Player.Blue;
  private List<Triangle> playableTiles;
  private RaycastHit2D hit;
  private Triangle currentTriangle;
  private TMPro.TMP_Text currentPlayerTextMesh;

  void initializeTriangle(GameObject trianglePrefab) {
    var complement = length % 2;
    var lengthIsOdd = complement == 1;
    for (int i = 0; i < length; ++i) {
      var start = lengthIsOdd ? length - i - 1 : length - i - 1;
      for (int j = -start; j <= start; ++j) {
        var triangle = Instantiate(trianglePrefab).GetComponent<Triangle>();
        var key = new Vector2Int(j, i - (length / 2));
        triangle.Setup(key.x, key.y, Math.Abs(j + i) % 2 == complement, this);
        map.Add(key, triangle);
      }
    }
    if (lengthIsOdd) {
      map[new Vector2Int(0, -1)].SetOwner(Player.Blue);
      map[new Vector2Int(-1, 0)].SetOwner(Player.Blue);
      map[new Vector2Int(1, 0)].SetOwner(Player.Blue);
      map[new Vector2Int(0, 0)].SetOwner(Player.Red);
      map[new Vector2Int(1, -1)].SetOwner(Player.Red);
      map[new Vector2Int(-1, -1)].SetOwner(Player.Red);
    } else {
      map[new Vector2Int(0, -1)].SetOwner(Player.Blue);
      map[new Vector2Int(-1, 0)].SetOwner(Player.Blue);
      map[new Vector2Int(1, 0)].SetOwner(Player.Blue);
      map[new Vector2Int(0, 0)].SetOwner(Player.Red);
      map[new Vector2Int(1, -1)].SetOwner(Player.Red);
      map[new Vector2Int(-1, -1)].SetOwner(Player.Red);
    }
  }

  void initializeHexagon(GameObject trianglePrefab) {
    for (int i = 0; i < length; ++i) {
      var difference = length - i;
      for (int j = -difference - length + 1; j < difference + length; ++j) {
        var triangle = Instantiate(trianglePrefab).GetComponent<Triangle>();
        var key = new Vector2Int(j, i);
        triangle.Setup(key.x, key.y, Math.Abs(j + i) % 2 == 0, this);
        map.Add(key, triangle);

        triangle = Instantiate(trianglePrefab).GetComponent<Triangle>();
        key = new Vector2Int(j, -1 - i);
        triangle.Setup(key.x, key.y, Math.Abs(j + i) % 2 == 1, this);
        map.Add(key, triangle);
      }
    }
    map[new Vector2Int(0, -1)].SetOwner(Player.Blue);
    map[new Vector2Int(-1, 0)].SetOwner(Player.Blue);
    map[new Vector2Int(1, 0)].SetOwner(Player.Blue);
    map[new Vector2Int(0, 0)].SetOwner(Player.Red);
    map[new Vector2Int(1, -1)].SetOwner(Player.Red);
    map[new Vector2Int(-1, -1)].SetOwner(Player.Red);
  }

  // Start is called before the first frame update
  void Start() {
    currentPlayerTextMesh = currentPlayerText.GetComponent<TMPro.TMP_Text>();
    var prefab = Resources.Load<GameObject>("Triangle");
    switch (shape) {
      case GridShape.Triangle:
        initializeTriangle(prefab);
        break;
      case GridShape.Hexagon:
        initializeHexagon(prefab);
        break;
    }
    newGameButton.SetActive(false);
    startTurn();
  }

  private enum Direction { Right, Left, UpLeft, UpRight, DownLeft, DownRight };

  private Triangle.DisplayColor colorForPlayer() {
    return currentPlayer == Player.Blue ? Triangle.DisplayColor.Blue : Triangle.DisplayColor.Red;
  }

  private Vector2Int directionVector(Triangle triangle, Direction direction) {
    switch (direction) {
      case Direction.UpLeft:
        return triangle.inverted ? Vector2Int.up : Vector2Int.left;

      case Direction.UpRight:
        return triangle.inverted ? Vector2Int.up : Vector2Int.right;

      case Direction.Right:
        return Vector2Int.right;

      case Direction.Left:
        return Vector2Int.left;

      case Direction.DownRight:
        return triangle.inverted ? Vector2Int.right : Vector2Int.down;

      case Direction.DownLeft:
        return triangle.inverted ? Vector2Int.left : Vector2Int.down;
    }
    throw new Exception($"Unknown direction {direction}");
  }

  private Triangle nextTriangle(Triangle triangle, Direction direction) {
    return map.TryGetValue(triangle.Coords() + directionVector(triangle, direction), out Triangle nextTriangle) ? nextTriangle : null;
  }

  private IEnumerable<Triangle> trianglesToMatchInDirection(Triangle triangle, Direction direction) {
    var triangles = new List<Triangle>();
    var currentTriangle = triangle;

    while (currentTriangle != null) {
      triangles.Add(currentTriangle);
      currentTriangle = nextTriangle(currentTriangle, direction);
      if (currentTriangle?.owner == null) {
        return Enumerable.Empty<Triangle>();
      }
      if (currentTriangle.owner == currentPlayer) {
        return triangles.Any(foundTriangle => foundTriangle.owner != null) ? triangles : Enumerable.Empty<Triangle>();
      }
    }
    return Enumerable.Empty<Triangle>();
  }


  static readonly IEnumerable<Direction> kDirections = new[] {
    Direction.DownLeft, Direction.DownRight, Direction.Left, Direction.Right, Direction.UpLeft, Direction.UpRight };

  private IEnumerable<Triangle> trianglesToMatch(Triangle triangle) {
    return triangle.owner != null ? Enumerable.Empty<Triangle>() : kDirections.SelectMany(direction => trianglesToMatchInDirection(triangle, direction)).ToList();
  }

  private bool isPlayable(Triangle triangle) {
    return triangle.owner == null && trianglesToMatch(triangle).Any();
  }

  private IEnumerable<Triangle> getPlayableTiles() {
    return map.Values.Where(isPlayable).Distinct();
  }

  private void resetTriangles() {
    foreach (var resetTriangle in map.Values) {
      resetTriangle.ResetDisplayColor();
    }
    foreach (var triangle in playableTiles) {
      triangle.SetDisplayColor(Triangle.DisplayColor.Yellow);
    }
  }

  private void startTurn() {
    currentPlayer = currentPlayer == Player.Blue ? Player.Red : Player.Blue;
    playableTiles = getPlayableTiles().ToList();
    if (playableTiles.Count == 0) {
      var redTiles = map.Values.Count(triangle => triangle.owner == Player.Red);
      var blueTiles = map.Values.Count(triangle => triangle.owner == Player.Blue);
      currentPlayerTextMesh.text = $"{(redTiles == blueTiles ? "TIE!" : redTiles > blueTiles ? "Red Won!" : "Blue Won!")}\nRed tiles: {redTiles}\nBlue tiles: {blueTiles}";
      newGameButton.SetActive(true);
      return;
    }
    currentPlayerTextMesh.text = $"Current player: {(currentPlayer == Player.Blue ? "Blue" : "Red")}";
    resetTriangles();
  }

  void Update() {
    hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
    if (!hit.collider) {
      if (currentTriangle != null) {
        currentTriangle = null;
        resetTriangles();
      }
      return;
    }
    var triangle = hit.collider.GetComponent<Triangle>();
    if (triangle == currentTriangle) {
      return;
    }
    currentTriangle = triangle;
    resetTriangles();
    var color = colorForPlayer();
    foreach (var matchedTriangle in trianglesToMatch(triangle)) {
      matchedTriangle.SetDisplayColor(color);
    }
  }

  public void TriangleClicked(Triangle triangle) {
    if (!isPlayable(triangle)) {
      return;
    }
    foreach (var matchedTriangle in trianglesToMatch(triangle)) {
      matchedTriangle.SetOwner(currentPlayer);
    }
    startTurn();
  }

  public void StartNewGame() {
    Debug.Log("start");
    SceneManager.LoadScene("scenes/Start");
  }
}

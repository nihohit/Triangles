using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum Player { Blue, Red };

public class Grid : MonoBehaviour {
  public enum GridShape { Triangle };

  public GridShape shape;
  public int length;
  private readonly Dictionary<Vector2Int, Triangle> map = new Dictionary<Vector2Int, Triangle>();
  private Player currentPlayer = Player.Blue;
  private List<Triangle> playableTiles;
  private RaycastHit2D hit;
  private Triangle currentTriangle;

  void initializeTriangle(GameObject trianglePrefab) {
    for (int i = 0; i < length; ++i) {
      for (int j = -i; j <= i; ++j) {
        var triangle = Instantiate(trianglePrefab).GetComponent<Triangle>();
        triangle.Setup(j, i - (length / 2), Math.Abs(j + i) % 2 == 1, this);
        map.Add(new Vector2Int(j, i - (length / 2)), triangle);
        Debug.Log(new Vector2Int(j, i - (length / 2)));
      }
    }
    map[new Vector2Int(1, 0)].SetOwner(Triangle.Owner.Blue);
    map[new Vector2Int(-1, 0)].SetOwner(Triangle.Owner.Red);
    map[new Vector2Int(-1, 1)].SetOwner(Triangle.Owner.Blue);
    map[new Vector2Int(1, 1)].SetOwner(Triangle.Owner.Red);
    map[new Vector2Int(1, -1)].SetOwner(Triangle.Owner.Red);
    map[new Vector2Int(-1, -1)].SetOwner(Triangle.Owner.Blue);
    map[new Vector2Int(-1, 2)].SetOwner(Triangle.Owner.Red);
    map[new Vector2Int(1, 2)].SetOwner(Triangle.Owner.Blue);
  }

  // Start is called before the first frame update
  void Start() {
    var prefab = Resources.Load<GameObject>("Triangle");
    switch (shape) {
      case GridShape.Triangle:
        initializeTriangle(prefab);
        break;
    }
    startTurn();
  }

  private IEnumerable<Triangle> getPlayableTiles() {
    return new List<Triangle>();
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
    if (!playableTiles.Any()) {
      //TODO end game
      return;
    }
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
    currentTriangle.SetDisplayColor(Triangle.DisplayColor.Yellow);
  }

  private Triangle.Owner ownerForPlayer() {
    return currentPlayer == Player.Blue ? Triangle.Owner.Blue : Triangle.Owner.Red;
  }

  public void TriangleClicked(Triangle triangle) {
    triangle.owner = ownerForPlayer();
    startTurn();
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Grid : MonoBehaviour {
  public enum GridShape { Triangle };

  public GridShape shape;
  public int length;

  void initializeTriangle(GameObject trianglePrefab) {
    for (int i = 0; i < length; ++i) {
      for (int j = -i; j <= i; ++j) {
        Debug.Log($"iterate over {i} {j}");
        var triangle = Instantiate(trianglePrefab).GetComponent<Triangle>();
        triangle.Setup(j, i - (length / 2), Math.Abs(j + i) % 2 == 1, this);
      }
    }
  }

  // Start is called before the first frame update
  void Start() {
    var prefab = Resources.Load<GameObject>("Triangle");
    switch (shape) {
      case GridShape.Triangle:
        initializeTriangle(prefab);
        break;
    }
  }

  // Update is called once per frame
  void Update() {

  }

  public void TriangleClicked(int xIndex, int yIndex) {
    Debug.Log($"{xIndex}, {yIndex}");
  }
}

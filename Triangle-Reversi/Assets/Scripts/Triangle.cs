using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : MonoBehaviour {
  public int xIndex;
  public int yIndex;
  public bool inverted;

  private const float kSpacing = 0.05F;
  private const float kScale = 1F / 3F;
  private Grid grid;

  // Start is called before the first frame update
  void Start() {
    this.transform.localScale = new Vector3(kScale, kScale, 1);
  }

  void OnMouseDown() {
    grid.TriangleClicked(xIndex, yIndex);
  }

  public void Setup(int x, int y, bool inverted, Grid grid) {
    Debug.Log($"setup {x} {y}");
    this.xIndex = x;
    yIndex = y;
    this.inverted = inverted;
    this.grid = grid;
    if (inverted) {
      this.transform.eulerAngles = new Vector3(0, 0, 180);
    }
    this.name = $"Triangle-{x}-{y}";

    this.transform.position = new Vector3(xIndex * (0.5F + kSpacing), yIndex * (-1 - kSpacing), 0);
  }
}

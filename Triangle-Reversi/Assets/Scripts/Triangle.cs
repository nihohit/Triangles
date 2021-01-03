using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : MonoBehaviour {
  public enum DisplayColor { White, Yellow, Red, Blue };

  public int xIndex;
  public int yIndex;
  public bool inverted;
  public DisplayColor displayColor;
  public Player? owner;

  private const float kSpacing = 0.05F;
  private const float kScale = 1F / 3F;
  private Grid grid;
  private SpriteRenderer spriteRenderer;

  // Start is called before the first frame update
  void Awake() {
    this.transform.localScale = new Vector3(kScale, kScale, 1);
    spriteRenderer = GetComponent<SpriteRenderer>();
  }

  void OnMouseDown() {
    grid.TriangleClicked(this);
  }

  public void Setup(int x, int y, bool inverted, Grid grid) {
    this.xIndex = x;
    yIndex = y;
    this.inverted = inverted;
    this.grid = grid;
    if (inverted) {
      this.transform.eulerAngles = new Vector3(0, 0, 180);
    }
    this.name = $"Triangle {x} {y}";

    this.transform.position = new Vector3(xIndex * (0.5F + kSpacing), yIndex * (-1 - kSpacing), 0);
  }

  private Color colorForDisplayColor(DisplayColor color) {
    switch (color) {
      case DisplayColor.Blue:
        return Color.blue;
      case DisplayColor.Red:
        return Color.red;
      case DisplayColor.White:
        return Color.white;
      case DisplayColor.Yellow:
        return Color.yellow;
      default:
        return Color.white;
    }
  }

  private DisplayColor displayColorForOwner(Player? owner) {
    if (owner == null) {
      return DisplayColor.White;
    }
    switch (owner) {
      case Player.Blue:
        return DisplayColor.Blue;
      case Player.Red:
        return DisplayColor.Red;
      default:
        return DisplayColor.White;
    }
  }

  public void SetDisplayColor(DisplayColor color) {
    displayColor = color;
    spriteRenderer.color = colorForDisplayColor(color);
  }

  public void ResetDisplayColor() {
    SetDisplayColor(displayColorForOwner(owner));
  }

  public void SetOwner(Player owner) {
    this.owner = owner;
    ResetDisplayColor();
  }

  public Vector2Int Coords() {
    return new Vector2Int(xIndex, yIndex);
  }
}

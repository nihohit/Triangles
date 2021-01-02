using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : MonoBehaviour {
  public int xIndex;
  public int yIndex;
  public bool inverted;

  private const float kSpacing = 0.05F;
  private const float kScale = 1F / 3F;

  // Start is called before the first frame update
  void Start() {
    this.transform.localScale = new Vector3(kScale, kScale, 1);
    if (inverted) {
      this.transform.eulerAngles = new Vector3(0, 0, 180);
    }

    this.transform.position = new Vector3(xIndex * (0.5F + kSpacing), yIndex * (1 + kSpacing), 0);
  }

  void OnMouseDown() {
    Debug.Log($"{xIndex}, {yIndex}");
  }
}

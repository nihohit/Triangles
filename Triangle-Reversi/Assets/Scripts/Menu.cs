using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
  public void StartHexagonReversi() {
    SceneManager.LoadScene("scenes/HexagonReversi");
  }

  public void StartTriangleReversi() {
    SceneManager.LoadScene("scenes/TriangleReversi");
  }
}

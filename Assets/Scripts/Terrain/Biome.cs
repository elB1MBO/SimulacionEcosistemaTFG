using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain{

public class Biome : MonoBehaviour
{
  [Range(0,1)]
  public float height;
  public Color startColor;
  public Color endColor;
  public int numSteps;
}

}
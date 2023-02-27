using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain{

public class TerrainProps : MonoBehaviour
{
  public int size;
  public bool[,] walkable;
  public bool[,] shore;
  public Vector3[,] tileCenter;

  public TerrainProps(int size){
    this.size = size;
    tileCenter = new Vector3[size,size];
    walkable = new bool[size,size];
    shore = new bool[size,size];
  }
}

}
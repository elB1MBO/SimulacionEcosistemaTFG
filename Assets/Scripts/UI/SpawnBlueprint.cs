using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlueprint : MonoBehaviour
{
    public GameObject henBlueprint;
    public GameObject foxBlueprint;

    public void SpawnHenBlueprints()
    {
        Instantiate(henBlueprint);
    }
    public void SpawnFoxBlueprints()
    {
        Instantiate(foxBlueprint);
    }
}

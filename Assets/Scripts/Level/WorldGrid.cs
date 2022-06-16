using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class WorldGrid : Core
{
    #region [ PROPERTIES ]

    public int[] gridSize = new int[] { 0, 0 };
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	
    public void GetGridSize(List<FloorTile> tiles)
    {
        List<float> xVals = new List<float>();
        List<float> zVals = new List<float>();
        
        foreach (FloorTile tile in tiles)
        {
            float x = tile.transform.position.x;
            float z = tile.transform.position.z;
            if (!xVals.Contains(x))
            {
                xVals.Add(x);
            }
            if (!zVals.Contains(z))
            {
                zVals.Add(z);
            }
        }

        gridSize[0] = xVals.Count;
        gridSize[1] = zVals.Count;
    }
}

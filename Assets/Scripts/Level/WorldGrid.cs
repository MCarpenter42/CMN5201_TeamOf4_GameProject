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

    [SerializeField] float gridCellSize = 2.0f;
    private LevelObject[,] worldGrid;
    [SerializeField] Vector2Int worldGridSize;
    [SerializeField] Vector3 worldGridOffset;

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        GridSetup();
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    private void GridSetup()
    {
        int cellsX = worldGridSize.x;
        int cellsZ = worldGridSize.y;
        worldGrid = new LevelObject[cellsX, cellsZ];

        for (int i = 0; i < cellsX; i++)
        {
            for (int j = 0; j < cellsZ; j++)
            {
                worldGrid[i, j] = null;
            }
        }

        worldGridOffset[0] = -0.5f * (float)worldGridSize.x * gridCellSize;
        worldGridOffset[1] = -0.5f * (float)worldGridSize.x * gridCellSize;
    }

    public bool MoveObject(int[] gridPos, int[] direction)
    {
        bool moveSucceeded = true;

        int[] newPos = gridPos;
        newPos[0] += direction[0];
        newPos[1] += direction[1];

        if (newPos[0] >= 0 && newPos[0] < worldGrid.GetLength(0) && newPos[1] >= 0 && newPos[1] < worldGrid.GetLength(1))
        {
            if (worldGrid[newPos[0], newPos[1]] != null)
            {
                moveSucceeded = false;
            }
        }
        else
        {
            moveSucceeded = false;
        }

        if (moveSucceeded)
        {
            LevelObject moveObj = worldGrid[gridPos[0], gridPos[1]];
            worldGrid[gridPos[0], gridPos[1]] = null;
            worldGrid[newPos[0], newPos[1]] = moveObj;
        }

        return moveSucceeded;
    }
}

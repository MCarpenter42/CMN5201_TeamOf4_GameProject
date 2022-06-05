using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class LevelController : Core
{
    #region [ PROPERTIES ]

    [SerializeField] public float gridCellScale = 1.0f;
    [HideInInspector] public WorldGrid worldGrid;
    [HideInInspector] public List<LevelObject> levelObjects = new List<LevelObject>();
    [HideInInspector] public List<FloorTile> floorTiles = new List<FloorTile>();

    private Player player;
    private LevelObject startDoor;

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        GetComponents();
        Setup();
    }

    void Start()
    {
        player.transform.localPosition = startDoor.transform.localPosition;
        player.GetGridPos();
        player.Move(player.GetStartingMove(), 0.8f);
        player.ChangeFacing(player.GetStartingMove());
    }

    void Update()
    {

    }

    void FixedUpdate()
    {

    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ SETUP FUNCTIONS ]

    private void GetComponents()
    {
        worldGrid = gameObject.AddComponent<WorldGrid>();
        levelObjects = ArrayToList(FindObjectsOfType<LevelObject>());
        floorTiles = ArrayToList(FindObjectsOfType<FloorTile>());
        player = GameManager.Player;
        foreach (LevelObject obj in levelObjects)
        {
            if (obj.type == ObjectTypes.StartDoor)
            {
                startDoor = obj;
            }
        }
    }

    private void Setup()
    {
        worldGrid.GetGridSize(floorTiles);
        worldGrid.OffsetFromScale(gridCellScale);
        ObjectsToGrid();
        GetGridPositions();
        SetTileColours();
    }

    private void ObjectsToGrid()
    {
        foreach (LevelObject obj in levelObjects)
        {
            Vector3 pos = obj.transform.position;
            pos.x += worldGrid.gridOffset.x;
            pos.y += worldGrid.gridOffset.y;
            pos.z += worldGrid.gridOffset.z;
            obj.transform.position = pos;
        }
    }

    private void GetGridPositions()
    {
        foreach (LevelObject obj in levelObjects)
        {
            obj.GetGridPos();
        }
    }

    private void SetTileColours()
    {
        FloorTile[,] sortedTiles = new FloorTile[worldGrid.gridSize[0], worldGrid.gridSize[1]];
        Debug.Log(worldGrid.gridSize[0] + ", " + worldGrid.gridSize[1]);
        foreach (FloorTile tile in floorTiles)
        {
            int a = (int)tile.gridPos.x;
            int b = (int)tile.gridPos.z;
            //Debug.Log(a + ", " + b);
            sortedTiles[a, b] = tile;
        }

        int index = 0;
        for (int i = 0; i < sortedTiles.GetLength(0); i++)
        {
            for (int j = 0; j < sortedTiles.GetLength(1); j++)
            {
                sortedTiles[i, j].SetTileMaterial(index);

                if (index == 0)
                {
                    index = 1;
                }
                else
                {
                    index = 0;
                }
            }
        }
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void LevelInputs()
    {
        float playerMoveTime = 0.25f;
        if (!player.isMoving)
        {
            if (GetInput(Controls.Movement.Up))
            {
                player.PlayerMove(Vector3.forward, playerMoveTime);
            }
            else if (GetInput(Controls.Movement.Down))
            {
                player.PlayerMove(-Vector3.forward, playerMoveTime);
            }
            else if (GetInput(Controls.Movement.Right))
            {
                player.PlayerMove(Vector3.right, playerMoveTime);
            }
            else if (GetInput(Controls.Movement.Left))
            {
                player.PlayerMove(-Vector3.right, playerMoveTime);
            }
        }
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

}

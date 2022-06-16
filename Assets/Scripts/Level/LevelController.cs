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

    public bool isGameplayLevel = true;
    [SerializeField] public float gridCellScale = 1.0f;
    [HideInInspector] public WorldGrid worldGrid;
    [HideInInspector] public List<LevelObject> levelObjects = new List<LevelObject>();
    [SerializeField] public bool useTileGrid = true;
    [HideInInspector] public List<FloorTile> floorTiles = new List<FloorTile>();

    private Player player;
    private StartPoint startPoint;

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
        if (isGameplayLevel)
        {
            if (player.useStartPath)
            {
                if (player.startPathPoints.Count > 0)
                {
                    player.FollowStartPath();
                }
            }
            else
            {
                player.transform.localPosition = startPoint.transform.localPosition;
                player.GetGridPos();
                Vector3 startingMove = player.GetGridDir(startPoint.GetFacing()).normalized * gridCellScale * (float)startPoint.moveMulti;
                player.Move(startingMove, 0.8f * (float)startPoint.moveMulti);
                player.ChangeFacing(startingMove);
            }
        }
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
        if (useTileGrid)
        {
            floorTiles = ArrayToList(FindObjectsOfType<FloorTile>());
        }
        player = GameManager.Player;
        startPoint = FindObjectOfType<StartPoint>();
    }

    private void Setup()
    {
        if (useTileGrid)
        {
            floorTiles = ArrayToList(FindObjectsOfType<FloorTile>());
            worldGrid.GetGridSize(floorTiles);
        }
        GetGridPositions();
        if (useTileGrid)
        {
            SetTileColours();
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
        foreach (FloorTile tile in floorTiles)
        {
            int a = (int)tile.gridPos.x;
            int b = (int)tile.gridPos.z;
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
        if (!player.isMoving)
        {
            if (GetInput(Controls.Movement.Up))
            {
                player.PlayerMove(Vector3.forward, player.moveTime);
            }
            else if (GetInput(Controls.Movement.Down))
            {
                player.PlayerMove(-Vector3.forward, player.moveTime);
            }
            else if (GetInput(Controls.Movement.Right))
            {
                player.PlayerMove(Vector3.right, player.moveTime);
            }
            else if (GetInput(Controls.Movement.Left))
            {
                player.PlayerMove(-Vector3.right, player.moveTime);
            }

            if (GetInputDown(Controls.Interaction.Interact) && !player.isMoving)
            {
                if (player.objectMoving != null)
                {
                    player.Release();
                }
                else if (player.canGrabFacing && player.objectMoving == null)
                {
                    player.Grab();
                }
            }

            if (player.objectMoving != null)
            {
                if (player.objectMoving.rotatable && !player.isMoving)
                {
                    if (GetInput(Controls.Interaction.RotateClockwise))
                    {
                        if (player.CanRotateAround(player.objectMoving.gridPos, 1.0f))
                        {
                            player.objectMoving.Rotate(1.0f, 1.0f);
                            player.RotateAround(player.objectMoving.gridPos, 1.0f, 1.0f);
                        }
                    }
                    else if (GetInput(Controls.Interaction.RotateCounterClockwise))
                    {
                        if (player.CanRotateAround(player.objectMoving.gridPos, -1.0f))
                        {
                            player.objectMoving.Rotate(-1.0f, 1.0f);
                            player.RotateAround(player.objectMoving.gridPos, -1.0f, 1.0f);
                        }
                    }
                }
            }
        }
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

}

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

    [Header("Level Properties")]
    public bool isGameplayLevel = true;
    public float levelFadeTime = 0.8f;
    [SerializeField] public float gridCellScale = 1.0f;
    [HideInInspector] public WorldGrid worldGrid;
    [HideInInspector] public List<LevelObject> levelObjects = new List<LevelObject>();
    [SerializeField] public bool useTileGrid = true;
    [HideInInspector] public List<FloorTile> floorTiles = new List<FloorTile>();

    private Player player;
    private StartPoint startPoint;
    private CameraController levelCam;

    [Header("Puzzle Properties")]
    [SerializeField][TextArea] string levelHintText = "";

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        GameManager.Instance.OnLevelLoad();
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
                player.DelayedPlayerMove(0.3f, startingMove, 0.25f);
                //player.ChangeFacing(startingMove);
            }

            LevelHint();

            if (GameManager.UIController.blackScreen != null)
            {
                GameManager.UIController.BlackScreenFade(false, levelFadeTime);
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
        player = FindObjectOfType<Player>();
        startPoint = FindObjectOfType<StartPoint>();
        levelCam = FindObjectOfType<CameraController>();
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
    
    private void LevelHint()
    {
        if (GameManager.UIController.hud.levelHint != null)
        {
            if (levelHintText != null && levelHintText.Length > 0)
            {
                Prompt levelHint = GameManager.UIController.hud.levelHint.GetComponent<Prompt>();
                float initialShowDelay = 1.0f;
                float initialHideDelay = initialShowDelay + levelHint.fixedShowHideDelay;
                levelHint.DoDelayedShow(true, initialShowDelay);
                levelHint.SetText(levelHintText, AdjustCondition.Never, AdjustCondition.Always);
                levelHint.DoDelayedShow(false, initialHideDelay);
            }
            else
            {
                GameManager.UIController.hud.levelHint.GetComponent<Prompt>().SetShown(false);
            }
        }
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void NextLevel()
    {
        StartCoroutine(DelayedSceneChange(levelFadeTime));
        if (GameManager.UIController.blackScreen != null)
        {
            GameManager.UIController.BlackScreenFade(true, levelFadeTime);
        }
    }

    private IEnumerator DelayedSceneChange(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        GoToScene('+');
    }

    public void PlayerInputs()
    {
        if (!player.isMoving && !levelCam.isRotating)
        {
            if (GetInput(Controls.Movement.Up))
            {
                switch (levelCam.facing)
                {
                    case CompassBearing.North:
                    default:
                        player.PlayerMove(Vector3.forward);
                        break;

                    case CompassBearing.East:
                        player.PlayerMove(Vector3.right);
                        break;

                    case CompassBearing.South:
                        player.PlayerMove(-Vector3.forward);
                        break;

                    case CompassBearing.West:
                        player.PlayerMove(-Vector3.right);
                        break;
                }
            }
            else if (GetInput(Controls.Movement.Down))
            {
                switch (levelCam.facing)
                {
                    case CompassBearing.North:
                    default:
                        player.PlayerMove(-Vector3.forward);
                        break;

                    case CompassBearing.East:
                        player.PlayerMove(-Vector3.right);
                        break;

                    case CompassBearing.South:
                        player.PlayerMove(Vector3.forward);
                        break;

                    case CompassBearing.West:
                        player.PlayerMove(Vector3.right);
                        break;
                }
            }
            else if (GetInput(Controls.Movement.Right))
            {
                switch (levelCam.facing)
                {
                    case CompassBearing.North:
                    default:
                        player.PlayerMove(Vector3.right);
                        break;

                    case CompassBearing.East:
                        player.PlayerMove(-Vector3.forward);
                        break;

                    case CompassBearing.South:
                        player.PlayerMove(-Vector3.right);
                        break;

                    case CompassBearing.West:
                        player.PlayerMove(Vector3.forward);
                        break;
                }
            }
            else if (GetInput(Controls.Movement.Left))
            {
                switch (levelCam.facing)
                {
                    case CompassBearing.North:
                    default:
                        player.PlayerMove(-Vector3.right);
                        break;

                    case CompassBearing.East:
                        player.PlayerMove(Vector3.forward);
                        break;

                    case CompassBearing.South:
                        player.PlayerMove(Vector3.right);
                        break;

                    case CompassBearing.West:
                        player.PlayerMove(-Vector3.forward);
                        break;
                }
            }
            else if (player.objectMoving != null)
            {
                SFXSource objSFX = player.objectMoving.gameObject.GetComponent<SFXSource>();
                if (objSFX.source.isPlaying)
                {
                    objSFX.Stop();
                }
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

    public void CameraInputs()
    {
        if (!levelCam.isRotating)
        {
            if (GetInputDown(Controls.Level.RotCamLeft))
            {
                levelCam.DoRotation(new Vector2Int(0, 1), 1.0f);
            }
            else if (GetInputDown(Controls.Level.RotCamRight))
            {
                levelCam.DoRotation(new Vector2Int(0, -1), 1.0f);
            }
        }

        if (!levelCam.isZooming)
        {
            if (GetInput(Controls.Level.ZoomCamIn))
            {
                levelCam.DoZoom(2, 0.4f);
            }
            else if (GetInput(Controls.Level.ZoomCamOut))
            {
                levelCam.DoZoom(-2, 0.4f);
            }
        }
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;

public class LevelController : Core
{
    #region [ PROPERTIES ]

    [Header("Level Properties")]
    public SceneType sceneType = SceneType.LevelGeneric;
    [HideInInspector] public bool isGameplayLevel { get { return (sceneType.ToString().Substring(0, 5) == "Level"); } }
    public float levelFadeTime = 0.8f;
    public bool useAudioMusic = true;
    public bool useAudioAtmospheric = false;
    public bool useAudioBreeze = false;
    public bool useAudioFauna = false;
    public bool useAudioFoliage = false;

    [Header("World Space Properties")]
    [SerializeField] public float gridCellScale = 1.0f;
    [HideInInspector] public WorldGrid worldGrid;
    [HideInInspector] public List<LevelObject> levelObjects = new List<LevelObject>();
    [SerializeField] public bool useTileGrid = true;
    [HideInInspector] public List<FloorTile> floorTiles = new List<FloorTile>();

    private int levelIndex = -1;

    private Player player;
    private StartPoint startPoint;
    private CameraController levelCam;

    [Header("Puzzle Properties")]
    [TextArea(1, 5)]
    [SerializeField] string levelHintText = "";

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
                player.DelayedPlayerMove(0.3f, startingMove, 0.25f);
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

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (GameManager.Instance.IsLevelScene(currentSceneIndex))
        {
            levelIndex = currentSceneIndex;
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
                levelHint.gameObject.SetActive(true);
                levelHint.SetText(levelHintText, AdjustCondition.Never, AdjustCondition.Always);
                levelHint.SetHiddenOffset(new Vector2(0.0f, levelHint.rTransform.rect.height + 20.0f));
                levelHint.rTransform.anchoredPosition += levelHint.hiddenOffset;

                float initialShowDelay = 1.0f;
                float initialHideDelay = initialShowDelay + levelHint.fixedShowHideDelay;
                levelHint.DoDelayedShow(true, initialShowDelay);
                levelHint.DoDelayedShow(false, initialHideDelay);
            }
            else
            {
                GameManager.UIController.hud.levelHint.GetComponent<Prompt>().SetShown(false);
            }
        }
    }

    public void RestartLevel()
    {
        GameManager.UIController.pauseMenu.Show(false);
    }

    public void OnLevelComplete()
    {
        GameManager.Instance.NextLevel();
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

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

}

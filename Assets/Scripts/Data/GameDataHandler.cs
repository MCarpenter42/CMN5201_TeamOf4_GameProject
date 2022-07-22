using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class GameDataHandler : Core
{
    #region [ OBJECTS ]

    public GameData DataMaster { get; private set; }
    private bool doSave = false;

    #endregion

    #region [ PROPERTIES ]



    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
    
    public GameDataHandler()
    {
        DataMaster = new GameData();
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]



    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ GAME STATE <-> LOCAL DATA ]

    public void GameStateToData()
    {
        if (doSave)
        {

        }
    }

    public void GameStateFromData()
    {

    }

    #endregion
    
    #region [ LOCAL DATA <-> SAVE FILES ]

    public void DataToDisk()
    {

    }

    public void DataFromDisk()
    {

    }

    #endregion
    
    #region [  ]



    #endregion

}

public class GameData
{
    public bool firstTimePlaying = true;
    public List<bool> levelsUnlocked = new List<bool>();
}

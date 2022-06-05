using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class FloorTile : LevelObject
{
    #region [ PROPERTIES ]

    [SerializeField] List<Material> materials;
    private MeshRenderer model;
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	#region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        OnAwake();
    }

    void Start()
    {
        OnStart();
    }
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	
    public void SetTileMaterial (int index)
    {
        model = GetComponentInChildren<MeshRenderer>(false);
        if (model != null)
        {
            if (InBounds(index, materials))
            {
                model.material = materials[index];
            }
            else
            {
                model.material = materials[0];
            }
        }
    }
}

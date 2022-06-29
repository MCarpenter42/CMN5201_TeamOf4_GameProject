using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class AudioTest : SFXSource
{
	#region [ PROPERTIES ]
	
	
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	#region [ BUILT-IN UNITY FUNCTIONS ]

    void Update()
    {
        if (Input.GetKeyDown("t"))
        {
            PlayAudioClip();
        }
    }

	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	
}

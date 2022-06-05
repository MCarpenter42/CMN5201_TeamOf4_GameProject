using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class Mirror : Core
{
	#region [ PROPERTIES ]
	
	
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public Vector3 ReflectedVector(Vector3 worldSpaceVect)
    {
		Vector3 localVect = transform.InverseTransformDirection(worldSpaceVect);
		localVect.z *= -1.0f;
		return transform.TransformDirection(localVect);
    }
}

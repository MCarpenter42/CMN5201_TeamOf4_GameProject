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

	[SerializeField] Axis reflectAxis = Axis.Z;
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	public Vector3 ReflectedVector(Vector3 worldSpaceVect)
    {
		Vector3 localVect = transform.InverseTransformDirection(worldSpaceVect);
		switch (reflectAxis)
        {
			case Axis.X:
				localVect.x *= -1.0f;
				break;

			case Axis.Y:
				localVect.y *= -1.0f;
				break;

			case Axis.Z:
			default:
				localVect.z *= -1.0f;
				break;
        }
		return transform.TransformDirection(localVect);
    }
}

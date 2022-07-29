using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class ListenerFollowPlayer : Core
{
    void Update()
    {
        if (GameManager.Player != null)
        {
            transform.position = GameManager.Player.transform.position + new Vector3(0.0f, 0.4f, 0.0f);
        }
        else
        {
            transform.position = Vector3.zero;
        }
    }
}

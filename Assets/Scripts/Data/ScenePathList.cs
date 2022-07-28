using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;

public class ScenePathList : MonoBehaviour
{
    public string timeModified;

    public string[] paths = new string[0];

    public int mainMenu = -1;
    public int levelTransition = -1;
    public int[] levels = new int[0];
}

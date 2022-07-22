using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;

public class Core : MonoBehaviour
{
    #region [ OBJECTS ]

	public static Controls Controls;
    public static VideoSettings VideoSettings;
    public static EncryptionHandler Encryption;

    #endregion

    #region [ ENUMERATION TYPES ]

    public enum Axis { X, Y, Z };
    public enum CompassBearing { North, East, South, West };
    public enum AdjustCondition { Never, Always, LessThan, GreaterThan };

    public enum ObjectTypes { Empty, Static, Dynamic, Player, StartPoint, EndPoint };
    public enum FloorTypes { Empty, Stone, Wood, Grass, Foliage, UnderWater };

    public enum BeamColours { White, Red, Green, Blue };


    public enum DebugDisplay { None, FPS };

    #endregion

    #region [ DELEGATES ]

    public delegate void VoidDelegate();

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ GAME STATE CONTROL ]

    public void Pause()
    {
        Time.timeScale = 0.0f;
        GameManager.isPaused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1.0f;
        GameManager.isPaused = false;
    }

    public void GoToScene(int targetSceneIndex)
    {
        GameManager.Instance.ChangeScene(targetSceneIndex);
    }
    
    public void GoToScene(char dir)
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        if (dir == '+')
        {
            index += 1;
        }
        else if (dir == '-')
        {
            index -= 1;
        }
        GameManager.Instance.ChangeScene(index);
    }

    public void Exit()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }

    #endregion

    #region [ MATHEMATICS & BOOLEAN LOGIC ]

    // I just added the ToRad and ToDeg functions for the sake of
    // my personal convenience.
    public static float ToRad(float degrees)
    {
        return degrees * Mathf.PI / 180.0f;
    }

    public static float ToDeg(float radians)
    {
        return radians * 180.0f / Mathf.PI;
    }

    public static int ToInt(bool intBool)
    {
        if (intBool)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public static int ToInt(bool intBool, int trueVal, int falseVal)
    {
        if (intBool)
        {
            return trueVal;
        }
        else
        {
            return falseVal;
        }
    }

    public static bool ToBool(int boolInt)
    {
        if (boolInt > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static Vector3 RestrictRotVector(Vector3 rotVect)
    {
        if (rotVect.x > 180.0f)
        {
            rotVect.x -= 360.0f;
        }
        else if (rotVect.x < -180.0f)
        {
            rotVect.x += 360.0f;
        }

        if (rotVect.y > 180.0f)
        {
            rotVect.y -= 360.0f;
        }
        else if (rotVect.y < -180.0f)
        {
            rotVect.y += 360.0f;
        }

        if (rotVect.z > 180.0f)
        {
            rotVect.z -= 360.0f;
        }
        else if (rotVect.z < -180.0f)
        {
            rotVect.z += 360.0f;
        }

        return rotVect;
    }
    
    public static float WrapClamp(float value, float min, float max)
    {
        float range = max - min;
        if (value < min)
        {
            float diff = min - value;
            int mult = (int)((diff - (diff % range)) / range) + 1;
            return value + (float)mult * range;
        }
        else if (value > max)
        {
            float diff = value - max;
            int mult = (int)((diff - (diff % range)) / range) + 1;
            return value - (float)mult * range;
        }
        else
        {
            return value;
        }
    }
    
    // This is just to make it easier to generate random integers
    public static int RandomInt(int valMin, int valMax)
    {
        float r = Random.Range(valMin, valMax + 1);
        int i = Mathf.FloorToInt(r);
        if (i > valMax)
        {
            i = valMax;
        }
        return i;
    }

    public static string[] StopwatchTime(float time)
    {
        int seconds = (int)Mathf.FloorToInt(time);
        int subSeconds = (int)Mathf.Floor((time - seconds) * 100.0f);

        int tMinutes = seconds - seconds % 60;
        int tSeconds = seconds % 60;

        string strMinutes = tMinutes.ToString();
        string strSeconds = tSeconds.ToString();
        string strSubSecs = subSeconds.ToString();

        if (strSeconds.Length < 2)
        {
            strSeconds = "0" + strSeconds;
        }
        if (strSubSecs.Length < 2)
        {
            strSubSecs = "0" + strSubSecs;
        }

        return new string[] { strMinutes, strSeconds, strSubSecs };
    }

    #endregion

    #region [ DATA STRUCTURE HANDLING ]

    public static bool InBounds<T>(int index, T[] array)
    {
        if (index > -1 && index < array.Length)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool InBounds<T>(int index, List<T> list)
    {
        if (index > -1 && index < list.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void ClearArray<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = default(T);
        }
    }

    public static List<T> ArrayToList<T>(T[] array)
    {
        List<T> listOut = new List<T>();
        foreach (T item in array)
        {
            listOut.Add(item);
        }
        return listOut;
    }

    public static void CopyListData<T>(List<T> source, List<T> destination)
    {
        for (int i = 0; i < source.Count; i++)
        {
            destination.Add(source[i]);
        }
    }

    public static T PickFromList<T>(List<T> itemList)
    {
        if (itemList.Count > 0)
        {
            int n = RandomInt(0, itemList.Count - 1);
            return itemList[n];
        }
        else
        {
            return default(T);
        }
    }

    #endregion

    #region [ OBJECT HANDLING ]

    public T GetOrAddComponent<T>(GameObject obj) where T : Component
    {
        if (obj.GetComponent<T>() != null)
        {
            return obj.GetComponent<T>();
        }
        else
        {
            return obj.AddComponent<T>();
        }
    }

    // I makes lists of children with a specific component often enough that
    // this was worth creating as a static function.
    public static List<GameObject> GetChildrenWithComponent<T>(GameObject parentObj)
    {
        List<GameObject> childrenWithComponent = new List<GameObject>();
        if (parentObj.transform.childCount > 0)
        {
            for (int i = 0; i < parentObj.transform.childCount; i++)
            {
                GameObject child = parentObj.transform.GetChild(i).gameObject;
                T childComponent;
                if (child.TryGetComponent<T>(out childComponent))
                {
                    childrenWithComponent.Add(child);
                }
            }
        }

        return childrenWithComponent;
    }

    // Pretty much the same deal here as with the "children with component" function.
    public static List<GameObject> GetChildrenWithTag(GameObject parentObj, string tag)
    {
        List<GameObject> childrenWithTag = new List<GameObject>();
        if (parentObj.transform.childCount > 0)
        {
            for (int i = 0; i < parentObj.transform.childCount; i++)
            {
                GameObject child = parentObj.transform.GetChild(i).gameObject;
                if (child.CompareTag(tag))
                {
                    childrenWithTag.Add(child);
                }
            }
        }

        return childrenWithTag;
    }
    
    // I makes lists of children with a specific component often enough that
    // this was worth creating as a static function.
    public static List<GameObject> GetListItemsWithComponent<T>(List<GameObject> objects)
    {
        List<GameObject> itemsWithComponent = new List<GameObject>();
        if (objects.Count > 0)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                GameObject item = objects[i];
                T itemComponent = item.GetComponent<T>();
                if (!itemComponent.Equals(null))
                {
                    itemsWithComponent.Add(item);
                }
            }
        }

        return itemsWithComponent;
    }

    // Pretty much the same deal here as with the "children with component" function.
    public static List<GameObject> GetListItemsWithTag(List<GameObject> objects, string tag)
    {
        List<GameObject> itemsWithTag = new List<GameObject>();
        if (objects.Count > 0)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                GameObject item = objects[0];
                if (item.CompareTag(tag))
                {
                    itemsWithTag.Add(item);
                }
            }
        }

        return itemsWithTag;
    }

    public static object GetProperty(object obj, string propertyName)
    {
        return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
    }

    public static T GetPropertyValue<T>(object obj, string propertyName)
    {
        return (T)obj.GetType().GetProperty(propertyName).GetValue(obj, null);
    }

    #endregion

    #region [ INPUT HANDLING ]

    public static bool GetInput(ControlInput input)
    {
        return Input.GetKey(input.Key);
    }
    
    public static bool GetInputDown(ControlInput input)
    {
        return Input.GetKeyDown(input.Key);
    }

    public static bool GetInputUp(ControlInput input)
    {
        return Input.GetKeyUp(input.Key);
    }

    #endregion

}

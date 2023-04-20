using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SceneManagerEditor : MonoBehaviour
{
    [MenuItem("Game/Progress/ResetLevelProgress")]
    public static void ResetLevelProgress()
    {
        PlayerPrefs.SetInt("level", 1);
    }
}

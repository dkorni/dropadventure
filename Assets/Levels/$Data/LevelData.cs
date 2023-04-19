using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    public Color DropLeftSliderColor;
    public Color HealthSliderColor;
}

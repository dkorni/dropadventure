using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    public int Index;
    public Color DropLeftSliderColor;
    public bool IsCutNeeded;
    public bool IsLaserNeeded;
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    public int Index;
    public int MinHealth;
    public Color DropLeftSliderColor;
    public Color HealthSliderColor;
    public bool IsCutNeeded;
    public bool IsLaserNeeded;
}

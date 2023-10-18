using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Settings", menuName = "ScriptableObjects/Game Settings")]
public class GameSettings : ScriptableObject
{
    public float animation_time = 0.1f;

    [Range(0f, 1f)] public float input_cooldown = 0.3f;
}

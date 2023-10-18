using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "ScriptableObjects/Level Settings")]
public class LevelSettings : ScriptableObject
{
    [Header("Board Settings"), Space]

    [Range(2, 14)] public int width = 2;
    [Range(2, 14)] public int height = 2;
    
    
    [Header("Spawn Settings"), Space]

    public List<GameObject> blocks;

    [Tooltip("Chance of copying the previous Block")]
    [Range(0, 100)] public int similarityChance = 0;

    private int _previousBlock = 0;


    [System.Serializable]
    public struct MatchRules
    {
        public int firstRule, secondRule, thirdRule;
    }

    [Header("Match Settings"), Space]

    [Tooltip("Ordered from small to big, First < Second < Third")]
    public MatchRules matchRules;


    private void OnValidate()
    {
        if (blocks == null || blocks.Count <= 0)
        {
            Debug.LogError("Blocks should not be empty!", this);
        }
    }

    /// <summary> Chooses a Random Block depending on similarity chance </summary>
    public GameObject GetRandomBlock()
    {
        int GetRandom()
        {
            return _previousBlock = Random.Range(0, blocks.Count);
        }

        if (similarityChance > 0)
            return (Random.Range(0, 101) <= similarityChance) ? blocks[_previousBlock] : blocks[GetRandom()];

        return blocks[GetRandom()];
    }

    /// <summary> Gets the correct sprite by comparing the Rules with the Group count. </summary>
    /// <param name="_groupCount">Amount of Elements in a group</param>
    public int GetSpriteIndex(int _groupCount)
    {
        if (_groupCount > matchRules.thirdRule)
            return 3;
        if (_groupCount > matchRules.secondRule)
            return 2;
        if (_groupCount > matchRules.firstRule)
            return 1;

        return 0;
    }
}

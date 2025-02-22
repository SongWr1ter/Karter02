using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName = "CharacterWeightSet", menuName = "CharacterWeight/CharacterWeightSet", order = 1)]
public class CharacterWeightSet : SerializedScriptableObject
{
    public List<CharacterWeight> WeightsList = new List<CharacterWeight>();
    [NonSerialized]private Dictionary<string,Bartending.Matrix4X5> WeightsDict = new Dictionary<string,Bartending.Matrix4X5>();

    public void Init()
    {
        #if UNITY_EDITOR
        foreach (var charc in WeightsList)
        {
            if (!WeightsDict.TryAdd(charc.Name, charc.Weight))
            {
                Debug.LogWarning("Duplicate weight: " + charc.Weight);
            }
        }
        #endif
    }

    public void printDict()
    {
        foreach (var x in WeightsDict)
        {
            Debug.Log(x.Key + " : " + x.Value);
        }
    }

    /// <summary>
    /// 第二个参数代表同一角色不同版本的心情矩阵
    /// </summary>
    /// <param name="key"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    [NonSerialized]private bool init = false;
    public Bartending.Matrix4X5 GetWeight(string key,int version = 1)
    {
        if (init == false)
        {
            init = true;
            foreach (var charc in WeightsList)
            {
                if (!WeightsDict.TryAdd(charc.Name, charc.Weight))
                {
                    Debug.LogWarning("Duplicate weight: " + charc.Weight);
                }
            }
        }
        if(version == 1)
            return WeightsDict.GetValueOrDefault(key);
        return WeightsDict.GetValueOrDefault(key + "V" + version);
    }
}

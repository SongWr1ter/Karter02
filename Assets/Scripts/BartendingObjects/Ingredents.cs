using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

public class Ingredents : SerializedMonoBehaviour
{
    public enum IngredentType
    {
        Base,
        Balance,
        Small
    }
    public IngredentType type;
    [HideInInspector]
    private Bartending.Vector5 input;
    public Bartending.Vector5 Input => input = new Bartending.Vector5(XYZ,WT);
    public string iName;
    [SerializeField]private Vector3 XYZ;
    [SerializeField]private Vector2 WT;
}

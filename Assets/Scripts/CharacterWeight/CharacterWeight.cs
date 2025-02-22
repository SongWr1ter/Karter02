using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName = "protype", menuName = "CharacterWeight/Protype")]
public class CharacterWeight : SerializedScriptableObject
{
    [ShowInInspector,TableMatrix(SquareCells = true,HorizontalTitle = "对每个属性影响的系数", VerticalTitle = "五种口味")]
    public float[,] Matrix = new float[4, 5];
    public string Name;
    private Bartending.Matrix4X5 weight;
    public Bartending.Matrix4X5 Weight => weight ??= new Bartending.Matrix4X5(Matrix);
}

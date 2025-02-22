using Sirenix.OdinInspector;
using UnityEngine;

public class js : SerializedMonoBehaviour
{
    [ShowInInspector]
    [Header("这个矩阵是被转置后显示出来的")]
    [TableMatrix(HorizontalTitle = "对每个属性影响的系数", VerticalTitle = "五种口味", SquareCells = true)]
    //SquareCells 为True，则其他的cell的宽高将于第一个cell的宽度相等
    public float[,] XXCelledMatrix = new float[4, 5];

    [ShowInInspector] [TableMatrix(SquareCells = true)]
    public float[] out4;
    [ShowInInspector] [TableMatrix(SquareCells = true)]
    public float[] in5;
    public float[] in5_base;
    public float[] in5_balence;
    public CharacterWeightSet characterWeightSet;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //PrintArray(DisPatchInRow(XCelledMatrix));
            Bartending.Matrix4X5 mat = new Bartending.Matrix4X5(XXCelledMatrix);
            // print(mat.ToString());
            // print("===In Col===");
            // PrintArray(DisPatchInCol(mat.ToArray()));
            // print("===In Row===");
            // PrintArray(DisPatchInRow(mat.ToArray()));
            
            in5= Bartending.Vector5.ToArray(Bartending.Reaction(new Bartending.Vector5(in5_base),new Bartending.Vector5(in5_balence),
                (a, b) => a + b));  
            var resl = Bartending.Matrix_4x5Multiply(new Bartending.Vector5(in5), mat);
            for (int i = 0; i < out4.Length; i++)
            {
                out4[i] = resl[i];
            }

            characterWeightSet.Init();
        }
    }
}

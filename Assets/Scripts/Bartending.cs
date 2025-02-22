using System;
using UnityEngine;

public abstract class Bartending
{
    #region Math

    public struct Vector5
    {
        public float x;
        public float y;
        public float z;
        public float w;
        public float t;

        public Vector5(float x, float y, float z, float w, float t)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
            this.t = t;
        }

        public Vector5(Vector3 v3, Vector2 v2)
        {
            this.x = v3.x;
            this.y = v3.y;
            this.z = v3.z;
            this.w = v2.x;
            this.t = v2.y;
        }

        public Vector5(Vector5 src)
        {
            this.x = src.x;
            this.y = src.y;
            this.z = src.z;
            this.w = src.w;
            this.t = src.t;
        }
        
        public Vector5(float[] values)
        {
            if (values == null || values.Length < 5)
            {
                Debug.LogWarning("Tried to set vector5 to index out of range.");
                this.x = 0;
                this.y = 0;
                this.z = 0;
                this.w = 0;
                this.t = 0;
            }
            this.x = values[0];
            this.y = values[1];
            this.z = values[2];
            this.w = values[3];
            this.t = values[4];
        }
        
        public override string ToString()
        {
            return $"({x}, {y}, {z}, {w},{t})";
        }

        public float this[int index]
        {
            get
            {
                if(index == 0)
                    return this.x;
                else if(index == 1)
                    return this.y;
                else if(index == 2)
                    return this.z;
                else if(index == 3)
                    return this.w;
                else if (index == 4)
                    return this.t;
                Debug.LogWarning("Tried to set vector5 to index out of range.");
                return 0;
            }
            set{
                if(index == 0)
                    this.x = value;
                else if(index == 1)
                    this.y = value;
                else if(index == 2)
                    this.z = value;
                else if(index == 3)
                    this.w = value;
                else if(index == 4)
                    this.t = value;
                else
                {
                    Debug.LogWarning("Tried to set vector5 to index out of range.");
                }
            }
        }
        
        public static float[] ToArray(Vector5 src)
        {
            float[] result = new float[5];
            for (int i = 0; i < 5; i++)
            {
                result[i] = src[i];
            }
            return result;
        }
        
        public static Vector5 operator+(Vector5 src1, Vector5 src2)
        {
            src1.x += src2.x;
            src1.y += src2.y;
            src1.z += src2.z;
            src1.w += src2.w;
            src1.t += src2.t;
            return src1;
        }

        public static void Clear(Vector5 src)
        {
            src.x = 0;
            src.y = 0;
            src.z = 0;
            src.w = 0;
            src.t = 0;
        }
    }

    // public class Vector4
    // {
    //     public float x;
    //     public float y;
    //     public float z;
    //     public float w;
    //     
    //     public Vector4(float x, float y, float z, float w, float t)
    //     {
    //         this.x = x;
    //         this.y = y;
    //         this.z = z;
    //         this.w = w;
    //     }
    //
    //     public Vector4()
    //     {
    //         this.x = 0;
    //         this.y = 0;
    //         this.z = 0;
    //         this.w = 0;
    //     }
    //
    //     public Vector4(Vector5 src)
    //     {
    //         this.x = src.x;
    //         this.y = src.y;
    //         this.z = src.z;
    //         this.w = src.w;
    //     }
    //
    //     public Vector4(float[] values)
    //     {
    //         if (values == null || values.Length < 4)
    //         {
    //             Debug.LogWarning("Tried to set vector4 to index out of range.");
    //             return;
    //         }
    //         this.x = values[0];
    //         this.y = values[1];
    //         this.z = values[2];
    //         this.w = values[3];
    //     }
    //
    //     public override string ToString()
    //     {
    //         return $"({x}, {y}, {z}, {w})";
    //     }
    //     public float this[int index]
    //     {
    //         get
    //         {
    //             if(index == 0)
    //                 return this.x;
    //             else if(index == 1)
    //                 return this.y;
    //             else if(index == 2)
    //                 return this.z;
    //             else if(index == 3)
    //                 return this.w;
    //             Debug.LogWarning("Tried to set vector5 to index out of range.");
    //             return 0;
    //         }
    //         set{
    //             if(index == 0)
    //                 this.x = value;
    //             else if(index == 1)
    //                 this.y = value;
    //             else if(index == 2)
    //                 this.z = value;
    //             else if(index == 3)
    //                 this.w = value;
    //             else
    //             {
    //                 Debug.LogWarning("Tried to set vector5 to index out of range.");
    //             }
    //         }
    //     }
    //     public static float[] ToArray(Vector4 src)
    //     {
    //         float[] result = new float[4];
    //         for (int i = 0; i < 4; i++)
    //         {
    //             result[i] = src[i];
    //         }
    //         return result;
    //     }
    // }

    public class Matrix4X5
    {
        private const int ROW = 4;
        private const int COL = 5;
        private float[,] data = new float[ROW,COL];

        public Matrix4X5()
        {
            for (int i = 0; i < ROW; i++)
            {
                for (int j = 0; j < COL; j++)
                {
                    data[i,j] = 0;
                }
            }
        }

        public Matrix4X5(float[,] data)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);
            if (rows == ROW && cols == COL)
            {
                float tmp;
                for (int i = 0; i < ROW; i++)
                {
                    for (int j = 0; j < COL; j++)
                    {
                        tmp = data[i,j];
                        this.data[i, j] = tmp;
                    }
                }
            }
        }

        public float this[int i, int j]
        {
            get
            {
                if(i < ROW && j < COL && i >=0 && j >= 0)
                    return data[i,j];
                Debug.LogWarning("Tried to set vector5 to index out of range.");
                return 0;
            }
            set
            {
                if(i < ROW && j < COL && i >=0 && j >= 0) 
                    data[i,j] = value;
                Debug.LogWarning("Tried to set vector5 to index out of range.");
            }
        }

        public override string ToString()
        {
            string output = "";
            float tmp;
            for (int i = 0; i < ROW; i++)
            {
                for (int j = 0; j < COL; j++)
                {
                    tmp = data[i,j];
                    output += tmp.ToString("F2") + " ";
                }
                output += "\n";
            }
            return output;
        }

        public float[,] ToArray()
        {
            return data;
        }
    }
    public static Vector4[] DisPatchInCol(float[,] array)
    {
        int width = array.GetLength(0);
        int height = array.GetLength(1);
        Vector4[] array2 = new Vector4[height];
        for (int i = 0; i < height; i++)
        {
            Vector4 tmp = new Vector4();
            for (int j = 0; j < width; j++)
            {
                tmp[j] = array[j, i];
            }
            array2[i] = tmp;
        }
        return array2;
    }
    
    public static Vector5[] DisPatchInRow(float[,] array)
    {
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);
        Vector5[] array2 = new Vector5[rows];
        for (int i = 0; i < rows; i++)
        {
            Vector5 tmp = new Vector5();
            for (int j = 0; j < cols; j++)
            {
                tmp[j] = array[i, j];
            }
            array2[i] = tmp;
        }
        return array2;
    }

    public static void PrintArray(Vector4[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Debug.Log(array[i].ToString());
        }
    }
    public static void PrintArray(Vector5[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Debug.Log(array[i].ToString());
        }
    }
    public static void PrintArray(Vector4 array)
    {
        Debug.Log(array.ToString());
    }
    public static void PrintArray(Vector5 array)
    {
        Debug.Log(array.ToString());
    }
    public static Vector4 Matrix_4x5Multiply(Vector5 r, Matrix4X5 W)
    {
        Vector4 e = new Vector4();
        for (int i = 0; i < 4; i++)
        {
            float tmp=0;
            for (int j = 0; j < 5; j++)
            {
                tmp += W[i, j] * r[j];
            }
            e[i] = tmp;
        }
        return e;
    }

   

    
    #endregion

    #region Reaction

    public static Vector5 Reaction(Vector5 a, Vector5 b,Func<float,float,float> f)
    {
        Vector5 resl = new Vector5();
        for (int i = 0; i < 5; i++)
        {
            resl[i] = f(a[i], b[i]);
        }
        return resl;
    }

    #endregion
    
    
}
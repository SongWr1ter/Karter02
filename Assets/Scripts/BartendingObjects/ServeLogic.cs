using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServeLogic : MonoBehaviour
{
    public CharacterWeightSet charcSet;
    private Bartending.Matrix4X5 weight = new Bartending.Matrix4X5();
    private void OnEnable()
    {
        MessageCenter.AddListener(OnServe,MESSAGE_TYPE.ServeItUp);
        MessageCenter.AddListener(OnCharcSelc,MESSAGE_TYPE.CharcSelc);
    }

    private void OnDisable()
    {
        MessageCenter.RemoveListener(OnServe,MESSAGE_TYPE.ServeItUp);
        MessageCenter.RemoveListener(OnCharcSelc,MESSAGE_TYPE.CharcSelc);
    }

    public Vector4 ServeItUp(Bartending.Vector5 input)
    {
        return Bartending.Matrix_4x5Multiply(input,weight);
    }

    public void OnServe(CommonMessage msg)
    {
        var resl = ServeItUp((Bartending.Vector5)msg.content);
        MessageCenter.SendMessage(new CommonMessage
        {
            content = resl
        },MESSAGE_TYPE.DrinkAll);
    }
    
    public void OnCharcSelc(CommonMessage msg)
    {
        string name = (string)msg.content;
        weight = charcSet.GetWeight(name);
    }
}

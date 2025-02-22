using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.DialogueTrees;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class testTrigger : SerializedMonoBehaviour
{
    [SerializeField]private DialogueTreeController dialogueTree;

    public int Power = 50;
    

   

    public void say()
    {
        dialogueTree.StartDialogue();
    }

    public void forward()
    {
       
    }

    /// <summary>
    /// make charc move
    /// </summary>
    public void AnimationMove()
    {
        print(1);
    }

    
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    public CharacterWeightSet characterWeightSet;
    private TMP_Dropdown dropdown;
    // Start is called before the first frame update
    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        
        dropdown.ClearOptions();
        foreach (var c in characterWeightSet.WeightsList)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData() { text = c.Name });
        }
        dropdown.RefreshShownValue();
        OnValueChanged(0);
    }

    private void OnEnable()
    {
        dropdown?.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnDisable()
    {
        dropdown.onValueChanged.RemoveListener(OnValueChanged);
    }

    public void OnValueChanged(int index)
    {
        if(index < 0) return;
        string name = dropdown.options[index].text;
        MessageCenter.SendMessage(new CommonMessage
        {
            content = name
        },MESSAGE_TYPE.CharcSelc);
    }
}

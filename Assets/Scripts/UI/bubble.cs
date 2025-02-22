using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class bubble : MonoBehaviour
{
    private TMP_Text text;
    private TMP_Text _name;
    private Image icon;

    public TMP_Text Text
    {
        get
        {
            if(text == null)
                text = GetComponentsInChildren<TMP_Text>()[1];
            return text;
        }
    }

    public TMP_Text Name
    {
        get
        {
            if(_name == null)
                _name = GetComponentsInChildren<TMP_Text>()[0];
            return _name;
        }
    }

    public Image Icon
    {
        get
        {
            if (icon == null)
            {
                icon = GetComponentsInChildren<Image>()[1];
            }
            return icon;
        }
    }
}

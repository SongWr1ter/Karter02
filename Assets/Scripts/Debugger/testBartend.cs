using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testBartend : MonoBehaviour
{
    public List<GameObject> bars = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        setBar();
    }

    public void setBar()
    {
        foreach (var g in bars)
        {
            g.SetActive(!g.activeSelf);
        }
    }

    
}

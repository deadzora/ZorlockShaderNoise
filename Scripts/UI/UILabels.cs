using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILabels : MonoBehaviour
{
    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            text.text = "Billow";
        }
        if (Input.GetKeyDown("2"))
        {
            text.text = "Fractal Brownian Motion";
        }
        if (Input.GetKeyDown("3"))
        {
            text.text = "Ridged Multi Fractal";
        }
        if (Input.GetKeyDown("4"))
        {
            text.text = "Hybrid Multi Fractal";
        }
        if (Input.GetKeyDown("5"))
        {
            text.text = "Hybrid Ridged Multi Fractal";
        }
        if (Input.GetKeyDown("6"))
        {
            text.text = "Thermal Erosion";
        }
        if (Input.GetKeyDown("7"))
        {
            text.text = "Hydraulic Erosion";
        }
    }
}

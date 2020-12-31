// by Ryan McAlpine
// references Game Dev Guide @ YouTube

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    //[SerializeField] private int minimum;
    //[SerializeField] private int maximum;
    //private int current;
    
    //[SerializeField] private Image mask;
    [SerializeField] private Image fill;
    //[SerializeField] private Color color;

    void Start()
    {
        fill.fillAmount = 0f;
    }

    /*
    // Update is called once per frame
    void Update()
    {
        //GetCurrentFill();
    }

    
    void GetCurrentFill()
    {
        float currentOffset = current - minimum;
        float maximumOffset = maximum - minimum;
        float fillAmount = currentOffset / maximumOffset;
        fill.fillAmount = fillAmount;
        fill.color = color;
    }
    */

    public void SetFill( float amount )
    {
        fill.fillAmount = amount;
    }
}

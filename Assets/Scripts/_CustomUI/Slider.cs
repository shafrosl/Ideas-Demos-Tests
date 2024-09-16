using System;
using MyBox;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using Debug = Utility.Debug;

public class Slider : MonoBehaviour
{
    public Image Main, Decrement, Increment, Border;
    [MinValue(0), MaxValue(100)] private int currentValue; 
    [MinValue(0), MaxValue(100)] private int alteredValue;
    
    public float AnimationSpeed = 1f;
    public int CurrentValue => currentValue;
    public int AlteredValue => alteredValue;
    private void Update() => UpdateBars();
    public void ChangeMainValue(int value) => currentValue += value;
    public void ChangeTemporaryValue(int difference) => alteredValue = currentValue + difference;
    public void ResetAlteredValue()
    {
        alteredValue = 0;
        Decrement.fillAmount = Increment.fillAmount = 0;
        Decrement.color.Modify(a: 0.75f);
        Increment.color.Modify(a: 0.75f);
    }

    public void Reset()
    {
        alteredValue = 0;
        Main.fillAmount = Decrement.fillAmount = Increment.fillAmount = 0;
        Decrement.color.Modify(a: 0.75f);
        Increment.color.Modify(a: 0.75f);
    }

    public void Initialize(int value)
    {
        Reset();
        Main.fillAmount = currentValue = value;
    }

    private void UpdateBars()
    {
        var cValue = CurrentValue.ToFloat(100);
        var aValue = AlteredValue.ToFloat(100);
        if (Math.Abs(cValue - aValue) > 0)
        {
            if (aValue > cValue)
            {
                // increment
                if (Math.Abs(Increment.fillAmount - aValue) > 0)
                {
                    if (Math.Abs(Increment.fillAmount - aValue) > 0.001f)
                    {
                        Increment.fillAmount = Mathf.Lerp(Increment.fillAmount, aValue, Time.deltaTime * AnimationSpeed);
                    }
        
                    if (Math.Abs(Increment.fillAmount - aValue) < 0.001f)
                    {
                        Increment.fillAmount = aValue;
                    }
                }
                
                if (Decrement.fillAmount > 0)
                {
                    Decrement.fillAmount = Mathf.Lerp(Decrement.fillAmount, 0, Time.deltaTime * AnimationSpeed);
        
                    if (Math.Abs(Decrement.fillAmount) < 0.001f)
                    {
                        Decrement.fillAmount = 0;
                    }
                }
            }
            else if (aValue < cValue)
            {
                // decrement
                if (Math.Abs(Decrement.fillAmount - aValue) > 0)
                {
                    if (Math.Abs(Decrement.fillAmount - aValue) > 0.001f)
                    {
                        Decrement.fillAmount = Mathf.Lerp(Decrement.fillAmount, aValue, Time.deltaTime * AnimationSpeed);
                    }

                    if (Math.Abs(Decrement.fillAmount - aValue) < 0.001f)
                    {
                        Decrement.fillAmount = aValue;
                    }
                }
                
                if (Increment.fillAmount > 0)
                {
                    Increment.fillAmount = Mathf.Lerp(Increment.fillAmount, 0, Time.deltaTime * AnimationSpeed);
        
                    if (Math.Abs(Increment.fillAmount) < 0.001f)
                    {
                        Increment.fillAmount = 0;
                    }
                }
            }
        }
        
        // main
        if (Math.Abs(Main.fillAmount - cValue) > 0)
        {
            if (Math.Abs(Main.fillAmount - cValue) > 0.001f)
            {
                Main.fillAmount = Mathf.Lerp(Main.fillAmount, cValue, Time.deltaTime * AnimationSpeed);
            }

            if (Math.Abs(Main.fillAmount - cValue) < 0.001f)
            {
                Main.fillAmount = cValue;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Stat
{
    /// <summary>
    /// Referinta la bara pe care Stat o controleaza
    /// </summary>
    [SerializeField]
    private BarScript bar;

    /// <summary>
    /// Valoarea maxima a Stat
    /// </summary>
    [SerializeField]
    private float maxVal;

    /// <summary>
    /// Valoarea curenta a Stat
    /// </summary>
    [SerializeField]
    private float currentVal;

    /// <summary>
    /// Proprietate pentru accesarea si setarea valorii curente
    /// </summary>
    public float CurrentVal
    {
        get
        {
            return currentVal;
        }

        set
        {
            //Fixeaza valoarea curenta intre 0 si maxim
            this.currentVal = Mathf.Clamp(value,0,MaxVal);

            //Updateaza bara
            Bar.Value = currentVal;
        }
    }

    /// <summary>
    /// Proprietate pentru accesarea valorii maxime
    /// </summary>
    public float MaxVal
    {
        get
        {
            return maxVal;
        }

        set
        {
            //Updateaza valoarea maxima
            this.maxVal = value;

            //Updateaza bara
            Bar.MaxValue = maxVal;
        }
    }

    public BarScript Bar 
    {
        get
        {
            return bar;
        }
    }

    public void Initialize()
    {
        this.MaxVal = maxVal;
        this.CurrentVal = currentVal;
    }
}

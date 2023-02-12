using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarScript : MonoBehaviour
{
    /// <summary>
    /// Viteza de miscare a barei de viata
    /// </summary>
    private float fillAmount;

    [SerializeField]
    private float lerpSpeed;

    /// <summary>
    /// O referire la continutul barei (bara colorata)
    /// </summary>
    [SerializeField]
    private Image content;

    /// <summary>
    /// O referire la textul de pe bara
    /// </summary>
    [SerializeField]
    private Text valueText;

    /// <summary>
    /// Indica valoarea maxima a barei, poate indica viata maxima a inamicului
    /// </summary>
    public float MaxValue { get; set; }

    /// <summary>
    /// O proprietate pentru setarea valorii barei
    /// Asigura generarea unei noi cantitati de umplere
    /// </summary>
    public float Value
    {
        set
        {
            fillAmount = Map(value, 0, MaxValue, 0, 1);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleBar();
    }

    //Actualizeaza bara
    private void HandleBar()
    {
        //Daca avem o noua cantitate de umplere, atunci actualizam
        if (fillAmount != content.fillAmount)
        {
            //Se obtine o animatie lina de tranzitie asupra cantitatii de umplere
            content.fillAmount = Mathf.Lerp(content.fillAmount, fillAmount, Time.deltaTime * lerpSpeed);
        }
    }

    //Reseteaza valoarea barei de viata
    public void Reset()
    {
        content.fillAmount = 1;
        Value = MaxValue;
    }

    private float Map(float value, float inMin , float inMax, float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
}

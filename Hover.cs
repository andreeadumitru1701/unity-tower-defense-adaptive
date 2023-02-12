using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : Singleton<Hover>
{
    //Referinta la renderul sprite-ului imaginii
    private SpriteRenderer spriteRenderer;

    //Referinta la renderul raze
    private SpriteRenderer rangeSpriteRenderer;

    public bool IsVisible { get; private set; }

    // Use this for initialization
    void Start()
    {
        //Creeaza referinte la renderul sprite-urilor
        this.spriteRenderer = GetComponent<SpriteRenderer>();

        this.rangeSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Asigura urmarirea mouse-ului in cadrul nivelului de joc
        FollowMouse();
    }

    //Asigura urmarirea mouse-ului cu imaginea pentru hover
    private void FollowMouse()
    {
        if (spriteRenderer.enabled)
        {
            //Traduce pozitia mouse-ului pe ecran in pozitie globala
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //Reseteaza valoarea Z la 0
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
    }

    
    /// <summary>
    /// Activeaza imaginea pentru hover
    /// </summary>
    /// <param name="sprite">Sprite-ul care apare la hover</param>
    public void Activate(Sprite sprite)
    {
        //Seteaza sprite-ul corect
        this.spriteRenderer.sprite = sprite;

        //Activeaza renderul
        spriteRenderer.enabled = true;

        rangeSpriteRenderer.enabled = true;

        IsVisible = true;
    }

    //Ascunde imaginea de hover
    public void Deactivate()
    {
        //Dezactiveaza renderul pentru a nu fi vizibil
        spriteRenderer.enabled = false;

        rangeSpriteRenderer.enabled = false;

        //Dezactiveaza butonul
        GameManager.Instance.ClickedBtn = null;

        IsVisible = false;

    }
}

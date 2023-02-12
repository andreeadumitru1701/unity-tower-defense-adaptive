using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileScript : MonoBehaviour
{
    public Point GridPosition { get; private set; }

    public bool IsEmpty { get; set; }

    private Tower myTower;

    private Color32 fullColor = new Color32(255, 118, 118, 255);

    private Color32 emptyColor = new Color32(96, 255, 90, 255);

    private SpriteRenderer spriteRenderer;

    public bool WalkAble { get; set; }

    public bool Debugging { get; set; }
    public Vector2 WorldPosition
    {
        get
        {
            return new Vector2(transform.position.x + (GetComponent<SpriteRenderer>().bounds.size.x / 2), transform.position.y - (GetComponent<SpriteRenderer>().bounds.size.y/2));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Seteaza placa din grila matriceala, alternatica unui constructor
    /// </summary>
    /// <param name="gridPos">Pozitia in grila a placii</param>
    /// <param name="worldPos">Pozitia placii in nivelul de joc</param>
    /// <param name="parent"></param>
    public void Setup(Point gridPos, Vector3 worldPos, Transform parent)
    {
        WalkAble = true;
        IsEmpty = true;
        this.GridPosition = gridPos;
        transform.position = worldPos;
        transform.SetParent(parent);
        LevelManager.Instance.Tiles.Add(gridPos, this);
    }
    
    //Mouseover este executat cand jucatorul trece cu mouse-ul peste o placa
    private void OnMouseOver()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && GameManager.Instance.ClickedBtn != null)
        {
            if (IsEmpty && !Debugging)//Coloreaza placa in verde
            {
                ColorTile(emptyColor);
            }
            if (!IsEmpty && !Debugging)//Coloreaza placa in rosu
            {
                ColorTile(fullColor);
            }
            else if (Input.GetMouseButtonDown(0))//Plaseaza un turn daca placa nu este ocupata
            {
                PlaceTower();
            }
        }
        else if (!EventSystem.current.IsPointerOverGameObject() && GameManager.Instance.ClickedBtn==null && Input.GetMouseButtonDown(0))
        {
            if (myTower != null)
            {
                GameManager.Instance.SelectTower(myTower);
            }
            else
            {
                GameManager.Instance.DeselectTower();
            }
        }
    }

    private void OnMouseExit()
    {
        if (!Debugging)
        {
            ColorTile(Color.white);
        }
    }

    //Plaseaza un turn pe placa
    private void PlaceTower()
    {
        WalkAble = false;

        if (AStar.GetPath(LevelManager.Instance.BlueSpawn, LevelManager.Instance.RedSpawn)==null)
        {
            //Nu exista un drum
            WalkAble = true;
            return;
        }

        //Creeaza turnul
        GameObject tower = Instantiate(GameManager.Instance.ClickedBtn.TowerPrefab, transform.position, Quaternion.identity);

        //Seteaza ordinea stratului pe care sta turnul
        tower.GetComponent<SpriteRenderer>().sortingOrder = GridPosition.Y;

        //Seteaza placa ca parinte de al turnului
        tower.transform.SetParent(transform);

        this.myTower = tower.transform.GetChild(0).GetComponent<Tower>();

        //Se asigura ca nu este goala
        IsEmpty = false;

        //Seteaza culoarea in alb
        ColorTile(Color.white);

        myTower.Price = GameManager.Instance.ClickedBtn.Price;

        //Cumpara turnul
        GameManager.Instance.BuyTower();

        WalkAble = false;
    }

    //Seteaza culoarea turnului
    private void ColorTile(Color newColor)
    {
        spriteRenderer.color = newColor;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private Stack<Node> path;

    private List<Debuff> debuffs = new List<Debuff>();

    private List<Debuff> debuffsToRemove = new List<Debuff>();

    private List<Debuff> newDebuffs = new List<Debuff>();

    [SerializeField]
    private Element elementType;

    private SpriteRenderer spriteRenderer;

    private int invulnerability = 2;

    [SerializeField]
    private Stat health;

    public bool Alive
    {
        get
        {
            return health.CurrentVal > 0;
        }
    }    

    private Animator myAnimator;

    //Pozitia inamicului pe grila
    public Point GridPosition { get; set; }

    //Destinatia inamicului
    private Vector3 destination;

    //Indica daca inamicul este activ
    public bool IsActive { get; set; }

    public float MaxSpeed { get; set; }

    public Element ElementType
    {
        get
        {
            return elementType;
        }
    }

    public float Speed 
    {
        get
        {
            return speed;
        }

        set
        {
            this.speed = value;
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        MaxSpeed = speed;
        health.Initialize();
    }

    private void Update()
    {
        HandleDebuffs();
        Move();
    }

    void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    //Genereaza inamicii in nivelul de joc
    public void Spawn(int health)
    {
        transform.position = LevelManager.Instance.BluePortal.transform.position;

        this.health.Bar.Reset();

        this.health.MaxVal = health;
        this.health.CurrentVal = this.health.MaxVal;

        //Incepe scalarea inamicilor
        StartCoroutine(Scale(new Vector3(0.1f, 0.1f), new Vector3(1, 1), false));

        //Seteaza drumul inamicilor
        SetPath(LevelManager.Instance.Path);
    }    

    /// <summary>
    /// Scaleaza monstri
    /// </summary>
    /// <param name="from">start scalare</param>
    /// <param name="to">final scalare</param>
    /// <returns></returns>
    public IEnumerator Scale(Vector3 from, Vector3 to, bool remove)
    {
        //Progresul scalarii
        float progress = 0;

        //Cat timp progresul este mai mic de 1, trebuie sa continuam scalarea
        while (progress <= 1)
        {
            //Scaleaza monstrul
            transform.localScale = Vector3.Lerp(from, to, progress);

            progress += Time.deltaTime;

            yield return null;
        }
        //Asigura ca scalarea este corecta la final
        transform.localScale = to;

        IsActive = true;

        if (remove)
        {
            Release();
        }
    }
  
    //Face inamicul sa se deplaseze pe drumul setat
    private void Move()
    {
        if (IsActive)
        {
            //Misca inamicul catre destinatie
            transform.position = Vector2.MoveTowards(transform.position, destination, Speed * Time.deltaTime);

            //Verifica daca inamicul a ajuns la destinatie
            if (transform.position == destination)
            {
                //Daca exista un drum si mai sunt noduri, continua sa se miste
                if (path != null && path.Count > 0)
                {
                    //Seteaza noua pozitie in matrice
                    GridPosition = path.Peek().GridPosition;

                    //Seteaza noua destinatie
                    destination = path.Pop().WorldPosition;
                }
            }
        }
    }

    /// <summary>
    /// Ofera inamicului drumul pe care sa mearga
    /// </summary>
    /// <param name="newPath">Noul drum al inamicului</param>
    /// <param name="active">Indica daca inamicul este activ</param>  
    private void SetPath(Stack<Node> newPath)
    {
        if (newPath != null) //Daca exista un drum
        {
            this.path = newPath; //Seteaza noul drum ca actualul drum

            GridPosition = path.Peek().GridPosition; //Seteaza noua pozitie din matrice

            destination = path.Pop().WorldPosition; //Seteaza noua destinatie
        }
    }

    /// <summary>
    /// Cand monstrul se loveste de ceva
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag =="RedPortal") //Daca monstrul se intersecteaza cu portalul rosu
        {
            //Incepe scalarea monstrului
            StartCoroutine(Scale(new Vector3(1, 1), new Vector3(0.1f, 0.1f), true));

            GameManager.Instance.Lives--;
        }

        if (other.tag=="Tile")
        {
            spriteRenderer.sortingOrder = other.GetComponent<TileScript>().GridPosition.Y;
        }
    }

    /// <summary>
    /// Elibereaza un monstru din joc in grupul de obiecte
    /// </summary>
    public void Release()
    {
        //Elimina toate debuff-urile
        debuffs.Clear();

        speed = MaxSpeed;

        //Dezactiveaza monstrul
        IsActive = false;

        //Asigura pozitia corecta de start
        GridPosition = LevelManager.Instance.BlueSpawn;

        //Elimina inamicul din joc
        GameManager.Instance.Pool.ReleaseObject(gameObject);

        //Elimina inamicul in grupul de obiecte
        GameManager.Instance.RemoveMonster(this);
    }

    public void TakeDamage(float damage, Element dmgSource)
    {
        if (IsActive)
        {
            if (dmgSource==ElementType)
            {
                damage = damage / invulnerability;
                invulnerability++;
            }

            health.CurrentVal -= damage;

            if (health.CurrentVal<=0)
            {
                SoundManager.Instance.PlaySfx("DieSoundEffect");

                GameManager.Instance.Currency += 2;

                myAnimator.SetTrigger("Die");

                IsActive = false;

                //GetComponent<SpriteRenderer>().sortingOrder--;
            }
        }
    }

    public void AddDebuff(Debuff debuff)
    {
        if (!debuffs.Exists(x=>x.GetType()==debuff.GetType()))
        {
            newDebuffs.Add(debuff);
        }
    }

    public void RemoveDebuff (Debuff debuff)
    {
        debuffsToRemove.Add(debuff);
    }

    private void HandleDebuffs()
    {
        if (newDebuffs.Count>0)
        {
            debuffs.AddRange(newDebuffs);

            newDebuffs.Clear();
        }

        foreach (Debuff debuff in debuffsToRemove)
        {
            debuffs.Remove(debuff);
        }

        debuffsToRemove.Clear();

        foreach (Debuff debuff in debuffs)
        {
            debuff.Update();
        }
    }
}

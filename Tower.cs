using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Element {STONE, FIRE, ICE, MAGIC, NONE}

public abstract class Tower : MonoBehaviour
{
    //Tipul proiectilului
    [SerializeField]
    private string projectileType;

    [SerializeField]
    private float projectileSpeed;

    [SerializeField]
    private int damage;

    [SerializeField]
    private float debuffDuration;

    [SerializeField]
    private float proc;

    public TowerUpgrade[] Upgrades { get; protected set; }

    public Element ElementType { get; protected set; }

    public int Price { get; set; }

    public float ProjectileSpeed
    {
        get { return projectileSpeed; }
    }


    //Renderul de sprite al turnului
    private SpriteRenderer mySpriteRenderer;

    //Tinta curenta a turnului
    private Monster target;

    //Nivelul curent al turnului
    public int Level { get; protected set; }

    public Monster Target
    {
        get { return target; }
    }

    public int Damage 
    {
        get
        {
            return damage;
        }
    }

    public float DebuffDuration
    {
        get
        {
            return debuffDuration;
        }
        set
        {
            this.debuffDuration = value;
        }
    }

    public float Proc
    {
        get
        {
            return proc;
        }

        set
        {
            this.proc = value;
        }
    }

    public TowerUpgrade NextUpgrade
    {
        get
        {
            if (Upgrades.Length>Level-1)
            {
                return Upgrades[Level - 1];
            }

            return null;
        }
    }

    private void Awake()
    {
        Level = 1;
    }

    //O coada cu monstri pe care turnul ii poate ataca
    private Queue<Monster> monsters = new Queue<Monster>();

    private bool canAttack = true;

    private float attackTimer;

    [SerializeField]
    private float attackCooldown;

    public void Set()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Set();
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
        Debug.Log(target);
    }

    //Selecteaza turnul
    public void Select()
    {
        mySpriteRenderer.enabled = !mySpriteRenderer.enabled;
        GameManager.Instance.UpdateUpgradeTip();
    }
    
    //Turnul ataca o tinta
    private void Attack()
    {
        if (!canAttack) //Daca nu poate ataca
        {
            //Numara cat timp a trecut de la ultimul atac
            attackTimer += Time.deltaTime;

            //Daca timpul trecut este mai mare decat timpul de cooldown, atunci se reseteaza si turnul ataca din nou
            if (attackTimer >= attackCooldown)
            {
                canAttack = true;
                attackTimer = 0;
            }
        }

        //Daca nu avem o tinta selectata dar avem mai multe tinte in raza
        if (target == null && monsters.Count > 0 && monsters.Peek().IsActive)
        {
            target = monsters.Dequeue();
        }

        if (target != null && target.IsActive) //Daca avem o tinta activa
        {
            if (canAttack) //Daca putem ataca atunci turnul ataca in tinta
            {
                Shoot();

                canAttack = false;
            }
        }

        if (target!=null && !target.Alive || target!=null && !target.IsActive)
        {
            target = null;
        }
    }

    public virtual string GetStats()
    {
        if (NextUpgrade!=null)
        {
            return string.Format("\nLevel: {0} \nDamage: {1} <color=#00ff00ff> +{4}</color> \nProc: {2}% <color=#00ff00ff>+{5}%</color> \nDebuff: {3} sec <color=#00ff00ff>+{6}</color>", Level,damage,proc,DebuffDuration,NextUpgrade.Damage,NextUpgrade.ProcChance,NextUpgrade.DebuffDuration);
        }

        return string.Format("\nLevel: {0} \nDamage: {1} \nProc: {2}% \nDebuff: {3} sec", Level, damage, proc, DebuffDuration);
    }

    /// <summary>
    /// Face turnul sa atace
    /// </summary>
    private void Shoot()
    {
        //Preia un proiectil din cele disponibile
        Projectile projectile = GameManager.Instance.Pool.GetObject(projectileType).GetComponent<Projectile>();

        //Asigura instantierea proiectilului pe pozitia turnului
        projectile.transform.position = transform.position;

        projectile.Initialize(this);
    }

    public virtual void Upgrade()
    {
        GameManager.Instance.Currency -= NextUpgrade.Price;
        Price += NextUpgrade.Price;
        this.damage += NextUpgrade.Damage;
        this.proc += NextUpgrade.ProcChance;
        this.debuffDuration += NextUpgrade.DebuffDuration;
        Level++;
        GameManager.Instance.UpdateUpgradeTip();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            monsters.Enqueue(other.GetComponent<Monster>()); //Adauga noi monstri in coada cand intra in raza de actiune
        }
    }

    public abstract Debuff GetDebuff();

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            //target = null;
            GameObject go = other.gameObject;

            if (go.activeInHierarchy)
            {
                target = null;
            }
        }
    }
}

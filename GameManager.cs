using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Delegate for the currency changed event
/// </summary>
public delegate void CurrencyChanged();

public class GameManager : Singleton<GameManager>
{
    /// <summary>
    /// An event that is triggered when the currency changes
    /// </summary>
    public event CurrencyChanged Changed;

    private TowerBtn clickedBtn;
    //A property for the clickedBtn
    public TowerBtn ClickedBtn { get; set; }

    //A reference to the currency text
    private int currency;

    private int wave = 0;

    private int difficulty = 1;

    private int lives;

    private bool gameOver = false;

    private int health=15;

    [SerializeField]
    private Text livesTxt;

    [SerializeField]
    private Text waveTxt;

    [SerializeField]
    private Text difficultyTxt;

    [SerializeField]
    private Text currencyTxt;

    [SerializeField]
    private GameObject waveBtn;

    [SerializeField]
    private GameObject gameOverMenu;

    [SerializeField]
    private GameObject upgradePanel;

    [SerializeField]
    private GameObject statsPanel;

    [SerializeField]
    private Text sellText;

    [SerializeField]
    private Text statTxt;

    [SerializeField]
    private Text upgradePrice;

    [SerializeField]
    private GameObject inGameMenu;

    [SerializeField]
    private GameObject optionsMenu;

    [SerializeField]
    private AudioClip spawn_sound;

    [SerializeField]
    private AudioClip death_sound;

    [SerializeField]
    private AudioClip pop_sound;

    [SerializeField]
    private AudioSource source;

    //The current selected tower
    private Tower selectedTower;

    private List<Monster> activeMonsters = new List<Monster>();

    //A property for the object pool
    public ObjectPool Pool { get; set; }

    public bool WaveActive
    {
        get
        {
            return activeMonsters.Count > 0;
        }
    }

    //Property for accessing the currency
    public int Currency
    {
        get
        {
            return currency;
        }

        set
        {
            this.currency = value;
            this.currencyTxt.text = value.ToString() + " <color=orange>Gold</color>";

            OnCurrencyChanged();
        }
    }

    public int Lives
    {
        get
        {
            return lives;
        }

        set
        {
            this.lives = value;

            if (lives <= 0)
            {
                this.lives = 0;
                GameOver();
            }

            livesTxt.text = lives.ToString();

        }
    }

    private void Awake()
    {
        Pool = GetComponent<ObjectPool>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Lives = 10;
        Currency = 10;
    }

    // Update is called once per frame
    void Update()
    {
        HandleEscape();
    }

    /// <summary>
    /// Pick a tower then buy button is pressed
    /// </summary>
    /// <param name="towerBtn">The clicked button</param>
    public void PickTower(TowerBtn towerBtn)
    {
        if (Currency >= towerBtn.Price && !WaveActive)
        {
            //Stores the clicked button
            this.ClickedBtn = towerBtn;

            SoundManager.Instance.PlaySfx("pop_sound");

            //Activates the hover icon
            Hover.Instance.Activate(towerBtn.Sprite);
        }
    }

    //Buys a tower
    public void BuyTower()
    {
        if (Currency >= ClickedBtn.Price)
        {
            SoundManager.Instance.PlaySfx("pop_sound");

            Currency -= ClickedBtn.Price;
            Hover.Instance.Deactivate();
        }

    }

    public void OnCurrencyChanged()
    {
        if (Changed!=null)
        {
            Changed();
        }
    }

    /// <summary>
    /// Selects a tower by clicking it
    /// </summary>
    /// <param name="tower">The clicked tower</param>
    public void SelectTower(Tower tower)
    {
        if (selectedTower != null) //If we have selected a tower
        {
            selectedTower.Select(); //Selects the tower
        }

        //Sets the selected tower
        selectedTower = tower;

        //Selects the tower
        selectedTower.Select();

        sellText.text = "+ " + (selectedTower.Price / 2).ToString() + " <color=orange>Gold</color>";

        upgradePanel.SetActive(true);
    }

    //Deselect the tower
    public void DeselectTower()
    {
        //If we have a selected tower
        if (selectedTower != null)
        {
            //Calls select to deselect it
            selectedTower.Select();
        }

        upgradePanel.SetActive(false);

        //Remove the reference tot the tower
        selectedTower = null;
    }

    //Handles escape presses
    private void HandleEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) //If we press escape
        {
            if (selectedTower==null && !Hover.Instance.IsVisible)
            {
                ShowInGameMenu();
            }
            else if (Hover.Instance.IsVisible)
            {
                DropTower();
            }
            else if (selectedTower!=null)
            {
                DeselectTower();
            }
        }
    }

    public void DifficultyCalculator()
    {
        if (lives > 8)
            difficulty += 3;
        else if (lives > 5)
            difficulty += 2;
        else
            difficulty -= 2;

        if (wave % 3 == 0)
            difficulty++;

        if (wave % 5 == 0)
            difficulty += 2;

        if (wave % 10 == 0)
            difficulty += 2;

        difficultyTxt.text = string.Format("Difficulty: <color=lime>{0}</color>", difficulty);

        health += difficulty / 2;
    }

    public void StartWave()
    {
        wave++;

        waveTxt.text = string.Format("Wave: <color=lime>{0}</color>", wave);

        StartCoroutine(SpawnWave());

        waveBtn.SetActive(false);
    }

    /// <summary>
    /// Spawns a wave of monsters
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnWave()
    {
        //Generates the path
        LevelManager.Instance.GeneratePath();

        DifficultyCalculator();

        for (int i = 0; i < difficulty; i++)
        {
            int monsterIndex = Random.Range(0, 5);

            string type = string.Empty;

            switch (monsterIndex)
            {
                case 0:
                    type = "Mob1";
                    break;
                case 1:
                    type = "Mob2";
                    break;
                case 2:
                    type = "Mob3";
                    break;
                case 3:
                    type = "Mob4";
                    break;
                case 4:
                    type = "Mob5";
                    break;
                case 5:
                    type = "Mob6";
                    break;
            }

            //Requests the monster from the pool
            Monster monster = Pool.GetObject(type).GetComponent<Monster>();

            monster.Spawn(health);

            //Adds the monster to the activemonster list
            activeMonsters.Add(monster);

            SoundManager.Instance.PlaySfx("spawn_sound");

            yield return new WaitForSeconds(2.5f);
        }

        LevelManager.Instance.GeneratePath();
    }

    /// <summary>
    /// Removes a monster from the game
    /// </summary>
    /// <param name="monster">Monster to remove</param>
    public void RemoveMonster(Monster monster)
    {
        activeMonsters.Remove(monster);

        if (!WaveActive && !gameOver)
        {
            waveBtn.SetActive(true);
        }
    }   
    
    public void GameOver()
    {
        if (!gameOver)
        {
            SoundManager.Instance.PlaySfx("death_sound");

            gameOver = true;
            gameOverMenu.SetActive(true);
            Time.timeScale = 0;
            upgradePanel.SetActive(false);
            statsPanel.SetActive(false);
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void SellTower()
    {
        if (selectedTower!=null)
        {
            Currency += selectedTower.Price / 2;

            selectedTower.GetComponentInParent<TileScript>().IsEmpty = true;

            Destroy(selectedTower.transform.parent.gameObject);

            DeselectTower();
        }
    }

    public void ShowStats()
    {
        statsPanel.SetActive(!statsPanel.activeSelf);
    }

    public void ShowSelectedTowerStats()
    {
        statsPanel.SetActive(!statsPanel.activeSelf);
        UpdateUpgradeTip();
    }

    public void SetTooltipText(string txt)
    {
        statTxt.text = txt;
    }

    public void UpdateUpgradeTip()
    {
        if (selectedTower!=null)
        {
            sellText.text= "+ "+(selectedTower.Price/2).ToString() + " <color=orange>Gold</color>";
            SetTooltipText(selectedTower.GetStats());

            if (selectedTower.NextUpgrade!=null)
            {
                upgradePrice.text = "- "+ selectedTower.NextUpgrade.Price.ToString() + " <color=orange>Gold</color>";
            }
            else
            {
                upgradePrice.text = string.Empty;
            }
        }
    }

    public void UpgradeTower()
    {
        if (selectedTower!=null)
        {
            if (selectedTower.Level<=selectedTower.Upgrades.Length && Currency>=selectedTower.NextUpgrade.Price)
            {
                SoundManager.Instance.PlaySfx("pop_sound");

                selectedTower.Upgrade();
            }
        }
    }

    public void ShowInGameMenu()
    {
        if (optionsMenu.activeSelf)
        {
            ShowMain();
        }
        else
        {
            inGameMenu.SetActive(!inGameMenu.activeSelf);
            if (!inGameMenu.activeSelf)
            {
                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale = 0;
            }
        }
    }

    private void DropTower()
    {
        ClickedBtn = null;
        Hover.Instance.Deactivate();
    }

    public void ShowOptions()
    {
        inGameMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void ShowMain()
    {
        inGameMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }
}

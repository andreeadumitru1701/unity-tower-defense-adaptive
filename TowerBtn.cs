using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerBtn : MonoBehaviour
{
    [SerializeField]
    private GameObject towerPrefab;

    [SerializeField]
    private Sprite sprite;

    [SerializeField]
    private int price;

    [SerializeField]
    private Text priceTxt;

    public GameObject TowerPrefab
    {
        get
        {
            return towerPrefab;
        }
    }

    public Sprite Sprite 
    {
        get
        {
            return sprite;
        }
    }

    public int Price 
    {
        get
        {
            return price;
        }
    }

    private void Start()
    {
        priceTxt.text = Price + " Gold";

        GameManager.Instance.Changed += new CurrencyChanged(PriceCheck);
    }

    private void PriceCheck()
    {
        if (price <=GameManager.Instance.Currency)
        {
            GetComponent<Image>().color = Color.white;
            priceTxt.color = Color.white;
        }
        else
        {
            GetComponent<Image>().color = Color.grey;
            priceTxt.color = Color.grey;
        }
    }

    public void ShowInfo(string type)
    {
        string tooltip = string.Empty;

        //Foloseste un prefabricat al unui turn pentru a obtine statisticile necesare tooltip-ului
        switch (type) 
        {
            case "Fire":
                FireTower fire = towerPrefab.GetComponentInChildren<FireTower>();
                tooltip = string.Format("<color=#ffa500ff><size=20><b>Fire</b></size></color>\nDamage: {0} \nProc: {1}%\nDebuff duration: {2} sec \nTick time: {3} sec \nTick damage: {4}\nCan apply a DOT to the target", fire.Damage, fire.Proc, fire.DebuffDuration, fire.TickTime, fire.TickDamage);
                break;

            case "Ice":
                IceTower ice = towerPrefab.GetComponentInChildren<IceTower>();
                tooltip = string.Format("<color=#61cafb><size=20><b>Ice</b></size></color>\nDamage: {0} \nProc: {1}%\nDebuff duration: {2} sec\nSlowing factor: {3}% \nHas a chance to slow down the target", ice.Damage, ice.Proc, ice.DebuffDuration, ice.SlowingFactor);
                break;

            case "Magic":
                MagicTower magic = towerPrefab.GetComponentInChildren<MagicTower>();
                tooltip = string.Format("<color=#fdd500><size=20><b>Magic</b></size></color>\nDamage: {0} \nProc: {1}%\nDebuff duration: {2} sec \nTick time: {3} sec \nSplash damage: {4}\nCan apply dripping poison", magic.Damage, magic.Proc, magic.DebuffDuration, magic.TickTime, magic.SplashDamage);
                break;

            case "Stone":
                StoneTower stone = towerPrefab.GetComponentInChildren<StoneTower>();
                tooltip = string.Format("<color=#c53544><size=20><b>Stone</b></size></color>\nDamage: {0} \nProc: {1}%\nDebuff duration: {2} sec \n Has a chance to stunn the target", stone.Damage, stone.Proc, stone.DebuffDuration);
                break;
        }

        GameManager.Instance.SetTooltipText(tooltip);
        GameManager.Instance.ShowStats();
    }
}

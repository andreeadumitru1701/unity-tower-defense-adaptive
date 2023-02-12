using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceTower : Tower
{
    [SerializeField]
    private float slowingFactor;

    public float SlowingFactor
    {
        get
        {
            return slowingFactor;
        }
    }

    private void Start()
    {
        base.Set();
        ElementType = Element.ICE;
        Upgrades = new TowerUpgrade[]
        {
            new TowerUpgrade(2,1,1,2,10),
            new TowerUpgrade(2,1,1,2,20),
        };
    }

    public override Debuff GetDebuff()
    {
        return new IceDebuff(SlowingFactor,DebuffDuration,Target);
    }

    public override string GetStats()
    {
        if (NextUpgrade != null)  //Daca urmatorul upgrade este disponibil
        {
            return string.Format("<color=#61cafb>{0}</color>{1} \nSlowing factor: {2}% <color=#00ff00ff>+{3}</color>", "<size=20><b>Ice</b></size>", base.GetStats(), SlowingFactor, NextUpgrade.SlowingFactor);
        }

        //Intoarcere la upgrade-ul curent
        return string.Format("<color=#61cafb>{0}</color>{1} \nSlowing factor: {2}%", "<size=20><b>Ice</b></size>", base.GetStats(), SlowingFactor);
    }

    public override void Upgrade()
    {
        this.slowingFactor += NextUpgrade.SlowingFactor;
        base.Upgrade();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneTower : Tower
{
    private void Start()
    {
        base.Set();
        ElementType = Element.STONE;
        Upgrades = new TowerUpgrade[]
        {
            new TowerUpgrade(2,2,1,2),
            new TowerUpgrade(5,3,1,2),
        };
    }

    public override Debuff GetDebuff()
    {
        return new StoneDebuff(Target,DebuffDuration);
    }

    public override string GetStats()
    {
        return string.Format("<color=#c53544>{0}</color>{1}", "<size=20><b>Stone</b></size>", base.GetStats());
    }
}

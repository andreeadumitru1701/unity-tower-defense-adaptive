using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicDebuff : Debuff
{
    private float tickTime;

    private float timeSinceTick;

    private MagicSplash splashPrefab;

    private int splashDamage;

    public MagicDebuff(int splashDamage, float tickTime, MagicSplash splashPrefab, float duration, Monster target) : base(target,duration)
    {
        this.splashDamage = splashDamage;
        this.tickTime = tickTime;
        this.splashPrefab = splashPrefab;
    }

    public override void Update()
    {
        if (target!=null)
        {
            timeSinceTick += Time.deltaTime;

            if (timeSinceTick>=tickTime)
            {
                timeSinceTick = 0;
                Splash();
            }
        }

        base.Update();
    }

    private void Splash()
    {
        MagicSplash tmp = GameObject.Instantiate(splashPrefab, target.transform.position, Quaternion.identity);

        tmp.Damage = splashDamage;

        Physics2D.IgnoreCollision(target.GetComponent<Collider2D>(), tmp.GetComponent<Collider2D>());
    }
}

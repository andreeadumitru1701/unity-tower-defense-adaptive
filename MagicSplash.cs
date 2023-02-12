using UnityEngine;

public class MagicSplash:MonoBehaviour
{
    public int Damage { get; set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag=="Monster")
        {
            other.GetComponent<Monster>().TakeDamage(Damage, Element.MAGIC);
            Destroy(gameObject);
        }
    }
}

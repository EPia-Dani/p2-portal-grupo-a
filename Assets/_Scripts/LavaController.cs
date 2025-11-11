using UnityEngine;

public class LavaController : MonoBehaviour
{
    public int damage = 150;
    private float damageTimer = 0f;
    HealthController health;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            health = collider.GetComponent<HealthController>();
            //SoundManager.Instance.PlayExclusiveSFX("player_die_lava");
            health.receiveDamage(damage);
        }
    }
}


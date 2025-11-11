using System;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;

    //Evento para comunicarse con el HUD
    public static event Action<int> OnHealthChanged;

    //Evento para comunicarse con DeathController
    public DeathController deathController;

    public void RecoverHealth(int amount)
    {
        if (currentHealth + amount > maxHealth)
            currentHealth = maxHealth;
        else
            currentHealth += amount;

        OnHealthChanged.Invoke(currentHealth);
    }

    public void receiveDamage(int amount)
    {
        currentHealth -= amount;

        SoundManager.Instance.PlaySFX("player_hurt");

        currentHealth = Mathf.Max(currentHealth, 0);

        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Player died!");
            SoundManager.Instance.PlaySFX("player_die");
            deathController.PlayerDead();
        }
    }


}

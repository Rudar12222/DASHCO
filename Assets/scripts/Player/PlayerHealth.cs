using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth; // Initialize health at start
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;

        // Clamp between 0 and maxHealth
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}

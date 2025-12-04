using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public TMP_Text healthtext;
    
    // ✅ NEW: Public property for enemies to check if the player is dead
    public bool IsDead { get; private set; } = false; 

    private void Start()
    {
        currentHealth = maxHealth; // Initialize health at start
        IsDead = false;
        UpdateHealthUI();
    }

    public void ChangeHealth(int amount)
    {
        if (IsDead) return; // Prevent healing/damage after death

        currentHealth += amount;
        
        // Clamp between 0 and maxHealth
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthtext != null)
        {
            healthtext.text ="HP: " + currentHealth + "/" + maxHealth;
        }
    }

    private void Die()
    {
        IsDead = true; // Set the flag so enemies stop chasing
        
        // Handle death visuals/logic
        // Example: Disable renderer, trigger death animation, etc.
        // For simplicity, we'll disable the whole object after a short delay
        
        Debug.Log(gameObject.name + " has died!");
        
        // Disable the component or GameObject after 2 seconds
        // IMPORTANT: We use SetActive(false) which causes the Player reference in EnemyController to become null, 
        // which helps stop the enemy even without the IsDead flag.
        Invoke(nameof(DisableGameObject), 2f); 
    }
    
    private void DisableGameObject()
    {
        gameObject.SetActive(false);
    }
}
using UnityEngine;
using TMPro;
public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public TMP_Text healthtext;

    private void Start()
    {
        currentHealth = maxHealth; // Initialize health at start
        healthtext.text ="HP:"+ currentHealth + "/" + maxHealth;
    
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
         healthtext.text ="HP:"+ currentHealth + "/" + maxHealth;
        // Clamp between 0 and maxHealth
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}

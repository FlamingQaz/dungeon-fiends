using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerCombat : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 100;

    [SerializeField]
    private float currentHealth;

    [SerializeField]
    private Slider healthBar;

    [SerializeField]
    private bool testTakeDamage = false;
    [SerializeField]
    private float testDamageToTake;

    [SerializeField]
    private bool isAlive;
    [SerializeField]
    private bool triggerResurrect;


    void Awake()
    {
        currentHealth = maxHealth;
        healthBar.value = currentHealth / maxHealth;
        
        isAlive = true;
        triggerResurrect = false;

        testTakeDamage = false;
    }


    void FixedUpdate()
    {
        if (isAlive)
        {
            if (testTakeDamage)
            {
                testTakeDamage = false;
                takeDamage(testDamageToTake);
            }

            if (currentHealth <= 0)
            {
                playerDeath();
            }
        }

        if (triggerResurrect)
        {
            triggerResurrect = false;
            resurrection();
        }
    }

    public void takeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.value = currentHealth / maxHealth;
    }

    private void playerDeath()
    {
        GetComponent<playerAnimation>().playDeathAnim();

        GetComponent<playerMovement>().enabled = false;
        GetComponent<playerAnimation>().enabled = false;
        isAlive = false;
    }

    private void resurrection()
    {
        GetComponent<playerMovement>().enabled = true;
        GetComponent<playerAnimation>().enabled = true;

        currentHealth = 1;
        healthBar.value = currentHealth / maxHealth;

        isAlive = true;

        GetComponent<playerAnimation>().playResurrectionAnim();
    }
}

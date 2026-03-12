using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHP = 5;
    public int currentHP;

    void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            currentHP = 0;
            Debug.Log(gameObject.name + " died");
        }

        Debug.Log(gameObject.name + " HP: " + currentHP);
    }

    public void ResetHP()
    {
        currentHP = maxHP;
    }

    public bool IsDead()
    {
        return currentHP <= 0;
    }
}
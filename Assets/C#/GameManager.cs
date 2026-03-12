using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Transform player;
    public Transform enemy;

    public static bool roundActive = true;

    Health playerHealth;
    Health enemyHealth;

    Vector3 playerStartPos;
    Vector3 enemyStartPos;

    bool resetting = false;

    int playerKills = 0;
    int enemyKills = 0;

    void Start()
    {
        playerHealth = player.GetComponent<Health>();
        enemyHealth = enemy.GetComponent<Health>();

        playerStartPos = player.position;
        enemyStartPos = enemy.position;

        roundActive = true;
    }

    void Update()
    {
        if (resetting) return;

        bool playerDead = playerHealth.IsDead();
        bool enemyDead = enemyHealth.IsDead();

        if (playerDead || enemyDead)
        {
            if (playerDead && enemyDead)
            {
                Debug.Log("Draw  /  Score  Player: " + playerKills + "  Enemy: " + enemyKills);
            }
            else if (enemyDead)
            {
                playerKills++;
                Debug.Log("Player Win  /  Score  Player: " + playerKills + "  Enemy: " + enemyKills);
            }
            else if (playerDead)
            {
                enemyKills++;
                Debug.Log("Enemy Win  /  Score  Player: " + playerKills + "  Enemy: " + enemyKills);
            }
            
            Debug.Log("Next Round...");
            
            StartCoroutine(ResetRound());
        }
    }

    IEnumerator ResetRound()
    {
        resetting = true;
        roundActive = false;

        yield return new WaitForSeconds(1.5f);

        DeleteAllBullets();

        player.position = playerStartPos;
        enemy.position = enemyStartPos;

        player.rotation = Quaternion.identity;
        enemy.rotation = Quaternion.identity;

        playerHealth.ResetHP();
        enemyHealth.ResetHP();

        yield return new WaitForSeconds(0.5f);

        roundActive = true;
        resetting = false;
    }

    void DeleteAllBullets()
    {
        Bullet[] bullets = FindObjectsOfType<Bullet>();

        foreach (Bullet bullet in bullets)
        {
            Destroy(bullet.gameObject);
        }
    }
}
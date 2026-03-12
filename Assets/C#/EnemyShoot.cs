using UnityEngine;

public class EnemyShoot : MonoBehaviour
{

    //現在未使用
    
    public Transform player;
    public Transform firePoint;
    public GameObject bulletPrefab;

    public float fireInterval = 1.5f;
    float timer;

    void Update()
    {
        if (!GameManager.roundActive) return;

        LookAtPlayer();

        timer += Time.deltaTime;

        if (timer >= fireInterval)
        {
            Shoot();
            timer = 0f;
        }
    }

    void LookAtPlayer()
    {
        Vector2 dir = player.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0,0,angle - 90f);
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Bullet>().ownerTag = "Enemy";
    }
}
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public Transform firePoint;
    public GameObject bulletPrefab;

    public float moveSpeed = 2f;
    public float stopDistance = 4f;
    public float strafeSpeed = 2f;

    public float fireInterval = 1.5f;

    Rigidbody2D rb;
    float timer;

    int strafeDir = 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

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

    void FixedUpdate()
    {
        if (!GameManager.roundActive) return;

        MoveToPlayer();
        Strafe();
    }

    void LookAtPlayer()
    {
        Vector2 dir = player.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    void MoveToPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
        }
    }

    void Strafe()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= stopDistance)
        {
            Vector2 side = transform.right * strafeDir;

            rb.MovePosition(rb.position + side * strafeSpeed * Time.fixedDeltaTime);

            if (Random.value < 0.01f)
            {
                strafeDir *= -1;
            }
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Bullet>().ownerTag = "Enemy";
    }
}
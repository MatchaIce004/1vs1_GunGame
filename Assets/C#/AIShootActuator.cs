using UnityEngine;

public class AIShootActuator
{
    private readonly Transform firePoint;
    private readonly GameObject bulletPrefab;
    private readonly float fireInterval;

    private float fireTimer;

    public AIShootActuator(Transform firePoint, GameObject bulletPrefab, float fireInterval = 1.5f)
    {
        this.firePoint = firePoint;
        this.bulletPrefab = bulletPrefab;
        this.fireInterval = fireInterval;
    }

    public void Tick(float deltaTime)
    {
        fireTimer += deltaTime;
    }

    public void Execute(AIShootAction action)
    {
        if (action != AIShootAction.Fire)
        {
            return;
        }

        TryShoot();
    }

    private void TryShoot()
    {
        if (fireTimer < fireInterval) return;
        if (firePoint == null || bulletPrefab == null) return;

        GameObject bulletObj = Object.Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletObj.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.ownerTag = "Enemy";
        }

        fireTimer = 0f;
    }
}
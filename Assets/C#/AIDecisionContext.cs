using System.Collections.Generic;
using UnityEngine;

public class AIDecisionContext
{
    public Transform EnemyTransform;
    public Transform PlayerTransform;

    public Rigidbody2D EnemyRb;
    public Rigidbody2D PlayerRb;

    public Health EnemyHealth;

    public List<Bullet> PlayerBullets;

    public float ArenaMinX;
    public float ArenaMaxX;
    public float ArenaMinY;
    public float ArenaMaxY;
}
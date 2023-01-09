using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    public List<Transform> wayPoints;
    Transform nextWayPoint;
    int currentWayPoint = 0;
    float reachedDistanceWayPoint = 0.1f;

    protected Hitable hitable;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        hitable = GetComponent<Hitable>();
    }

    void OnEnable()
    {
        nextWayPoint = wayPoints[currentWayPoint];
    }

    void FixedUpdate()
    {
        if (hitable.IsAlive)
        {
            if (CanMove)
                Flight();
            else
                rb.velocity = Vector3.zero;
        }
        else
        {
            // If flying dies : gravity 1 for falling after death
            rb.gravityScale = 2f;
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

    }

    // Goto next waypoint
    protected void Flight()
    {
        // Retrieve direction to next WP
        Vector2 direction = (nextWayPoint.position - transform.position).normalized;

        // Move towards next WP
        float distance = Vector2.Distance(nextWayPoint.position, transform.position);
        rb.velocity = direction * walkSpeed;

        // Check if we reached WP + switch to next
        if (distance <= reachedDistanceWayPoint)
        {
            currentWayPoint++;
            if (currentWayPoint >= wayPoints.Count)
                currentWayPoint = 0;

            nextWayPoint = wayPoints[currentWayPoint];
            FlipDirection();
        }

    }

    new void FlipDirection()
    {
        if (WalkDirection == WalkableDirection.Right && (transform.position.x >= nextWayPoint.position.x))
        {
            WalkDirection = WalkableDirection.Left;
        }
        else if (WalkDirection == WalkableDirection.Left && (transform.position.x < nextWayPoint.position.x))
        {
            WalkDirection = WalkableDirection.Right;
        }
    }


}

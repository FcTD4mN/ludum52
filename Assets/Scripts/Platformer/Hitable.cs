using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitable : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 100f;
    [SerializeField]
    private float health = 100f;

    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
            if (health <= 0)
            {
                IsAlive = false;
            }
        }
    }

    private bool isAlive = true;
    public bool IsAlive
    {
        get
        {
            return isAlive;
        }
        set
        {
            isAlive = value;
            animator.SetBool("isAlive", value);
            Debug.Log("Alive valute : " + IsAlive);
        }
    }

    [SerializeField]
    private bool isInvincible = false;
    public float invincibilityCooldown = 0.25f;
    float timeSinceHit = 0;

    Animator animator;

    // Start is called before the first frame update
    void OnEnable()
    {
        animator = GetComponent<Animator>();
    }

    float lastHit = 0f;

    void Update()
    {
        if (isInvincible)
        {
            // Remove invincibility after a time
            if (timeSinceHit > invincibilityCooldown)
            {
                isInvincible = false;
                timeSinceHit = 0;
            }

            timeSinceHit += Time.deltaTime;
        }
    }

    public void Hit(int damage)
    {
        // Debug.Log("We hit the object named : " + gameObject.name);
        if (IsAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;
            animator.SetTrigger("Hurt");
            // Debug.Log("health = " + Health);
        }
    }

    void RemoveFromScene()
    {
        Destroy(gameObject);
    }
}

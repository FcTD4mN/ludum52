using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLauncher : MonoBehaviour
{
    // Arrow :
    public float arrowDamage = 10f;
    public float arrowSpeed = 15f;
    private List<GameObject> firedArrows;
    public List<cResourceDescriptor.eResourceNames> availableArrowType;
    private int currentArrowType;

    // Bomb :
    public float bombDamage = 10f;
    public float bombSpeed = 5f;
    public float bombPushForce = 4f;

    // Bomb aim assist
    public BombTrajectory traj;
    bool isDragging = false;
    Vector2 startPoint;
    Vector2 endPoint;
    Vector2 direction;
    Vector2 force;
    float distance;

    // Prefabs :
    public GameObject arrowPrefab;
    public GameObject bombPrefab;
    public Transform launchPoint;
    public Vector3 launchOffset;

    public void OnEnable()
    {
        firedArrows = new List<GameObject>();
        currentArrowType = 0;
        availableArrowType = new List<cResourceDescriptor.eResourceNames>();
        availableArrowType.Add(cResourceDescriptor.eResourceNames.Arrows);
        availableArrowType.Add(cResourceDescriptor.eResourceNames.FireArrows);
    }

    void Update()
    {
        // Throw bomb aim assist
        // @TODO : Foutre ça dans un InputAction
        if ((int)GameManager.mResourceManager.GetRessource(cResourceDescriptor.eResourceNames.Bombs) > 0)
        {
            if (Input.GetMouseButtonDown(1))
            {
                isDragging = true;
                OnDragStart();
            }
            if (Input.GetMouseButtonUp(1))
            {
                isDragging = false;
                OnDragEnd();
            }

            if (isDragging)
                OnDrag();
        }
    }

    public void SwitchArrowType()
    {
        currentArrowType++;
        if (currentArrowType >= availableArrowType.Count)
            currentArrowType = 0;
    }

    public cResourceDescriptor.eResourceNames CurrentArrowType()
    {
        return availableArrowType[currentArrowType];
    }

    public void ClearFiredArrows()
    {
        foreach (GameObject arrow in firedArrows)
        {
            GameObject.Destroy(arrow);
        }

        firedArrows.Clear();
    }

    public void FireArrow()
    {
        // Instantiate arrow
        GameObject arrowPrefab = null;
        if (availableArrowType[currentArrowType] == cResourceDescriptor.eResourceNames.Arrows)
            arrowPrefab = Resources.Load<GameObject>("Prefabs/Platformer/Character/Arrow");
        else
            arrowPrefab = Resources.Load<GameObject>("Prefabs/Platformer/Character/FireArrow");

        GameObject arrow = Instantiate(arrowPrefab, transform.position, arrowPrefab.transform.rotation);

        // Retrieve direction to next WP
        // Can be adjusted for AIM ASSIT
        Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionRaw = (target - transform.position);
        Vector2 direction = directionRaw.normalized;

        // Move towards next WP
        float distance = Vector2.Distance(target, transform.position);
        Rigidbody2D rbArrow = arrow.GetComponent<Rigidbody2D>();
        rbArrow.velocity = direction * arrowSpeed;

        GameManager.mResourceManager.AddResource(availableArrowType[currentArrowType].ToString(), -1, false);
    }

    public void LaunchBomb(bool facingRight)
    {
        // Instantiate bomb and push it forward
        GameObject bomb = Instantiate(bombPrefab, launchPoint.position, transform.rotation);
        Vector3 direction;
        if (facingRight)
            direction = transform.right + Vector3.up;
        else
            direction = -transform.right + Vector3.up;

        Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();
        rb.AddForce(direction * bombSpeed, ForceMode2D.Impulse);
        bomb.transform.Translate(launchOffset);

        GameManager.mResourceManager.AddResource(cResourceDescriptor.eResourceNames.Bombs.ToString(), -1, false);
    }

    public void AimLaunchBomb(bool facingRight, Vector2 force)
    {
        // Instantiate bomb and push it forward
        GameObject bomb = Instantiate(bombPrefab, launchPoint.position, transform.rotation);
        Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = false;
        rb.AddForce(force, ForceMode2D.Impulse);

        GameManager.mResourceManager.AddResource(cResourceDescriptor.eResourceNames.Bombs.ToString(), -1, false);
    }

    // Handle Aim draw for bomb :
    void OnDragStart()
    {
        if ((int)GameManager.mResourceManager.GetRessource(cResourceDescriptor.eResourceNames.Bombs) <= 0)
        {
            return;
        }

        startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        traj.Show();
    }

    void OnDrag()
    {
        if ((int)GameManager.mResourceManager.GetRessource(cResourceDescriptor.eResourceNames.Bombs) <= 0)
        {
            return;
        }

        endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        distance = Vector2.Distance(startPoint, endPoint);
        direction = (startPoint - endPoint).normalized;
        force = direction * distance * bombPushForce;

        traj.UpdateDots(transform.position, force);
    }

    void OnDragEnd()
    {
        if ((int)GameManager.mResourceManager.GetRessource(cResourceDescriptor.eResourceNames.Bombs) <= 0)
        {
            return;
        }

        // Throw the bomb
        AimLaunchBomb(GameManager.mInstance.playerCtrler.IsFacingRight ? true : false, force);
        traj.Hide();
    }

}

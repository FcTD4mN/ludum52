using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLauncher : MonoBehaviour
{
    // Arrow :
    public float arrowDamage = 10f;
    public float arrowSpeed = 15f;
    private List<GameObject> firedArrows;

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

    [HideInInspector]
    public Vector3 targetPos;

    public void OnEnable()
    {
        firedArrows = new List<GameObject>();
    }

    void Update()
    {
        // Throw bomb aim assist
        // @TODO : Foutre ça dans un InputAction
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
        // Converting rotation in degree
        // Vector3 rotation = targetPosWorld - launchPoint.position;
        // float rotZ;
        // if (GameManager.mInstance.playerCtrler.IsFacingRight)
        //     rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        // else
        //     rotZ = Mathf.Atan2(-rotation.y, -rotation.x) * Mathf.Rad2Deg;

        // Quaternion arrowRotation = Quaternion.Euler(0, 0, rotZ);

        // Instantiate arrow
        GameObject arrow = Instantiate(arrowPrefab, launchPoint.position, arrowPrefab.transform.rotation);

        // Sending toward direction
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        Vector3 direction = targetPos - launchPoint.position;
        Vector2 directionNorm = direction.normalized;
        rb.velocity = new Vector2(directionNorm.x * arrowSpeed, directionNorm.y * arrowSpeed);

        // Face arrow in the right direction
        Vector3 originScale = arrow.transform.localScale;
        arrow.transform.localScale = new Vector3(
            originScale.x * transform.localScale.x > 0 ? 1 : -1,
            originScale.y,
            originScale.z
        );

        firedArrows.Add(arrow);
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
    }

    // Handle Aim draw for bomb :
    void OnDragStart()
    {
        startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        traj.Show();
    }

    void OnDrag()
    {
        endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        distance = Vector2.Distance(startPoint, endPoint);
        direction = (startPoint - endPoint).normalized;
        force = direction * distance * bombPushForce;

        traj.UpdateDots(transform.position, force);
    }

    void OnDragEnd()
    {
        // Throw the bomb
        AimLaunchBomb(GameManager.mInstance.playerCtrler.IsFacingRight ? true : false, force);
        traj.Hide();
    }

}

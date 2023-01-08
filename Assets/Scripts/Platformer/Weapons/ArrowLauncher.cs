using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowLauncher : MonoBehaviour
{
    // Arrow stats :
    public float arrowDamage = 10f;
    public float arrowSpeed = 15f;
    private List<GameObject> firedArrows;

    // Bomb stats :
    public float bombDamage = 10f;
    public float bombSpeed = 5f;

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

}

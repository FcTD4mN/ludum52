using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowLauncher : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform launchPoint;

    public void FireArrow()
    {
        // Instantiate arrow
        GameObject arrow = Instantiate(arrowPrefab, launchPoint.position, arrowPrefab.transform.rotation);

        // Face arrow in the right direction
        Vector3 originScale = arrow.transform.localScale;
        arrow.transform.localScale = new Vector3(
            originScale.x * transform.localScale.x > 0 ? 1 : -1,
            originScale.y,
            originScale.z
        );
    }

    private void DeleteArrows()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float destroyAfterTime = 2f;
    private float startingTime;

    bool m_Started;
    public LayerMask m_LayerMask;

    Animator animator;

    void Start()
    {
        //Use this to ensure that the Gizmos are being drawn when in Play Mode.
        m_Started = true;
    }

    void OnEnable()
    {
        animator = GetComponent<Animator>();
        startingTime = Time.time;
    }

    void FixedUpdate()
    {
        // Destroy arrow after certain distance if not hitting anything
        if (Time.time - startingTime > destroyAfterTime && m_Started)
        {
            MyCollisions();
            m_Started = false;
            animator.SetTrigger("Explode");
        }
    }

    void DestroyItself()
    {
        Destroy(gameObject);
    }

    void MyCollisions()
    {
        //Use the OverlapBox to detect if there are any other colliders within this box area.
        //Use the GameObject's centre, half the size (as a radius) and rotation. This creates an invisible box around your GameObject.
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(gameObject.transform.position, transform.localScale * 2, 0f, m_LayerMask);
        int i = 0;
        //Check when there is a new collider coming into contact with the box
        while (i < hitColliders.Length)
        {
            Debug.Log("Hit : " + hitColliders[i].name + i);
            // @TODO : Read les stats
            hitColliders[i].GetComponent<Hitable>().Hit(30);
            i++;
        }
    }

    //Draw the Box Overlap as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        // Gizmos.color = Color.red;
        // //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        // if (m_Started)
        //     //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
        //     Gizmos.DrawWireCube(transform.position, transform.localScale * 2);
    }

}
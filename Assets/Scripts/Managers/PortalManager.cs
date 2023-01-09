using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public List<InteractablePortal> portals;
    private List<InteractablePortal> availablePortals;

    public void Initialize()
    {
        availablePortals = new List<InteractablePortal>();
    }

    public void UnlockPortal(InteractablePortal p)
    {
        availablePortals.Add(p);
    }

    public void DisplayAvailablePortals()
    {
        foreach (InteractablePortal portal in portals)
        {
            bool unlocked = availablePortals.Contains(portal);
            // Debug.Log("Portal : " + portal.name + " / unlocked : " + unlocked);
        }
    }

    public void TravelToPortal(InteractablePortal p)
    {
        if (availablePortals.Contains(p))
        {
            // Set animation for leaving
            p.animator.SetTrigger("Open");

            // Travel Player to portal
            GameObject player = GameObject.Find("Player").gameObject;
            player.transform.position = p.transform.position;
        }
    }
}
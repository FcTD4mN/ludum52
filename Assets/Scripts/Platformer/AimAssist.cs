using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimAssist : MonoBehaviour
{

    private Camera mainCam;
    private Vector3 mousePos;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // Vector3 mos_pos = Mouse.current.position.ReadValue();
        // mousePos = mainCam.ScreenToWorldPoint(mos_pos);

        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = mousePos - transform.position;
        float rotZ;
        if (GameManager.mInstance.playerCtrler.IsFacingRight)
            rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        else
            rotZ = Mathf.Atan2(-rotation.y, -rotation.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }
}

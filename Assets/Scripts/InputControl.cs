using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControl : MonoBehaviour
{
    private Camera cam;
    private Vector3 mousePos;
    private float maxDis = 10f;
    private BlockControl block;

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        block = GetComponent<BlockControl>();
    }

    private void Update()
    {
        MouseInput();
        ObjCheck();
    }

    private int click;
    private void MouseInput()
    {
        if (Input.GetMouseButtonDown(0)) click = 1;
        else if (Input.GetMouseButton(0)) click = 2;
        else if (Input.GetMouseButtonUp(0))
        {
            click = 3;
            block.ClickOver();
        }
        else click = 0;
    }

    private void ObjCheck()
    {
        if (click >= 1 && click < 3)
        {
            mousePos = Input.mousePosition;
            mousePos = cam.ScreenToWorldPoint(mousePos);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, transform.forward, maxDis);
            if (hit)
            {
                if (hit.transform.CompareTag("ClickableObject"))
                {
                    hit.transform.GetComponent<ClickableObject>().Click(click);
                }
            }
        }
    }
}

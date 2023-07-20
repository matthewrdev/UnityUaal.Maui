using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DetectTouchPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.touchCount <= 0)
        {
            return;
        }

        TouchPhase touchPhase = Input.GetTouch(0).phase;
        if (touchPhase != TouchPhase.Began)
        {
            return;
        }
        
        Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        RaycastHit raycastHit;
        if (Physics.Raycast(raycast, out raycastHit))
        {
            Bridge[] components = GameObject.FindObjectsByType<Bridge>(FindObjectsSortMode.None);
            Bridge bridge = components.Length > 0 ? components[0] : null;
            if (bridge != null)
            {
                bridge.SendContent("Touch", raycastHit.collider.name + "=" + raycastHit.point.x + "," + raycastHit.point.y + "," + raycastHit.point.z);
            }
        }
    }
}

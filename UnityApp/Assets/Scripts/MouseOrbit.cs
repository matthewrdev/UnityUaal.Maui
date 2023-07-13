using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("Camera-Control/Mouse Orbit")]
public class MouseOrbit : MonoBehaviour
{
    public Transform target;


    private Quaternion rotationY;
    private Vector2 fingerDown;
    private Vector2 fingerUp;
    private bool detectSwipeOnlyAfterRelease = false;
    private Vector3 rotation;
    private float rotationSpeed = 0.12f;
    private float SWIPE_THRESHOLD = 1f;
    void CheckSwipe()
    {
        //Check if Vertical swipe
        if (VerticalMove() > SWIPE_THRESHOLD && VerticalMove() > HorizontalValMove())
        {
            //Debug.Log("Vertical");
            if (fingerDown.y - fingerUp.y > 0)//up swipe
            {
                // OnSwipeUp();
            }
            else if (fingerDown.y - fingerUp.y < 0)//Down swipe
            {
                // OnSwipeDown();
            }
            fingerUp = fingerDown;
        }

        //Check if Horizontal swipe
        else if (HorizontalValMove() > SWIPE_THRESHOLD && HorizontalValMove() > VerticalMove())
        {
            //Debug.Log("Horizontal");
            if (fingerDown.x - fingerUp.x > 0)//Right swipe
            {
                OnSwipeRight();
            }
            else if (fingerDown.x - fingerUp.x < 0)//Left swipe
            {
                OnSwipeLeft();
            }
            fingerUp = fingerDown;
        }

        //No Movement at-all
        else
        {
            //Debug.Log("No Swipe!");
        }
    }
    private void Update()
    {
        TouchSwipe();
    }
    void TouchSwipe()
    {
        if (Input.touchCount == 1)
        {
            // if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            //     return;


            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    fingerUp = touch.position;
                    fingerDown = touch.position;
                }

                //Detects Swipe while finger is still moving
                if (touch.phase == TouchPhase.Moved)
                {
                    if (!detectSwipeOnlyAfterRelease)
                    {
                        fingerDown = touch.position;
                        CheckSwipe();
                    }
                }


            }



        }
    }

    float VerticalMove()
    {
        return Mathf.Abs(fingerDown.y - fingerUp.y);
    }

    float HorizontalValMove()
    {
        return Mathf.Abs(fingerDown.x - fingerUp.x);
    }

    void OnSwipeUp()
    {
        float verticalMove = this.VerticalMove();

        rotation.x = verticalMove * rotationSpeed;
        rotation.y = 0;
        rotation.z = 0;
        target.Rotate(rotation, Space.World);

    }

    void OnSwipeDown()
    {

        float verticalMove = this.VerticalMove();
        rotation.x = -verticalMove * rotationSpeed;
        rotation.y = 0;
        rotation.z = 0;
        target.Rotate(rotation, Space.World);
    }

    void OnSwipeLeft()
    {
        float horizontalmove = HorizontalValMove();

        rotation.x = 0;
        rotation.y = horizontalmove * rotationSpeed;
        rotation.z = 0;
        // transform.GetChild(0).Rotate(rotation);
        target.Rotate(rotation, Space.Self);

    }

    void OnSwipeRight()
    {
        float horizontalmove = HorizontalValMove();
        rotation.x = 0;
        rotation.y = -horizontalmove * rotationSpeed;
        rotation.z = 0;
        //transform.GetChild(0).Rotate(rotation);
        target.Rotate(rotation, Space.Self);
    }
}
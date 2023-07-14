using UnityEngine;

public class CamZoom : MonoBehaviour
{
    public float zoomSpeed = 250f;
    public float zoomMin = 30f;
    public float zoomMax = 80f;

    Camera cam;
    float touchZoomSpeed = 0.1f;
    float mouseZoomSpeed = 15.0f;

    float horizontalSpeed = 12f;

    void Start()
    {
        cam = GetComponent<Camera>();
    }
    private void LateUpdate()
    {
        //      if (Input.GetMouseButton(0))
        //      {
        //	float amtToMove = Input.GetAxis("Mouse X") * horizontalSpeed * Time.deltaTime;
        //	transform.Translate(Vector3.left * amtToMove, Space.World);
        //}

        //if (Input.touchCount == 2)
        //{
        //	// get current touch positions
        //	Touch tZero = Input.GetTouch(0);
        //	Touch tOne = Input.GetTouch(1);
        //	// get touch position from the previous frame
        //	Vector2 tZeroPrevious = tZero.position - tZero.deltaPosition;
        //	Vector2 tOnePrevious = tOne.position - tOne.deltaPosition;

        //	float oldTouchDistance = Vector2.Distance(tZeroPrevious, tOnePrevious);
        //	float currentTouchDistance = Vector2.Distance(tZero.position, tOne.position);

        //	// get offset value
        //	float deltaDistance = oldTouchDistance - currentTouchDistance;
        //	Zoom(deltaDistance, touchZoomSpeed);
        //}
        //else
        //{
        //	Zoom(Input.GetAxis("Mouse ScrollWheel"), mouseZoomSpeed);

        //	/*GetComponent<Camera>().fieldOfView -= Input.GetAxis("Mouse Y") * zoomSpeed * 0.01f;
        //	GetComponent<Camera>().fieldOfView = Mathf.Clamp(GetComponent<Camera>().fieldOfView, zoomMin, zoomMax);*/
        //}

        float pinchAmount = 0;
        Vector3 panAmount = Vector3.zero;
        DetectTouchMovement.Calculate();
        if (Mathf.Abs(DetectTouchMovement.pinchDistanceDelta) > 0)
        { // zoom
            pinchAmount = DetectTouchMovement.pinchDistanceDelta;

        }

        if (DetectTouchMovement.panVector != Vector2.zero)
        {
            Vector2 newPanVector = DetectTouchMovement.panVector;
            panAmount = -transform.up * newPanVector.y + -transform.right * newPanVector.x;


        }


        transform.position += panAmount * 10;
        Zoom(pinchAmount, touchZoomSpeed);
    }

    void Zoom(float deltaMagnitudeDiff, float speed)
    {

        cam.fieldOfView -= deltaMagnitudeDiff * speed;
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, zoomMin, zoomMax);
    }
}
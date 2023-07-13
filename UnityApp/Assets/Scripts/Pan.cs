using UnityEngine;

public class Pan : MonoBehaviour
{
    public float panSpeed = 2.5f;

    public bool limitPan = true;

    public float yMax = 4.9f;
    public float yMin = -5.4f;

    public float xMax = 2.7f;
    public float xMin = -3.0f;

    private void Update()
    {
        if (limitPan)
        {
            var y = Mathf.Clamp(transform.position.y, yMin, yMax);
            var x = Mathf.Clamp(transform.position.x, xMin, xMax);
            transform.position = new Vector3(x, y, 0f);
        }

        if (Input.GetKey("w"))
        {
            transform.Translate(Vector3.up * Time.deltaTime * panSpeed);
        }
        else if (Input.GetKey("s"))
        {
            transform.Translate(-Vector3.up * Time.deltaTime * panSpeed);
        }

        if (Input.GetKey("d"))
        {
            transform.Translate(Vector3.right * Time.deltaTime * panSpeed);
        }
        else if (Input.GetKey("a"))
        {
            transform.Translate(-Vector3.right * Time.deltaTime * panSpeed);
        }
    }
}
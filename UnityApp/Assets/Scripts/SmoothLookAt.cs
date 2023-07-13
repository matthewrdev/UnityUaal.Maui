using UnityEngine;

[AddComponentMenu("Camera-Control/Smooth Look At")]
public class SmoothLookAt : MonoBehaviour
{
    public Transform target;
    public float damping = 6.0f;
    public bool smooth = true;

    private void LateUpdate()
    {
        if (target)
        {
            if (smooth)
            {
                // Look at and dampen the rotation
                var rotation = Quaternion.LookRotation(target.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
            }
            else
            {
                // Just lookat
                transform.LookAt(target);
            }
        }
    }

    private void Start()
    {
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    }
}
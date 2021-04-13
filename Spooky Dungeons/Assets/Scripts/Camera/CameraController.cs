using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    private Transform target;
    public float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 goalPos = target.position;
        goalPos.y = transform.position.y;
        transform.position = Vector3.SmoothDamp(transform.position, goalPos, ref velocity, smoothTime);
    }
}
using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {
    Vector3 displacement;
    public Transform target;
    Vector3 oldPosition;
    Camera camera;
    float originalSize;
    public float speedScaling = 2.0f;


	// Use this for initialization
	void Start () {
        camera = GetComponent<Camera>();
        originalSize = camera.orthographicSize;
        //target = GameObject.FindGameObjectWithTag("Player").transform;
        displacement = transform.position - target.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.position = SuperSmoothLerp(transform.position, oldPosition, target.position + displacement, Time.fixedDeltaTime, 10);
        oldPosition = target.position + displacement;
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, originalSize + Vector3.Distance(transform.position, oldPosition) * speedScaling, Time.fixedDeltaTime * 2);
	}

    Vector3 SuperSmoothLerp(Vector3 x0, Vector3 y0, Vector3 yt, float t, float k)
    {
        Vector3 f = x0 - y0 + (yt - y0) / (k * t);
        return yt - (yt - y0) / (k * t) + f * Mathf.Exp(-k * t);
    }
}

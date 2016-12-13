using UnityEngine;
using System.Collections;

public class ExplosionCameraShake : MonoBehaviour {
    void Start()
    {
        Camera[] cameras = FindObjectsOfType<Camera>();
        for(int i = 0; i < cameras.Length; i++)
        {
            Vector3 viewPoint = cameras[i].WorldToViewportPoint(transform.position);
            Debug.Log(viewPoint);
            if (viewPoint.z > 0 && viewPoint.x > 0 && viewPoint.x < 1 && viewPoint.y > 0 && viewPoint.y < 1)
            {
                Debug.Log("Shaking tha camera");
                cameras[i].GetComponent<CameraShake>().ShakeCamera(5, 0.2f);
            }
        }
    }
}

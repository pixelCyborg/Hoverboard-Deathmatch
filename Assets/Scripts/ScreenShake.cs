using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour {
    Texture2D noise;
    private bool shaking;
    private Camera camera;
    private int width;
    private int height;

    void Start()
    {
        width = noise.width;
        height = noise.height;
    }

    public void Shake(float time)
    {
        StartCoroutine(_Shake(time));
    }

    IEnumerator _Shake(float time)
    {
        while(shaking)
        {
            yield return new WaitForEndOfFrame();
            Vector2 coord = new Vector2(Random.Range(0, width), Random.Range(0, height));
            Color perlinSample = noise.GetPixel((int)coord.x, (int)coord.y);
            float sampleMagnitude = perlinSample.grayscale;
            camera.transform.localPosition += (Vector3)(coord.normalized * sampleMagnitude);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

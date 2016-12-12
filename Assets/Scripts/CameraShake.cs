///Daniel Moore (Firedan1176) - Firedan1176.webs.com/
///26 Dec 2015
///
///Shakes camera parent object

using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{

    public bool debugMode = false;//Test-run/Call ShakeCamera() on start

    public float shakeAmount;//The amount to shake this frame.
    public float shakeDuration;//The duration this frame.

    //Readonly values...
    float shakePercentage;//A percentage (0-1) representing the amount of shake to be applied when setting rotation.
    float startAmount;//The initial shake amount (to determine percentage), set when ShakeCamera is called.
    float startDuration;//The initial shake duration, set when ShakeCamera is called.

    bool isRunning = false; //Is the coroutine running right now?

    public bool smooth;//Smooth rotation?
    public float smoothAmount = 5f;//Amount to smooth

    public float testAmount = 5.0f;
    public float testTime = 0.5f;
    Vector3 oldShake = Vector3.zero;
    ThirdPersonOrbitCam orbit;

    void Start()
    {
        orbit = GetComponent<ThirdPersonOrbitCam>();
        if (debugMode) ShakeCamera();
    }

#if UNITY_EDITOR
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ShakeCamera(testAmount, testTime);
        }
    }
#endif

    void ShakeCamera()
    {

        startAmount = shakeAmount;//Set default (start) values
        startDuration = shakeDuration;//Set default (start) values

        if (!isRunning) StartCoroutine(Shake());//Only call the coroutine if it isn't currently running. Otherwise, just set the variables.
    }

    public void ShakeCamera(float amount, float duration)
    {

        shakeAmount += amount;//Add to the current amount.
        startAmount = shakeAmount;//Reset the start amount, to determine percentage.
        shakeDuration += duration;//Add to the current time.
        startDuration = shakeDuration;//Reset the start time.

        if (!isRunning) StartCoroutine(Shake());//Only call the coroutine if it isn't currently running. Otherwise, just set the variables.
    }


    IEnumerator Shake()
    {
        isRunning = true;

        while (shakeDuration > 0.01f)
        {
            Vector3 shakeOffset = Random.insideUnitSphere * shakeAmount;//A Vector3 to add to the Local Rotation
            shakeOffset.z = 0;//Don't change the Z; it looks funny.

            shakePercentage = shakeDuration / startDuration;//Used to set the amount of shake (% * startAmount).

            shakeAmount = startAmount * shakePercentage;//Set the amount of shake (% * startAmount).
            shakeDuration = Mathf.Lerp(shakeDuration, 0, Time.deltaTime);//Lerp the time, so it is less and tapers off towards the end.


            if (smooth)
            {
                orbit.shakeOffset = Vector3.Lerp(oldShake, shakeOffset, Time.deltaTime * smoothAmount);//Quaternion.Euler(rotationAmount - transform.localRotation.eulerAngles);//Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rotationAmount), Time.deltaTime * smoothAmount);
                oldShake = shakeOffset;                      //transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rotationAmount), Time.deltaTime * smoothAmount);
            }
            else
                orbit.shakeOffset = shakeOffset;//Quaternion.Euler(rotationAmount);//Set the local rotation the be the rotation amount.
                //transform.localRotation = Quaternion.Euler(rotationAmount);//Set the local rotation the be the rotation amount.

            yield return null;
        }
        transform.localRotation = Quaternion.identity;//Set the local rotation to 0 when done, just to get rid of any fudging stuff.
        isRunning = false;
    }
}
 
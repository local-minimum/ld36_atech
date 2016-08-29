using UnityEngine;
using System.Collections.Generic;

public class CameraZoomMove : MonoBehaviour {

    public int animationSteps = 20;
    public float animationSpeed = 0.1f;
    public float animationInitialDelay = 1f;

    public float camSizeStart;
    public float camSizeEnd;
    public Vector3 camPosStart;
    public Vector3 camPosEnd;

    public AnimationCurve posEasing;
    public AnimationCurve sizeEasing;

    void Start()
    {

        StartCoroutine(AnimateCamera(SingleCam.Cam));
    }


    IEnumerator<WaitForSeconds> AnimateCamera(Camera cam)
    {
        cam.transform.position = Vector3.LerpUnclamped(camPosStart, camPosEnd, posEasing.Evaluate(0f));
        cam.orthographicSize = Mathf.LerpUnclamped(camSizeStart, camSizeEnd, sizeEasing.Evaluate(0f));

        yield return new WaitForSeconds(animationInitialDelay);
        float stepL = 1 / (float)animationSteps;
        for (int i = 0; i<animationSteps; i++)
        {
            cam.transform.position = Vector3.LerpUnclamped(camPosStart, camPosEnd, posEasing.Evaluate(stepL * i));
            cam.orthographicSize = Mathf.LerpUnclamped(camSizeStart, camSizeEnd, sizeEasing.Evaluate(stepL * i));
            yield return new WaitForSeconds(animationSpeed);
        }

        cam.transform.position = Vector3.LerpUnclamped(camPosStart, camPosEnd, posEasing.Evaluate(1f));
        cam.orthographicSize = Mathf.LerpUnclamped(camSizeStart, camSizeEnd, sizeEasing.Evaluate(1f));


    }

}

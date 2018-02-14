using UnityEngine;
using System.Collections;
using System;

public class CameraScript : MonoBehaviour 
{
    private float minCameraSize = 7.5f;
    private float maxCameraSize = 8.5f;

    private float minAspectRatio = 1.5f;
    private float maxAspectRatio = 1.8f;

	private void Start () 
    {
        var aspectRatio = Screen.height / (float)Screen.width;
        if (aspectRatio < minAspectRatio) {
            aspectRatio = minAspectRatio;
        }
        if (aspectRatio > maxAspectRatio) {
            aspectRatio = maxAspectRatio;
        }

        var aspectThreshold = (aspectRatio - minAspectRatio) / (maxAspectRatio - minAspectRatio);
        var cameraSize = (aspectThreshold * (maxCameraSize - minCameraSize)) + minCameraSize;
        var camera = GetComponent<Camera>();

        camera.orthographicSize = (float)Math.Round(cameraSize, 2);
	}
}

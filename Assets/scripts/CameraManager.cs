using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

    // Use this for initialization
    void Start () {
        float targetAspect = 9 / 16;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;
        Camera camera = GetComponent<Camera>();

        if (scaleHeight < 1.0f)
        {
            camera.orthographicSize = camera.orthographicSize / scaleHeight;
        }
        //float targetaspect = 16/9;
        //float deviceaspect = Screen.width / Screen.height;
        //float scale = targetaspect / deviceaspect;
        //Camera camera = GetComponent<Camera>();

        //if (scale < 1)
        //{
        //    Rect rect = camera.rect;
        //    rect.width = 1.0f;
        //    rect.height = scale;
        //    rect.y = 0;
        //    rect.x = (1.0f - scale) / 2.0f;
        //    camera.rect = rect;
        //}
        //else
        //{
        //    float scalewidth = 1.0f / scale;
        //    Rect rect = camera.rect;
        //    rect.width = scalewidth;
        //    rect.height = 1.0f;
        //    rect.x = (1.0f - scalewidth) / 2.0f;
        //    rect.y = 0;
        //    camera.rect = rect;
        //}
    }
}

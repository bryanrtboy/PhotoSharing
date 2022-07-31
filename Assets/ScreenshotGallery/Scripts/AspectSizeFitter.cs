//Bryan Leister, July 2022
//Script to fix the sizing of a rawimage on a UI panel that is set to expand in all directions.
//Put this on the RawImage

using UnityEngine;
using UnityEngine.UI;

public class AspectSizeFitter : MonoBehaviour
{
    public CanvasScaler m_canvas;
    private RawImage _raw;
    private ScreenOrientation _lastOrientation;
    private void OnEnable()
    {
        if (_raw == null)
            _raw = GetComponent<RawImage>();

        if (_raw)
        {
            _raw.enabled = true;
            SetAspectRatio(_raw);
        }

        _lastOrientation = Screen.orientation;

        InvokeRepeating(nameof(DetectOrientationChange),1,.2f);
    }

    private void OnDisable()
    {
        if(_raw)
            _raw.rectTransform.localScale = Vector3.one;
        CancelInvoke();
    }

    void DetectOrientationChange()
    {
        if (Screen.orientation == _lastOrientation)
            return;

        _raw.enabled = false;
        _raw.rectTransform.localScale = Vector3.one;
        Debug.Log(Screen.orientation.ToString() + ", last orientation was " + _lastOrientation.ToString());
        _lastOrientation = Screen.orientation;
        Invoke(nameof(EnableImage),.1f);
    }

    void EnableImage()
    {
        SetAspectRatio(_raw);
        _raw.enabled = true;
    }

    private void SetAspectRatio(RawImage raw)
    {
        Texture myTexture = raw.texture;

        if (Screen.orientation == ScreenOrientation.LandscapeLeft ||
            Screen.orientation == ScreenOrientation.LandscapeRight)
        {


            if (myTexture.height > myTexture.width)
            {
                float scaleFactor =(float)Screen.height/ Screen.width;
                raw.rectTransform.localScale = new Vector3((myTexture.width / (float)myTexture.height) * scaleFactor, 1, 1);
                //Debug.Log(("Landscape orientation with a vertical image. Scale is set to " + raw.rectTransform.localScale) + ". Scale factor is " + scaleFactor);
            }

        } else
        {
            if (myTexture.width > myTexture.height)
            {
                float scaleFactor =(float)Screen.width/ Screen.height;
                raw.rectTransform.localScale = new Vector3(1, (myTexture.height / (float)myTexture.width) * scaleFactor, 1);
                //Debug.Log(("Portrait orientation with a horizontal image. Scale is set to " + raw.rectTransform.localScale) + ". Scale factor is " + scaleFactor);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;


public class TakeScreenShot : MonoBehaviour
{
    public GameObject[] m_thingsToHide;

    // Update is called once per frame
    void Update()
    {
        // if (Keyboard.current[Key.Space].wasPressedThisFrame )
        // {
        //     foreach (GameObject g in m_thingsToHide)
        //         g.SetActive(false);
        //
        //     StartCoroutine(TakeTheShot());
        //     Debug.Log("Taking a screen shot");
        // }
    }

    public void TakeAScreenShot(GameObject thingToDeActivateAfterShot)
    {
        foreach (GameObject g in m_thingsToHide)
            g.SetActive(false);

        StartCoroutine(TakeTheShot(thingToDeActivateAfterShot));
    }
    IEnumerator TakeTheShot(GameObject thingToDeActivateAfterShot)
    {
        yield return new WaitForEndOfFrame();

        //ScreenCapture.CaptureScreenshot( System.DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss") + GetMainGameViewSize().ToString() + ".png");

        foreach (GameObject g in m_thingsToHide)
            g.SetActive(true);

        yield return new WaitForEndOfFrame();
        thingToDeActivateAfterShot.SetActive(false);
    }

    public static Vector2 GetMainGameViewSize()
    {
        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        System.Object Res = GetSizeOfMainGameView.Invoke(null,null);
        return (Vector2)Res;
    }

    public static string GetMainGameViewName()
    {
        string name = "";
        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        System.Object Res = GetSizeOfMainGameView.Invoke(null,null);
        return name;
    }
}

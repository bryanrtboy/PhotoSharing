using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using UnityEditor;
using System.IO;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class SaveScreen : MonoBehaviour
{

    // public GameObject m_buttonPrefab;
    // public string m_path = "/Screenshots/";
    //public int m_buttonCount = 10;
    public GameObject[] m_thingsToHide;
    public GameObject[] m_galleryUIToShow;
    public Text m_debug;

    bool m_isLocked = false;
    int m_count = 0;
    //    Texture2D texture = null;
    public RawImage[] m_buttons;

    void Start()
    {
        //Gallery as it's own level fix (DAM FIX)
        for (int i = 0; i < m_buttons.Length; i++)
        {
            m_buttons[i].gameObject.SetActive(false);
        }

        StartCoroutine(LoadExistingImagesOnStart());
    }

    public void SaveScreenToDisk()
    {
        if (m_isLocked)
            return;

        m_isLocked = true;
        StartCoroutine(SaveAndUse());
    }

    IEnumerator SaveAndUse()
    {
        HideThingsForScreenshot(true);
        yield return new WaitForEndOfFrame();
        string filename = m_count.ToString() + ".png";
        string url = Application.persistentDataPath + "/" + filename;


        Debug.Log("Saving screenshot to " + url);

#if UNITY_EDITOR
        ScreenCapture.CaptureScreenshot(url);
#else
              ScreenCapture.CaptureScreenshot(filename);
#endif
        // Take shot

        // Wait
        yield return null;

        // Debug.Log("Trying to load from " + url);
        // m_debug.text += "Trying to load from " + url + "\n";
        //LoadTheLevel(1);
        //Wait A minute to show gallery (DAM FIX)
        StartCoroutine(DelayedShare(url));

    }


    public void HideThingsForScreenshot(bool isHiding)
    {
        foreach (GameObject g in m_thingsToHide)
            g.SetActive(!isHiding);
    }

    public void ShowBackButton(bool isShowingBackButton)
    {
        foreach (GameObject g in m_galleryUIToShow)
            g.SetActive(isShowingBackButton);
    }

    public void ShowGalleryButtons(bool isShowingButtons)
    {
        foreach (RawImage ri in m_buttons)
        {
            if (isShowingButtons && ri.texture != null)
                ri.gameObject.SetActive(true);
            else
                ri.gameObject.SetActive(false);
        }

    }

    IEnumerator LoadExistingImagesOnStart()
    {
        //List<string> existingImages = new List<string>();
        string[] files = Directory.GetFiles(Application.persistentDataPath);
        int count = 0;
        foreach (string s in files)
            if (s.EndsWith("png"))
            {
                UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + s);
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(www.error);
                    m_debug.text += www.error + "\n";
                }
                else
                {
                    Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    RawImage ri = m_buttons[count].GetComponent<RawImage>();
                    ri.color = Color.white;
                    if (ri)
                    {
                        ri.texture = myTexture;
                    }
                    count++;
                }

            }
    }


    IEnumerator DelayedShare(string path)
    {
        //If we don't wait, we need a unique ID for each texture's name to load properly...
        // yield return new WaitForSeconds(2f);

        System.DateTime startTime = System.DateTime.Now.AddSeconds(6);

        while (IsFileUnavailable(path, startTime))
        {
            //Debug.Log("file locked " + Time.time);
            yield return new WaitForSeconds(.05f);
        }


            // 
            // run your code that needs the file to be loaded here
            //this path is different than the application data path, prefix it with...file:// for MacOSX
            UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + path);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
                m_debug.text += www.error + "\n";
            }
            else
            {
                Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                RawImage ri = m_buttons[m_count].GetComponent<RawImage>();
                if (ri)
                {
                    //Debug.Log("Loading the texture");
                    ri.texture = myTexture;
                    m_buttons[m_count].transform.SetAsFirstSibling();
                    m_buttons[m_count].gameObject.SetActive(true);
                }

            }


            m_isLocked = false;
            ShowBackButton(true);
            ShowGalleryButtons(true);
            m_count++;
            if (m_count >= m_buttons.Length)
                m_count = 0;

            Color color = new Color(Random.value, Random.value, Random.value);
            Camera.main.backgroundColor = color;
    }

    protected virtual bool IsFileUnavailable(string path, System.DateTime start)
    {
        //timeout if something goes wrong
        if (System.DateTime.Compare(start, System.DateTime.Now) < 0)
        {
            // Debug.Log("Timed out");
            // m_debug.text += "Timed Out \n";
            return false;
        }
        // if file doesn't exist, return true
        if (!File.Exists(path))
        {
            // Debug.Log("File does not exist");
            // m_debug.text += "File does not exist \n";
            return true;
        }

        //Give the system 5 seconds lead time, if the file is much older, it must not be a new screen capture
        if (System.DateTime.Compare(File.GetLastWriteTime(path).AddSeconds(5), System.DateTime.Now) < 0)
        {
            // Debug.Log("File is earlier than now");
            // m_debug.text += "File is earlier than now \n";
            return true;
        }

        FileInfo file = new FileInfo(path);
        FileStream stream = null;

        try
        {
            stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
        }
        catch (IOException)
        {
            //the file is unavailable because it is:
            //still being written to
            //or being processed by another thread
            //or does not exist (has already been processed)
            //Debug.Log("Does not exist or is being processed by another thread");
            return true;
        }
        finally
        {
            if (stream != null)
                stream.Close();
        }

        //file is not locked
        return false;
    }

}

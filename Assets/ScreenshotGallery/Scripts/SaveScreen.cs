//Bryan Leister, July 2022
//Script to take a screenshot and then save it to the user's disk space. The limit is the number
//of buttons, 4 in this example. That way, the old images are deleted so the app does not gradually
//fill up the users disk.
//To do - implement a Save to Photo Album function so users can save their screenshots to their phones
//photo album to use however they want. Mobile screenshots are saved as part of the app and is not accessible
//to the user otherwise.
//Will use the open source plug-in here: https://github.com/yasirkula/UnityNativeGallery
//NOTE: the plugins folder is ignored on the Git repository, you will need to install it yourself using the link above.


using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class SaveScreen : MonoBehaviour
{
    public GameObject[] m_hideDuringCapture;
    public GameObject[] m_showDuringCapture;
    public GameObject m_gallery;
    public RawImage m_featuredImage;
    public RawImage[] m_buttons;

    bool _isLocked = false;
    private GameObject _emptyButton = null;

    private void Awake()
    {
        m_gallery.SetActive(false);
        m_featuredImage.transform.parent.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        StartCoroutine(LoadExistingImagesOnStart());
    }

    public void SaveScreenToDisk()
    {
        if (_isLocked)
            return;

        _isLocked = true;
        StartCoroutine(SaveAndUse());
    }

    IEnumerator SaveAndUse()
    {
        Visibility(m_hideDuringCapture,false);
        Visibility(m_showDuringCapture,true);
        _emptyButton = null;
        for (int i =0; i < m_buttons.Length; i++)
        {
            if (m_buttons[i].texture == null)
            {
                _emptyButton = m_buttons[i].gameObject;
                break;
            }
        }

        //If gallery is full, delete the last image and overwrite with the new one
        if (_emptyButton == null)
        {
            int lastChildIndex = m_buttons[0].transform.parent.childCount - 1;
            _emptyButton = m_buttons[0].transform.parent.GetChild(lastChildIndex).gameObject;
            string path = Application.persistentDataPath + "/" + _emptyButton.name;
            if (File.Exists(path))
                File.Delete(path);

        }

        string filename = RandomStringGenerator(10) + ".png";
        _emptyButton.name = filename;
        string url = Application.persistentDataPath + "/" + filename;

        // Wait for things to be hidden, then
        // Take shot and save it to disc – full path in editor, name only for mobile
        yield return new WaitForEndOfFrame();

#if UNITY_EDITOR
        ScreenCapture.CaptureScreenshot(url);
#else
        ScreenCapture.CaptureScreenshot(filename);
#endif
        //Delay before showing the gallery
        yield return new WaitForSeconds(.5f);
        StartCoroutine(DelayedShare(url));
    }

    private void ShowButtonsThatHaveImages()
    {
        foreach (RawImage ri in m_buttons)
        {
            if (ri.texture != null)
            {
                SetAspectRatio(ri);
                ri.gameObject.SetActive(true);
            }
            else
                ri.gameObject.SetActive(false);
        }

    }

    public void ShowFeaturedImagePanel(RawImage image)
    {
        m_featuredImage.texture = image.texture;
        SetAspectRatio(image);
        m_featuredImage.name = image.gameObject.name;
        m_featuredImage.transform.parent.gameObject.SetActive(true);
    }

    private IEnumerator LoadExistingImagesOnStart()
    {
        string[] files = Directory.GetFiles(Application.persistentDataPath);
        foreach (var rawImage in m_buttons)
        {
            rawImage.gameObject.SetActive(false);
        }

        int count = 0;
        foreach (string s in files)
            if (s.EndsWith("png"))
            {
                UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + s);
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(www.error);
                }
                else if(count < m_buttons.Length)
                {
                    Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    //Debug.Log("Downloaded " + count);
                    RawImage ri = m_buttons[count].GetComponent<RawImage>();
                    if (ri)
                    {

                        ri.color = Color.white;
                        ri.texture = myTexture;
                        SetAspectRatio(ri);
                        ri.gameObject.name = s.Substring(s.Length - 14);
                        ri.gameObject.SetActive(true);
                    }
                    count++;
                }

            }
    }

    private static void SetAspectRatio(RawImage raw)
    {
        Texture myTexture = raw.texture;
        float scalar = myTexture.height / (float)myTexture.width;

        if (myTexture.height < myTexture.width)
        {
            raw.rectTransform.localScale = new Vector3(1, scalar, 1);
        }
        else
        {
            scalar = myTexture.width / (float)myTexture.height;
            raw.rectTransform.localScale = new Vector3(scalar, 1, 1);
        }
    }

    private IEnumerator DelayedShare(string path)
    {
        //If we don't wait, we need a unique ID for each texture's name to load properly...
        // yield return new WaitForSeconds(2f);

        var startTime = DateTime.Now.AddSeconds(6);

        while (IsFileUnavailable(path, startTime))
        {
            //Debug.Log("file locked " + Time.time);
            yield return new WaitForSeconds(.05f);
        }

            // run your code that needs the file to be loaded here
            //this path is different than the application data path, prefix it with...file:// for MacOSX
            var www = UnityWebRequestTexture.GetTexture("file://" + path);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                var ri = _emptyButton.GetComponent<RawImage>();
                if (ri)
                {
                    ri.texture = myTexture;
                    _emptyButton.transform.SetAsFirstSibling();
                    _emptyButton.gameObject.SetActive(true);
                }

            }


            _isLocked = false;
            Visibility(m_showDuringCapture,false);
            m_gallery.SetActive(true);
            ShowButtonsThatHaveImages();

            var color = new Color(Random.value, Random.value, Random.value);
            if (Camera.main is not null) Camera.main.backgroundColor = color;
    }

    private void Visibility(GameObject[] objects, bool isVisible)
    {
        foreach (GameObject g in objects)
            g.SetActive(isVisible);
    }

    protected virtual bool IsFileUnavailable(string path, System.DateTime start)
    {
        //timeout if something goes wrong
        if (System.DateTime.Compare(start, System.DateTime.Now) < 0)
        {
            Debug.Log("Timed out");
            return false;
        }
        // if file doesn't exist, return true
        if (!File.Exists(path))
        {
            Debug.Log("File does not exist");
            return true;
        }

        //Give the system 5 seconds lead time, if the file is much older, it must not be a new screen capture
        if (System.DateTime.Compare(File.GetLastWriteTime(path).AddSeconds(5), System.DateTime.Now) < 0)
        {
           Debug.Log("File is earlier than now");
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
            Debug.Log("Does not exist or is being processed by another thread");
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

    private static string RandomStringGenerator(int length)
    {
        string result = "";
        for (int i = 0; i < length; i++)
        {
            char c = (char)('A' + Random.Range(0, 26));
            result += c;
        }

        return result;
    }

}

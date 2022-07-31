//Bryan Leister, July 2022
//Deletes a file from the users disc, based on the Editor name of the in-game object
//That name is set using the SaveScreen script when the screenshot was saved to disc.
//Works on mobile and desktop

using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class DeleteImage : MonoBehaviour
{
    public RawImage m_rawImage;

    public void DeleteTheImage()
    {
        string filename = m_rawImage.gameObject.name;
        string path = Application.persistentDataPath + "/" + filename;

        if (File.Exists(path))
        {
            File.Delete(path);
            m_rawImage.texture = null;
            GameObject o;
            (o = m_rawImage.gameObject).SetActive(false);
            m_rawImage.texture = null;
            o.name = "[deleted]";
           //Debug.Log("Deleting " + path);
        }
        else
            Debug.LogError($"Can not delete '{path}'. File does not exist.");
    }

    public void SetCurrentButton(RawImage raw)
    {
        m_rawImage = raw;

    }
}

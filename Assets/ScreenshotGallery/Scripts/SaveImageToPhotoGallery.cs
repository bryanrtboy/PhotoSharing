using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(DeleteImage))]
public class SaveImageToPhotoGallery : MonoBehaviour
{
    public RawImage m_featuredImage;
    public TextMeshProUGUI m_results;
    public Button m_exportButton;
    private DeleteImage _deleteImage;

    private void Awake()
    {
        _deleteImage = GetComponent<DeleteImage>();
    }

    private void OnEnable()
    {
        NativeGallery.Permission permission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Write);
        if (m_exportButton && permission != NativeGallery.Permission.Granted)
            m_exportButton.interactable = false;
        Debug.Log("Checking write permission is " + NativeGallery.CheckPermission(NativeGallery.PermissionType.Write) );
    }

    private void OnDisable()
    {
        if (!m_results) return;
        m_results.transform.parent.gameObject.SetActive(false);
        m_results.text = "";

    }

    public void SaveToGallery()
    {
        string filename = _deleteImage.m_rawImage.gameObject.name;
        string path = Application.persistentDataPath + "/" + filename;

        //NativeGallery.SaveImageToGallery( string existingMediaPath, string album, string filename, MediaSaveCallback callback = null ):
        //use this function if the image is already saved on disk. Enter the file's path to existingMediaPath.

        // Save the screenshot to Gallery/Photos
        NativeGallery.SaveImageToGallery( path, "GalleryTest", filename, ( success, savedPath ) => ShowResults(success,filename,savedPath,path) );

    }

    void ShowResults(bool success,string filename, string savedPath, string path)
    {
        NativeGallery.Permission permission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Write);
        if (success && permission == NativeGallery.Permission.Granted)
        {
            Debug.Log("Saved " + filename + " to " + savedPath + ". Permission is " + permission + ". Path is " + path);
            _deleteImage.DeleteTheImage();
            m_featuredImage.texture = null;
            if (!m_results) return;
            m_results.transform.parent.gameObject.SetActive(true);
            m_results.text = "Export was successful!";

        }
        else
        {
            Debug.Log("Failed to save " + filename + " to " + path + ". Permission is " + permission + ". Path is " + path);
            if (!m_results) return;
            m_results.transform.parent.gameObject.SetActive(true);
            m_results.text = "Export failed: " + permission;
        }
    }

}

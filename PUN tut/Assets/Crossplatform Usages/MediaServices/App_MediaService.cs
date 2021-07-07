using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.EssentialKit;

public class App_MediaService : MonoBehaviour
{
    public static App_MediaService instance;
    GalleryAccessStatus readAccessStatus;
    GalleryAccessStatus readWriteAccessStatus;
    CameraAccessStatus cameraAccessStatus;

    public Texture2D currentImage;

    private void Start()
    {
        instance = this; // create a singleton of the class for reference
    }

    // to use the function: App_MediaService.instance.GetImageFromGallery();
    public void GetImageFromGallery()
    {
        GalleryAccessStatus readAccessStatus = MediaServices.GetGalleryAccessStatus(GalleryAccessMode.Read);
        if (readAccessStatus == GalleryAccessStatus.NotDetermined)
        {
            MediaServices.RequestGalleryAccess(GalleryAccessMode.Read, callback: (result, error) =>
            {
                Debug.Log("Request for gallery access finished.");
                Debug.Log("Gallery access status: " + result.AccessStatus);
            });
        }
        if (readAccessStatus == GalleryAccessStatus.Authorized)
        {
            MediaServices.SelectImageFromGallery(canEdit: true, (textureData, error) =>
            {
                if (error == null)
                {
                    Debug.Log("Select image from gallery finished successfully.");
                    //textureData.GetTexture() // This returns the texture
                    currentImage = textureData.GetTexture();
                }
                else
                {
                    Debug.Log("Select image from gallery failed with error. Error: " + error);
                }
            });
        }
    }

    // to use the function: App_MediaService.instance.TakePictureWithCamera();
    public void TakePictureWithCamera()
    {
        cameraAccessStatus = MediaServices.GetCameraAccessStatus();
        if (cameraAccessStatus == CameraAccessStatus.NotDetermined)
        {
            MediaServices.RequestCameraAccess(callback: (result, error) =>
            {
                Debug.Log("Request for camera access finished.");
                Debug.Log("Camera access status: " + result.AccessStatus);
            });
        }
        if (cameraAccessStatus == CameraAccessStatus.Authorized)
        {
            MediaServices.CaptureImageFromCamera(true, (textureData, error) =>
            {
                if (error == null)
                {
                    Debug.Log("Capture image using camera finished successfully.");
                    currentImage = textureData.GetTexture();
                }
                else
                {
                    Debug.Log("Capture image using camera failed with error. Error: " + error);
                }
            });
        }
    }

    // to use the function: App_MediaService.instance.SaveImageToGallery(image);
    // image is the image to be saved to the gallery
    public void SaveImageToGallery(Texture2D texture)
    {
        GalleryAccessStatus readWriteAccessStatus = MediaServices.GetGalleryAccessStatus(GalleryAccessMode.ReadWrite);
        if (readAccessStatus == GalleryAccessStatus.NotDetermined)
        {
            MediaServices.RequestGalleryAccess(GalleryAccessMode.ReadWrite, callback: (result, error) =>
            {
                Debug.Log("Request for gallery access finished.");
                Debug.Log("Gallery access status: " + result.AccessStatus);
            });
        }
        if (readAccessStatus == GalleryAccessStatus.Authorized)
        {
            MediaServices.SaveImageToGallery(texture, (result, error) =>
            {
                if (error == null)
                {
                    Debug.Log("Save image to gallery finished successfully.");
                }
                else
                {
                    Debug.Log("Save image to gallery failed with error. Error: " + error);
                }
            });
        }
    }

}

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
    public Texture2D currentImage; //IG_MediaService.instance.currentImage;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }
    //App_MediaService.instance.GetImageFromGallery();
    public void GetImageFromGallery()
    {
        Debug.Log("the get image method invoked");
        readAccessStatus = MediaServices.GetGalleryAccessStatus(GalleryAccessMode.Read);
        if (!(readAccessStatus == GalleryAccessStatus.Authorized))
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
                    //The event of successufully get the image goes here
                }
                else
                {
                    Debug.Log("Select image from gallery failed with error. Error: " + error);
                }
            });

        }

    }
    //App_MediaService.instance.TakePictureWithCamera();
    public void TakePictureWithCamera()
    {
        cameraAccessStatus = MediaServices.GetCameraAccessStatus();
        if (cameraAccessStatus != CameraAccessStatus.Authorized)
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
    //App_MediaService.instance.SaveImageToGallery(textureValue);
    public void SaveImageToGallery(Texture2D texture)
    {
        readWriteAccessStatus = MediaServices.GetGalleryAccessStatus(GalleryAccessMode.ReadWrite);
        if (readWriteAccessStatus != GalleryAccessStatus.Authorized)
        {
            MediaServices.RequestGalleryAccess(GalleryAccessMode.ReadWrite, callback: (result, error) =>
            {
                Debug.Log("Request for gallery access finished.");
                Debug.Log("Gallery access status: " + result.AccessStatus);
            });
        }
        if (readWriteAccessStatus == GalleryAccessStatus.Authorized)
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
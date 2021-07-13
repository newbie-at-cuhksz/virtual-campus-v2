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
    public Texture2D currentImage; //App_MediaService.instance.currentImage;
    public List<Texture2D> currentImages; // App_MediaService.instance.currentImages; to select more than one image

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        currentImages = new List<Texture2D>();
    }

    //App_MediaService.instance.GetImageFromGallery(maxSize);
    public void GetImageFromGallery(int maxSize)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create Texture from selected image
                currentImage = NativeGallery.LoadImageAtPath(path, maxSize);
                // Event is triggered here
                if (currentImage == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }
            }
        });

    }

    public void GetImagesFromGallery(int maxSize) // get more than one image, size is how many images you want to fetch
    {
        //currentImages = new List<Texture2D>(); // renew the instance
        NativeGallery.Permission permission = NativeGallery.GetImagesFromGallery((paths) =>
        {
            foreach (var path in paths)
            {
                Debug.Log("Image path: " + path);
                if (path != null)
                {
                    // Create Texture from selected image
                    currentImages.Add(NativeGallery.LoadImageAtPath(path, maxSize));
                    Debug.Log("in process the images size is: " + currentImages.Count);
                }
            }
            Debug.Log("the images size is: " + currentImages.Count);
            //Event goes here
        });
    }

    public void SetBoardGetImagesFromGallery()
    {
        SetBoardGetImagesFromGallery(512);
    }

    public void SetBoardGetImagesFromGallery(int maxSize) // get more than one image, size is how many images you want to fetch
    {
        //currentImages = new List<Texture2D>(); // renew the instance
        NativeGallery.Permission permission = NativeGallery.GetImagesFromGallery((paths) =>
        {
            foreach (var path in paths)
            {
                if (currentImages.Count >= 6) continue;
                Debug.Log("Image path: " + path);
                if (path != null)
                {
                    // Create Texture from selected image
                    currentImages.Add(NativeGallery.LoadImageAtPath(path, maxSize));
                    Debug.Log("in process the images size is: " + currentImages.Count);
                }
            }
            Debug.Log("the images size is: " + currentImages.Count);
            EventManager_Board.instance.PressedSelectImage();
        });
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

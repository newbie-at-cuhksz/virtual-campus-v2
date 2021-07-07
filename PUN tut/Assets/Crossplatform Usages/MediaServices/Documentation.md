### Media Services

1. Function

   Media services includes selecting image from library, taking picture, save the image into the gallery

   The media service is realized by a script "App_MediaService.cs", the script provides a public class which supports media services.

2. How to use

   to use the media services, create an instance of the App_MediaService class, then call the methods:

   ```csharp
   App_MediaService.instance.GetImageFromGallery();
   // get image from gallery
   App_MediaService.instance.TakePictureWithCamera();
   // get image by camera
   App_MediaService.instance.SaveImageToGallery(image);
   // save the image (type: Texture2D) into gallery
   
   App_MediaService.instance.currentImage;
   // reference the image
   // when getting images, you need to call this to get the retrieved image
   ```

   
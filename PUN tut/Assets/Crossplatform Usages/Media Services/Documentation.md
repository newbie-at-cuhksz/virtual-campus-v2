### Media Services

1. Function

   Media services includes selecting image from library, taking picture, save the image into the gallery

   The media service is realized by a script "App_MediaService.cs", the script provides a public class which supports media services.

2. How to use

   to use the media services, create an instance of the App_MediaService class, then call the methods:

   ```csharp
   App_MediaService.instance.GetImageFromGallery(maxSize);
   // get image from gallery, max size is the max size of the image (unit: ps)
   App_MeidaService.instance.GetImagesFromGallery(maxSize);
   // get multiple images from the library, max size is the max size of the image (unit: ps)
   App_MediaService.instance.TakePictureWithCamera();
   // get image by camera
   App_MediaService.instance.SaveImageToGallery(image);
   // save the image (type: Texture2D) into gallery
   
   App_MediaService.instance.currentImage;
   // reference the image
   // when getting images, you need to call this to get the retrieved image
App_MediaService.instance.currentImages;
   // call this to reference the multiple images to be retrieved
// the type is List<Texture2D>
   ```

   
note: 
   
   it is better to use a event to refresh the fetched image
   
   the following code may not function properly

   ```csharp
App_MediaService.instance.GetImageFromGallery(512);
   Texture2D imageShow = App_MediaService.instance.currentImage;
```
   
   the imageShow will not receive the most recent updated image to fetch
   
   However, you'd better to use a Event to refresh the image, the event can be added to the APP_MediaService.cs.

   eg: you should write a eventManager script and trigger the event here
   
   ![image-20210710223901464](C:\Users\11957\AppData\Roaming\Typora\typora-user-images\image-20210710223901464.png)
   
   (write a event is kind of complicated, you can use a simpler method if possible)


using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.Media;
using GalleryApp.Core;
using System.IO;

namespace GalleryApp.Android
{
    [Activity(Label = "GalleryApp.Android", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += async (sender, args) =>
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    Toast.MakeText(this, "No camera available :(.", ToastLength.Short);
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "RealTimeGallery",
                    Name = $"{DateTime.Now.ToString("yyyyMMdd-HHmmss")}.jpg"
                });

                if (file == null)
                    return;

                using (var memoryStream = new MemoryStream())
                using (var fileStream = file.GetStream())
                {
                    await fileStream.CopyToAsync(memoryStream);

                    var photoUploader = new PhotoUploader();
                    await photoUploader.UploadPhoto(memoryStream.GetBuffer(), ".jpg");
                }

                Toast.MakeText(this, "Picture uploaded.", ToastLength.Short);
            };
        }
    }
}


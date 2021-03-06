﻿using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;

namespace KillerAIMP.Android
{
    [Activity(Label = "KillerAIMP", ScreenOrientation=ScreenOrientation.Portrait, Theme = "@style/MainTheme", MainLauncher = true, Icon = "@drawable/icon",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private readonly string[] _permission =
        {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage
        };

        private const int RequestId = 0;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            RequestPermissions(_permission, RequestId);
            
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            // PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            // base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
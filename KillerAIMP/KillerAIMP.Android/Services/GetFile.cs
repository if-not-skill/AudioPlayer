using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using KillerAIMP.Services;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Environment = Android.OS.Environment;
using Exception = Java.Lang.Exception;
using String = Java.Lang.String;


[assembly: Xamarin.Forms.Dependency(typeof(KillerAIMP.Android.Services.GetFile))]
namespace KillerAIMP.Android.Services
{
    internal class GetFile : IMyFile
    {
        public IList<string> GetFileLocation()
        {
            var responce = CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
            
            var files = new List<string>();
            foreach (var dir in RootDirectory())
            {
                if (Directory.Exists(dir))
                {
                    var file = Directory.EnumerateFiles(dir).ToList<string>();  
                    file.ForEach(f => { if (f.EndsWith("mp3")) files.Add(f);});
                }
            }
            return files;
        }

        

        private IList<string> RootDirectory()
        {
            var pathlist = new List<string>();

            try
            {
                var temp = new List<string>();
                
                pathlist.Add(Environment.GetExternalStoragePublicDirectory(
                    Environment.DirectoryDocuments).AbsolutePath);
                pathlist.Add(Environment.GetExternalStoragePublicDirectory(
                    Environment.DirectoryDownloads).AbsolutePath);
                pathlist.Add(Environment.GetExternalStoragePublicDirectory(
                    Environment.DirectoryMusic).AbsolutePath);
                pathlist.Add(Environment.GetExternalStoragePublicDirectory(
                    Environment.DirectoryDocuments).AbsolutePath);

                foreach (var path in pathlist)
                {
                    if (Directory.Exists(path: path))
                    {
                        temp.AddRange(collection: Directory.EnumerateDirectories(path: path).ToList());
                    }
                }
                pathlist.AddRange(collection: temp);
                return pathlist;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
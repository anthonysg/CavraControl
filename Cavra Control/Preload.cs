using System;
using System.IO;
using System.Text;
using System.Media;
using System.Collections.Generic;

using Eto.Drawing;
using Eto.Forms;

namespace CavraControl
{
    public static class Preload
    {
        static Dictionary<string, Image> cache = new Dictionary<string, Image>();
        static string image_path = Directory.GetCurrentDirectory();




        public static void SetResourcePath(string path)
        {
            image_path = path;
        }

        public static Image ImageResource(string fileName)
        {
            if (cache.ContainsKey(fileName))
                return cache[fileName];


            var img = new Bitmap(Path.Combine(image_path, fileName));
            cache.Add(fileName, img);
            return img;
        }
    }
}

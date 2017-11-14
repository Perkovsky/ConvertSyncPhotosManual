using System;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using ImageProcessor.Imaging.Formats;
using ImageProcessor;
using ImageProcessor.Plugins.WebP.Imaging.Formats;

namespace ConvertSyncPhotosManual
{
    /// <summary>
    /// This class is needed to convert image: resize, optimize, etc
    /// </summary>
    public class Converter
    {
        private readonly int QUALITY = 40;

        private string sourceDirectoryName;
        private string destDirectoryName;

        public string SourceDirectoryName => sourceDirectoryName;

        public Converter()
        {
            // get setting from XML
            Settings settings = new Settings();

            // watcherDirectory
            sourceDirectoryName = @settings.Fields.WatcherDirectory;
            if (!Directory.Exists(sourceDirectoryName))
            {
                throw new Exception("Watcher directory not exists! See \"settings.xml\".");
            }

            // convertDirectory
            destDirectoryName = @settings.Fields.ConvertDirectory;
            if (!Directory.Exists(destDirectoryName))
            {
                throw new Exception("Convert directory not exists! See \"settings.xml\".");
            }
        }

        public void Resize(string sourceFullFileName, int width, int height)
        {
            #region get destFileName
            string sourceFileName = Path.GetFileName(sourceFullFileName);
            string sourceFullParentDirectory = Path.GetDirectoryName(sourceFullFileName);

            // fix if file copy in root parent directory
            string sourceParentDirectory = "";
            if (!Path.GetDirectoryName(sourceDirectoryName).Equals(sourceFullParentDirectory))
            {
                sourceParentDirectory = Path.GetFileName(sourceFullParentDirectory);
            }

            string destFullParentDirectory = string.Format(@"{0}{1}\", destDirectoryName, sourceParentDirectory);
            string destFileName = string.Format(@"{0}{1}", destFullParentDirectory, sourceFileName);

            //// create destination directory if exists
            //if (!Directory.Exists(destFullParentDirectory)) Directory.CreateDirectory(destFullParentDirectory);
            #endregion

            if (File.Exists(destFileName)) return;

            try
            {
                byte[] photoBytes = File.ReadAllBytes(sourceFullFileName);
                // format is automatically detected though can be changed.
                ISupportedImageFormat format = new JpegFormat { Quality = QUALITY };
                //ISupportedImageFormat format = new WebPFormat { Quality = QUALITY }; // see: https://ru.wikipedia.org/wiki/WebP
                Size size = new Size(width, 0);
                using (MemoryStream inStream = new MemoryStream(photoBytes))
                {
                    // initialize the ImageFactory using the overload to preserve EXIF metadata.
                    using (ImageFactory imageFactory = new ImageFactory(/*preserveExifData: true*/))
                    {
                        // load, resize, set the format and quality and save an image.
                        imageFactory.Load(inStream)
                                    .Resize(size)
                                    .Format(format)
                                    .Save(destFileName);
                    }
                }
                Console.WriteLine("{0} - Copied & Resized to {1}x{2}", destFileName, width, height);
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("{0} - Copied & Resized to {1}x{2} error:", destFileName, width, height) + Environment.NewLine + e.ToString());
            }
        }

        public async Task ResizeAsync(string sourceFullFileName, int width, int height)
        {
            var task = new Task(() => Resize(sourceFullFileName, width, height));
            task.Start();
            await task;
        }
    }
}

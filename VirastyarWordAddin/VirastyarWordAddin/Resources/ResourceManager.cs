using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using VirastyarWordAddin;
using VirastyarWordAddin.Log;
using System.IO.Compression;

namespace SCICT.NLP.Utility.ResourceManagement
{
    public static class ResourceManager
    {
        private static Assembly MainAssembly = null;

        public static void Init(Assembly mainAssembly)
        {
            Debug.Assert(mainAssembly != null);
            if (mainAssembly == null)
            {
                throw new ArgumentException("Main assembly must not be null!", "mainAssembly");
            }
            ResourceManager.MainAssembly = mainAssembly;
        }

        public static Stream GetResource(string resourceName)
        {
            CheckInitialized();
            string fullResName = MainAssembly.FullName.Split(',')[0] + ".Resources." + resourceName;
            return MainAssembly.GetManifestResourceStream(fullResName);
        }

        public static bool SaveResourceAs(string resourceName, string destPath)
        {
            // TODO: If directory does not exist

            CheckInitialized();
            try
            {
                Stream inputStream = GetResource(resourceName);

                if (inputStream != null)
                {
                    using (inputStream)
                    {
                        inputStream.Seek(0, SeekOrigin.Begin);

                        using (var outputStream = File.Create(destPath))
                        {
                            var data = new byte[1024];
                            int readed;
                            do
                            {
                                readed = inputStream.Read(data, 0, data.Length);
                                outputStream.Write(data, 0, readed);
                            } while (readed == data.Length);

                            return true;
                        }
                    }
                }
                else // Try .zip files
                {
                    string zippedResourceName = resourceName + ".zip";
                    inputStream = GetResource(zippedResourceName);

                    if (inputStream != null)
                    {
                        using (inputStream)
                        {
                            inputStream.Seek(0, SeekOrigin.Begin);

                            using (var outputStream = File.Create(destPath))
                            {
                                using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                                {
                                    var data = new byte[1024];
                                    int readed;
                                    do
                                    {
                                        readed = gzipStream.Read(data, 0, data.Length);
                                        outputStream.Write(data, 0, readed);
                                    } while (readed == data.Length);
                                }

                                return true;
                            }
                        }
                    }
                }
            }
            catch(UnauthorizedAccessException ex)
            {
                LogHelper.DebugException("Unable to save the resource:" + resourceName, ex);
            }
            catch (Exception ex)
            {
                LogHelper.DebugException("Unable to save the resource:" + resourceName, ex);
            }

            return false;
        }

        private static void CheckInitialized()
        {
            if (MainAssembly == null)
                throw new InvalidOperationException("ResourceManager is not initialized");
        }
    }
}

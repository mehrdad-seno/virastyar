using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;
using Microsoft.Office.Interop.Word;
using VirastyarWordAddin.Log;

namespace VirastyarWordAddin
{
    public static class SettingsHelper
    {
        /// <summary>
        /// Returns the absolute path of the given virastyar file.
        /// </summary>
        public static string GetFullPath(string name, VirastyarFilePathTypes fileFormat)
        {
            if (fileFormat == VirastyarFilePathTypes.AllUsersFiles)
            {
                return Path.Combine(GetCommonDataPath(), name);
            }
            else if (fileFormat == VirastyarFilePathTypes.UserFiles)
            {
                return Path.Combine(GetUserDataPath(), name);
            }
            else
            {
                return Path.Combine(GetInstallationPath(), name);
            }
        }

        /// <summary>
        /// Returns the base directory where this addin runs from.
        /// </summary>
        /// <returns></returns>
        public static string GetInstallationPath()
        {
            string basePath = Registry.GetValue(Constants.FullApplicationRegKey, "InstallPath", "") as string;
            if (basePath == null)
                basePath = "";

            return basePath;
        }

        public static string GetUserDataPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Constants.Virastyar);
        }

        public static string GetCommonDataPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                Constants.Virastyar);
        }

        public static OfficeVersion GetOfficeVersion()
        {
            string strVer = Globals.ThisAddIn.Application.Version;
            if (strVer.CompareTo(Constants.Office2003Version) == 0)
                return OfficeVersion.Office2003;
            if (strVer.CompareTo(Constants.Office2007Version) == 0)
                return OfficeVersion.Office2007;
            if (strVer.CompareTo(Constants.Office2010Version) == 0)
                return OfficeVersion.Office2010;

            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns the user templates path of the version of the running office.
        /// </summary>
        /// <returns></returns>
        public static string GetOfficeUserTemplatesLocation()
        {
            string ver = Globals.ThisAddIn.Application.Version;
            string templatesLocation = "";
            
            if (ver.CompareTo(Constants.Office2003Version) == 0)
            {
                string regKeyStr = Registry.CurrentUser.Name + "\\" + Constants.OfficeRegKey + "\\" + ver + "\\Common\\General";
                templatesLocation = (string)Registry.GetValue(regKeyStr, Constants.OfficeUserTemplatesName, "");
                
                if (string.IsNullOrEmpty(templatesLocation))
                {
                    string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    templatesLocation = Path.Combine(appDataPath, @"Microsoft\Templates\");
                }
                return templatesLocation;
            }
            else
            {
                var trustedLocationsKey = Registry.CurrentUser.OpenSubKey(
                    Constants.OfficeRegKey + "\\" + ver + "\\Word\\Security\\Trusted Locations");
                if (trustedLocationsKey == null)
                {
                    // TODO: Is it possible ?!
                    throw new InvalidOperationException("No trusted location is defined in the Word");
                }

                string[] subKeys = trustedLocationsKey.GetSubKeyNames();

                foreach (var subKeyName in subKeys)
                {
                    var subKeyReg = trustedLocationsKey.OpenSubKey(subKeyName);
                    string path = ((string)subKeyReg.GetValue("Path")).TrimEnd('\\');

                    if (!string.IsNullOrEmpty(path))
                    {
                        if (string.IsNullOrEmpty(templatesLocation))
                            templatesLocation = path;
                        else if (path.EndsWith(@"Microsoft\Templates"))
                            templatesLocation = path;
                    }
                }

                if (!string.IsNullOrEmpty(templatesLocation))
                    return templatesLocation;
            }

            throw new InvalidOperationException("No trusted location is defined in the Word");
        }

        /// <summary>
        /// Returns the template name accroding to the version of the running office.
        /// </summary>
        /// <returns></returns>
        public static string GetVirastyarTemplateName()
        {
            OfficeVersion ver = GetOfficeVersion();
            switch (ver)
            {
                case OfficeVersion.Office2003:
                    return Constants.Virastyar2003TemplateName;
                case OfficeVersion.Office2007:
                    return Constants.Virastyar2007TemplateName;
                case OfficeVersion.Office2010:
                    return Constants.Virastyar2010TemplateName;
                default:
                    throw new Exception("Invalid/Unknown office version");
            }
        }

        public static bool RemoveCustomization(string addInName)
        {
            for (int i = 1; i <= Globals.ThisAddIn.Application.AddIns.Count; i++)
            {
                object index = i;
                AddIn addIn = Globals.ThisAddIn.Application.AddIns.get_Item(ref index);
                if (addIn.Name.CompareTo(addInName) == 0)
                {
                    try
                    {
                        addIn.Installed = false;
                        //addIn.Delete();
                        return true;
                    }
                    catch(Exception ex)
                    {
                        LogHelper.ErrorException("Unable to remove the specified template from word Addins", ex);
                    }
                }
            }
            return false;
        }

        public static Template LoadTemplate(Application wordApp, string templateFullPath)
        {
            string templateName = Path.GetFileName(templateFullPath);
            string templatePath = Path.GetDirectoryName(templateFullPath);

            foreach (Template installedTemplate in wordApp.Templates)
            {
                if (installedTemplate.Name.CompareTo(templateName) == 0)
                {
                    if (installedTemplate.Path.CompareTo(templatePath) != 0)
                    {
                        SettingsHelper.RemoveCustomization(installedTemplate.Name);
                    }
                    else
                    {
                        //install = false;
                    }
                }
            }
            object install = true;
            wordApp.AddIns.Add(templateFullPath.ToString(), ref install);

            object path = templateFullPath;
            return wordApp.Templates.get_Item(ref path);
        }
    }
}

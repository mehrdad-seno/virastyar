using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

        public static OfficeVersions GetOfficeVersion()
        {
            string strVer = ThisAddIn.OfficeVersion;
            if (strVer.CompareTo(Constants.Office2003Version) == 0)
                return OfficeVersions.Office2003;
            if (strVer.CompareTo(Constants.Office2007Version) == 0)
                return OfficeVersions.Office2007;
            if (strVer.CompareTo(Constants.Office2010Version) == 0)
                return OfficeVersions.Office2010;

            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns the user templates path of the version of the running office.
        /// </summary>
        /// <exception cref="InvalidOperationException" />
        /// <returns></returns>
        public static string GetOfficeUserTemplatesLocation()
        {
            string ver = ThisAddIn.OfficeVersion;
            string templatesLocation = "";

            if (string.Compare(ver, Constants.Office2003Version, StringComparison.Ordinal) == 0)
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
                    // Is it possible ?!
                    throw new InvalidOperationException("No trusted location is defined in the Word. trustedLocationsKey is null");
                }

                string[] subKeys = trustedLocationsKey.GetSubKeyNames();

                foreach (var subKeyName in subKeys)
                {
                    var subKeyReg = trustedLocationsKey.OpenSubKey(subKeyName);
                    string path = ((string)subKeyReg.GetValue("Path", "")).TrimEnd('\\');
                    string description = ((string)subKeyReg.GetValue("Description", ""));

                    if (!string.IsNullOrEmpty(path))
                    {
                        if (string.IsNullOrEmpty(templatesLocation))
                            templatesLocation = path;
                        else if (path.EndsWith(@"Microsoft\Templates"))
                            templatesLocation = path;
                        else if (description == Constants.VirastyarTrustedLocationDescription)
                            templatesLocation = path;
                    }
                }

                if (!string.IsNullOrEmpty(templatesLocation))
                    return templatesLocation;
            }

            throw new InvalidOperationException("No trusted location is defined in the Word");
        }

        public static int GetLastOfficeUserTemplateLocationIndex()
        {
            const int @default = 50;
            string ver = ThisAddIn.OfficeVersion;

            if (string.Compare(ver, Constants.Office2003Version, StringComparison.Ordinal) == 0)
            {
                return @default;
            }
            else
            {
                var trustedLocationsKey = Registry.CurrentUser.OpenSubKey(
                    Constants.OfficeRegKey + "\\" + ver + "\\Word\\Security\\Trusted Locations");

                if (trustedLocationsKey == null)
                {
                    // Is it possible ?!
                    throw new InvalidOperationException("No trusted location is defined in the Word. trustedLocationsKey is null");
                }

                string[] subKeys = trustedLocationsKey.GetSubKeyNames();

                int index = 0;
                bool alreadyExisted = false;
                // Iterate and see if there is already a location with the same path
                foreach (var subKeyName in subKeys.OrderBy(s => s))
                {
                    // Try to find the last index
                    // Microsoft, please continue using your existing conventions and don't change it whitout notifying us!
                    if (subKeyName.ToLower().StartsWith("location"))
                    {
                        string indexPart = subKeyName.ToLower().Substring("location".Length);
                        int curIndex;
                        if (int.TryParse(indexPart, out curIndex))
                        {
                            if (curIndex > index)
                                index = curIndex;
                        }
                    }
                }

                return index + 1;
            }
        }

        public static void SetOfficeUserTemplatesLocation(string location)
        {
            throw new NotImplementedException();
			string ver = ThisAddIn.OfficeVersion;
            string templatesLocation = "";

            if (string.Compare(ver, Constants.Office2003Version, StringComparison.Ordinal) == 0)
            {
                //string regKeyStr = Registry.CurrentUser.Name + "\\" + Constants.OfficeRegKey + "\\" + ver + "\\Common\\General";
                //templatesLocation = (string)Registry.GetValue(regKeyStr, Constants.OfficeUserTemplatesName, "");

                //if (string.IsNullOrEmpty(templatesLocation))
                //{
                //    string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                //    templatesLocation = Path.Combine(appDataPath, @"Microsoft\Templates\");
                //}
                //return templatesLocation;
            }
            else
            {
                var trustedLocationsKey = Registry.CurrentUser.OpenSubKey(
                    Constants.OfficeRegKey + "\\" + ver + "\\Word\\Security\\Trusted Locations");

                if (trustedLocationsKey == null)
                {
                    // Is it possible ?!
                    throw new InvalidOperationException("No trusted location is defined in the Word. trustedLocationsKey is null");
                }

                string[] subKeys = trustedLocationsKey.GetSubKeyNames();

                int index = 0;
                bool alreadyExisted = false;
                // Iterate and see if there is already a location with the same path
                foreach (var subKeyName in subKeys.OrderBy(s => s))
                {
                    // Try to find the last index
                    // Microsoft, please continue using your existing conventions and don't change it whitout notifying us!
                    if (subKeyName.ToLower().StartsWith("location"))
                    {
                        string indexPart = subKeyName.ToLower().Substring("location".Length);
                        int curIndex;
                        if (int.TryParse(indexPart, out curIndex))
                        {
                            if (curIndex > index)
                                index = curIndex;
                        }
                    }

                    var subKeyReg = trustedLocationsKey.OpenSubKey(subKeyName);
                    string path = ((string)subKeyReg.GetValue("Path", "")).TrimEnd('\\');

                    if (!string.IsNullOrEmpty(path))
                    {
                        if (string.Compare(
                                Path.GetFullPath(location).TrimEnd('\\'),
                                Path.GetFullPath(path).TrimEnd('\\'),
                                StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            return;
                        }
                    }
                }

                index += 1;
                var newLocationRegKey = trustedLocationsKey.CreateSubKey("Location" + index);
                if (newLocationRegKey != null)
                {
                    newLocationRegKey.SetValue("Description", "Virastyar Template location", RegistryValueKind.String);
                    newLocationRegKey.SetValue("Path", Path.GetFullPath(location), RegistryValueKind.String);
                }
            }
        }

        /// <summary>
        /// Returns the template name accroding to the version of the running office.
        /// </summary>
        /// <returns></returns>
        public static string GetVirastyarTemplateName()
        {
            OfficeVersions ver = GetOfficeVersion();
            switch (ver)
            {
                case OfficeVersions.Office2003:
                    return Constants.Virastyar2003TemplateName;
                case OfficeVersions.Office2007:
                    return Constants.Virastyar2007TemplateName;
                case OfficeVersions.Office2010:
                    return Constants.Virastyar2010TemplateName;
                default:
                    throw new Exception("Invalid/Unknown office version");
            }
        }

        public static bool RemoveCustomization(string addInName)
        {
            for (int i = 1; i <= Globals.ThisAddIn.Application.AddIns.Count; i++)
            {
                AddIn addIn;
                object index = i;

                try
                {
                    addIn = Globals.ThisAddIn.Application.AddIns.get_Item(ref index);
                }
                catch (COMException ex)
                {
                    LogHelper.ErrorException("Error while retrieving addin with AddIns.get_Item", ex);
                    continue;
                }
                if (string.Compare(addIn.Name, addInName, true) == 0)
                {
                    try
                    {
                        addIn.Installed = false;
                        return true;
                    }
                    catch(Exception ex)
                    {
                        LogHelper.ErrorException(string.Format(
                            "Unable to remove the specified template ({0}) from word Addins", addInName), ex);
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

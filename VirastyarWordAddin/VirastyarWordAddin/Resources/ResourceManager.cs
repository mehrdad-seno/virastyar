// Virastyar
// http://www.virastyar.ir
// Copyright (C) 2011 Supreme Council for Information and Communication Technology (SCICT) of Iran
// 
// This file is part of Virastyar.
// 
// Virastyar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Virastyar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Virastyar.  If not, see <http://www.gnu.org/licenses/>.
// 
// Additional permission under GNU GPL version 3 section 7
// The sole exception to the license's terms and requierments might be the
// integration of Virastyar with Microsoft Word (any version) as an add-in.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using VirastyarWordAddin;

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
            CheckInitialized();
            try
            {
                Stream inputStream = GetResource(resourceName);
                inputStream.Seek(0, SeekOrigin.Begin);

                // TODO: If directory does not exist

                Stream outputStream = File.Create(destPath);
                byte[] data = new byte[1024 * 1024];
                int readed;
                do
                {
                    readed = inputStream.Read(data, 0, data.Length);
                    outputStream.Write(data, 0, readed);
                } while (readed == data.Length);
                inputStream.Close();
                outputStream.Close();
                return true;
            }
            catch (Exception ex)
            {
                ThisAddIn.DebugWriteLine(ex);
                return false;
            }
        }

        private static void CheckInitialized()
        {
            if (MainAssembly == null)
                throw new InvalidOperationException("ResourceManager is not initialized");
        }
    }
}

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace VirastyarWordAddin
{
    public class TipOfTheDayData
    {
        public List<string> TipRtfs { get; set; }
        public List<string> TipLinks { get; set; }

        public TipOfTheDayData()
        {
            TipRtfs = new List<string>();
            TipLinks = new List<string>();
        }

        public TipOfTheDayData(XmlReader reader)
            : this()
        {
            LoadFromStream(reader);
        }


        public TipOfTheDayData(string fileName)
            : this()
        {
            LoadFromFile(fileName);
        }

        public bool SaveToFile(string fileName)
        {
            try
            {
                XDocument xdoc = new XDocument();
                var rootElem = new XElement("root");

                for(int i = 0; i < TipRtfs.Count; i++)
                {
                    string tip = TipRtfs[i];
                    string link = TipLinks[i].Trim();
                    var tipElem = new XElement("Tip", tip);
                    if (!String.IsNullOrEmpty(link))
                        tipElem.Add(new XAttribute("link", link));
                    rootElem.Add(tipElem);
                }

                xdoc.Add(rootElem);

                xdoc.Save(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool LoadFromFile(string fileName)
        {
            try
            {
                XmlReader reader = XmlReader.Create(fileName);
                return LoadFromStream(reader);
            }
            catch
            {
                return false;
            }
        }

        public bool LoadFromStream(XmlReader tipStream)
        {
            this.TipRtfs.Clear();

            try
            {
                XDocument xdoc = XDocument.Load(tipStream, LoadOptions.PreserveWhitespace);
                var rootElem = xdoc.Element("root");

                foreach (var tipElem in rootElem.Elements("Tip"))
                {
                    this.TipRtfs.Add(tipElem.Value);
                    var linkAttr = tipElem.Attribute("link");
                    if (linkAttr != null)
                        this.TipLinks.Add(linkAttr.Value.Trim());
                    else
                        this.TipLinks.Add("");
                }

                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}

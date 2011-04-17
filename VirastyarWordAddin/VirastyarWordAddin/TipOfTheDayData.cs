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

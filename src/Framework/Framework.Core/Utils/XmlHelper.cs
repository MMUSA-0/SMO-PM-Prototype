using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Framework.Core.Utils
{
    public class XmlHelper
    {
        public static string GetXmlFromObject(object o)
        {
            var sw = new StringWriter();
            XmlTextWriter tw = null;
            try
            {
                var serializer = new XmlSerializer(o.GetType());
                tw = new XmlTextWriter(sw);
                serializer.Serialize(tw, o);
            }
            catch (Exception ex)
            {
                //Handle Exception Code
            }
            finally
            {
                sw.Close();
                tw?.Close();
            }

            return sw.ToString();
        }

        public static object GetObjectFromXml(string xml, Type objectType)
        {
            StringReader strReader = null;
            XmlTextReader xmlReader = null;
            object obj = null;
            try
            {
                strReader = new StringReader(xml);
                var serializer = new XmlSerializer(objectType);
                xmlReader = new XmlTextReader(strReader);
                obj = serializer.Deserialize(xmlReader);
            }
            catch (Exception exp)
            {
                //Handle Exception Code
            }
            finally
            {
                xmlReader?.Close();
                strReader?.Close();
            }

            return obj;
        }
    }
}
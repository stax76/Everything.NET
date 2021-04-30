
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace EverythingNET
{
    [Serializable()]
    public class AppSettings
    {
        public string[] RecentSearches = new string[] { };
    }

    class SettingsManager
    {
        static string _SettingsFile;

        public static string SettingsFile {
            get {
                if (_SettingsFile == null)
                {
                    string fileName = "Settings.xml";
                    string test = AppDomain.CurrentDomain.BaseDirectory + fileName;

                    if (File.Exists(test))
                        return _SettingsFile = test;

                    string settingsDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                        "\\" + (Assembly.GetEntryAssembly().GetCustomAttributes(
                        typeof(AssemblyProductAttribute)).First() as AssemblyProductAttribute).Product;

                    if (!Directory.Exists(settingsDir))
                        Directory.CreateDirectory(settingsDir);

                    _SettingsFile = settingsDir + "\\" + fileName;
                }

                return _SettingsFile;
            }
        }

        public static AppSettings Load()
        {
            if (!File.Exists(SettingsFile))
                return new AppSettings();

            XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
            XmlTextReader reader = new XmlTextReader(SettingsFile);
            return (AppSettings)serializer.Deserialize(reader);
        }

        public static void Save(object obj)
        {
            using XmlTextWriter writer = new XmlTextWriter(SettingsFile, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            serializer.Serialize(writer, obj);
        }
    }
}

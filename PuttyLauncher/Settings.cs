using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PuttyLauncher
{
    public class Settings
    {
        [XmlIgnore]
        private static readonly object LOCKOBJ;
        [XmlIgnore]
        private static readonly string FILENAME;
        [XmlIgnore]
        private static DateTime? fileDate;
        [XmlIgnore]
        private static Settings settings;

        [XmlElement("putty-path")]
        public string PuttyPath { get; set; }

        static Settings()
        {
            LOCKOBJ = new object();
            FILENAME = Path.Combine(AppContext.BaseDirectory, "settings.xml");

            fileDate = null;
            settings = null;
        }

        private Settings()
        {
            PuttyPath = string.Empty;
        }

        public static Settings Load()
        {
            lock (LOCKOBJ)
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(FILENAME);

                    if (fileInfo.LastWriteTime == fileDate && settings != null)
                    {
                        return settings;
                    }

                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));

                    using (FileStream stream = new FileStream(FILENAME, FileMode.Open, FileAccess.Read))
                    {
                        settings = serializer.Deserialize(stream) as Settings;
                        fileDate = fileInfo.LastWriteTime;

                        return settings;
                    }
                }
                catch
                {
                    fileDate = null;
                    settings = null;

                    Settings defaultSetting = new Settings();

                    if (File.Exists(FILENAME) == false)
                    {
                        Save(defaultSetting);
                    }
                    return defaultSetting;
                }
            }
        }

        public static bool Save(Settings settings)
        {
            lock (LOCKOBJ)
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));

                    using (FileStream stream = new FileStream(FILENAME, FileMode.Create, FileAccess.Write))
                    {
                        using (XmlWriter writer = XmlWriter.Create(stream, new XmlWriterSettings() { Indent = true, IndentChars = "  ", Encoding = new UTF8Encoding(false) }))
                        {
                            serializer.Serialize(writer, settings);
                        }
                    }
                    return true;
                }
                catch { }

                return false;
            }
        }
    }
}

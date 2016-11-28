using System;
using System.Linq;
using System.IO;
using System.Xml.Serialization;

namespace CopyApp
{
    class Config
    {
        public static void Directory_Save(DirectoryCpoier DC)
        {
            FileStream fs = new FileStream("Config.cfg", FileMode.OpenOrCreate);
            XmlSerializer xs = XmlSerializer.FromTypes(new[] { typeof(DirectoryCpoier) })[0];
            xs.Serialize(fs, DC);
            fs.Close();
        }

        public static DirectoryCpoier Directory_Read()
        {
            if (File.Exists("Config.cfg") && File.ReadAllBytes("Config.cfg").Length > 0)
            {
                FileStream fs = new FileStream("Config.cfg", FileMode.OpenOrCreate);
                XmlSerializer xs = XmlSerializer.FromTypes(new[] { typeof(DirectoryCpoier) })[0];
                DirectoryCpoier DC = (DirectoryCpoier)xs.Deserialize(fs);
                fs.Close();
                return DC;
            }
            return null;
        }

        public static void Delete_Config()
        {
            if (File.Exists("Config.cfg"))
            {
                File.Delete("Config.cfg");
            }
        }
    }
}

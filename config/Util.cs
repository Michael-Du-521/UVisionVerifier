using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using System.Drawing;
using System.Xml;

namespace VisionUBei.config
{
    public static class Util
    {
        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key, string value, string fileName);
        public static UBeiConfig ReadConfig(string path, string file)
        {
            UBeiConfig config = new UBeiConfig();
            if (string.IsNullOrEmpty(path + "\\" + file)) return null;
            XmlDocument xmlDoc = new XmlDocument();
     
            try
            {
                xmlDoc.Load(path + "\\" + file);
                XmlNode Node = xmlDoc.SelectSingleNode("/System/Common");
        
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("基础配置加载异常 \r\n 异常原因：" + ex.ToString());
            }

            return config;
        }
    }
}

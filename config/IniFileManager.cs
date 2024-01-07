using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VisionUBei;
using VisionUBei.Log;

namespace UVisionFTPUploader
{
    public class IniFileManager
    {
     

        //fields
        [DllImport("kernel32")]
        public static extern bool GetPrivateProfileString(string section, string key, string init, StringBuilder value,int size, string filename);

        [DllImport("kernel32")]
        public static extern uint GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault, string lpFileName);

        [DllImport("kernel32")]
        public static extern bool WritePrivateProfileString(string section, string key, string value, string filename);
        private static string _path;
        private static bool _isOpen;


        //fields_IniFileManager
        private Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
        public static List<string> listDefaultPath;//default upload paths
        public static string productionLineName;//产线名称
        public static string stationName;//工站名称
        public static string uploadCycle;//备份周期

        public static string fTPSeverAddress;
        public static string remoteRootFolderName;
        public static string fTPUserName;
        public static string fTPUserPassword;

        public static string isAutoRun; // true or false=>若启动软件后 2min 无操作 将自动运行上传功能
        public static string autoRunWaitingPeriod;

        //constuctors
        public IniFileManager(string iniFilePath)
        {
            Load(iniFilePath);
            IniFileManager.productionLineName = data["PARAM"]["productionLineName"];
            IniFileManager.stationName = data["PARAM"]["stationName"];
            IniFileManager.uploadCycle = data["PARAM"]["uploadCycle"];
            IniFileManager.fTPSeverAddress = data["PARAM"]["fTPSeverAddress"];
            IniFileManager.remoteRootFolderName = data["PARAM"]["remoteRootFolderName"];
            IniFileManager.fTPUserName = data["PARAM"]["fTPUserName"];
            IniFileManager.fTPUserPassword = data["PARAM"]["fTPUserPassword"]; 
            IniFileManager.isAutoRun = data["PARAM"]["isAutoRun"];
            IniFileManager.autoRunWaitingPeriod = data["PARAM"]["autoRunWaitingPeriod"];
            IniFileManager.listDefaultPath = new List<string>(data["PARAM"]["listDefaultPath"].Split(',').Select(item => item.Trim()).ToList()); // Use Select to trim spaces
        }

        //methods
        #region iniFile整体加载 整体写入

        /// <summary>
        /// Load data from .ini into a data dictionary
        /// </summary>
        private void Load(string iniFilePath)
        {
            string currentSection = null;

            foreach (string line in File.ReadLines(iniFilePath))
            {
                string trimmedLine = line.Trim();
                //handle section
                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                    data[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                //handle key and value
                else if (!string.IsNullOrWhiteSpace(currentSection) && trimmedLine.Contains("="))
                {
                    string[] parts = trimmedLine.Split(new char[] { '=' }, 2);
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();
                    data[currentSection][key] = value;
                }
            }
        }

        public string GetValue(string section, string key)
        {
            if (data.TryGetValue(section, out var sectionData) && sectionData.TryGetValue(key, out var value))
            {
                return value;
            }
            return null; // Key or section not found
        }

        public void SetValue(string section, string key, string value)
        {
            //如果data 字典中不包含该section,key and value
            if (!data.TryGetValue(section, out var sectionData))
            {
                sectionData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                data[section] = sectionData;
            }
            //如果已经包含该参数组
            sectionData[key] = value;
        }

        public void Save(string iniFilePath)
        {
            List<string> lines = new List<string>();
            // data is a three levels dictionary for which contains a section as the first level and key-value as the second and third levels
            foreach (var sectionKvp in data)
            {
                lines.Add($"[{sectionKvp.Key}]");

                foreach (var keyValuePair in sectionKvp.Value)
                {
                    lines.Add($"{keyValuePair.Key}={keyValuePair.Value}");
                }
            }
            //write all lines into iniFilePath
            File.WriteAllLines(iniFilePath, lines);
        }

        #endregion
        #region 开启与关闭文件
        public static bool Open(string file)
        {
            if (!File.Exists(file))
            {
                return false;
            }
            _isOpen = true;
            _path = file;
            Global.AddLog(LoggerType.Main, LogMsgType.Info, "开始读取INI文件.路径：" + file);

            return true;
        }

        public static void Close()
        {
            _isOpen = false;
            _path = string.Empty;
            Global.AddLog(LoggerType.Main, LogMsgType.Info, "读取INI文件结束.");

        }

        #endregion

        #region 读取参数

        public static bool ReadBool(string section, string key, bool value)
        {
            //Global.AddLog(LoggerType.Function, LogMsgType.Info, string.Format("IniFile.ReadBool()"));
            if (!_isOpen || string.IsNullOrEmpty(_path))
            {
                Global.AddLog(LoggerType.Main, LogMsgType.Info, string.Format("读取INI参数失败. Section={0}  key={1}  defaultValue={2}", section, key, value));
                return value;
            }

            string init = value ? "true" : "false";
            StringBuilder buff = new StringBuilder(64);
            bool re = GetPrivateProfileString(section, key, init, buff, 64, _path);

            if (!re)
            {
                Global.AddLog(LoggerType.Main, LogMsgType.Info, string.Format("读取INI参数失败. Section={0}  key={1}  defaultValue={2}", section, key, value));
                return value;
            }
            string read = ((buff.ToString()).Trim()).ToLower();

            Global.AddLog(LoggerType.Main, LogMsgType.Info, string.Format("读取INI参数. Section={0}  key={1}  value={2}", section, key, read));

            return read == "true";
        }

        public static int ReadInteger(string section, string key, int value)
        {
            //Global.AddLog(LoggerType.Function, LogMsgType.Info, string.Format("IniFile.ReadInteger()"));
            if (!_isOpen || string.IsNullOrEmpty(_path))
            {
                Global.AddLog(LoggerType.Main, LogMsgType.Info, string.Format("读取INI参数失败. Section={0}  key={1}  defaultValue={2}", section, key, value));
                return value;
            }
            uint re = GetPrivateProfileInt(section, key, value, _path);
            Global.AddLog(LoggerType.Main, LogMsgType.Info, string.Format("读取INI参数. Section={0}  key={1}  value={2}", section, key, re));
            return (int)re;
        }

        public static string ReadString(string section, string key, string value)
        {
            //Global.AddLog(LoggerType.Function, LogMsgType.Info, string.Format("IniFile.ReadString()"));
            if (!_isOpen || string.IsNullOrEmpty(_path))
            {
                Global.AddLog(LoggerType.Main, LogMsgType.Info, string.Format("读取INI参数失败. Section={0}  key={1}  defaultValue={2}", section, key, value));
                return value;
            }

            string init = value == null ? string.Empty : value.Trim();
            StringBuilder buff = new StringBuilder(64);

            bool re = GetPrivateProfileString(section, key, init, buff, 64, _path);
            if (!re)
            {
                Global.AddLog(LoggerType.Main, LogMsgType.Info, string.Format("读取INI参数失败. Section={0}  key={1}  defaultValue={2}", section, key, value));
                return value;
            }
            string read = (buff.ToString()).Trim();
            Global.AddLog(LoggerType.Main, LogMsgType.Info, string.Format("读取INI参数. Section={0}  key={1}  value={2}", section, key, read));
            return read;
        }

        public static double ReadDouble(string section, string key, double value)
        {
            //Global.AddLog(LoggerType.Function, LogMsgType.Info, string.Format("IniFile.ReadDouble()"));
            if (!_isOpen || string.IsNullOrEmpty(_path))
            {
                Global.AddLog(LoggerType.Main, LogMsgType.Info, string.Format("读取INI参数失败. Section={0}  key={1}  defaultValue={2}", section, key, value));
                return value;
            }

            string init = string.Empty;
            StringBuilder buff = new StringBuilder(64);
            bool re = GetPrivateProfileString(section, key, init, buff, 64, _path);

            if (!re)
            {
                Global.AddLog(LoggerType.Main, LogMsgType.Info, string.Format("读取INI参数失败. Section={0}  key={1}  defaultValue={2}", section, key, value));
                return value;
            }
            string read = ((buff.ToString()).Trim()).ToLower();
            double.TryParse(read, out value);
            Global.AddLog(LoggerType.Main, LogMsgType.Info, string.Format("读取INI参数. Section={0}  key={1}  value={2}", section, key, read));
            return value;
        }

        #endregion

        #region 写入参数

        public static void WriteBool(string section, string key, bool value)
        {
            //Global.AddLog(LoggerType.Function, LogMsgType.Info, string.Format("IniFile.WriteBool()"));
            if (!_isOpen || string.IsNullOrEmpty(_path))
            {
                Global.AddLog(LoggerType.Main, LogMsgType.Info, string.Format("写入INI参数失败. Section={0}  key={1}  defaultValue={2}", section, key, value));
                return;
            }
            string write = value ? "true" : "false";
            WritePrivateProfileString(section, key, write, _path);
            Global.AddLog(LoggerType.Main, LogMsgType.Info, string.Format("写入INI参数. Section={0}  key={1}  value={2}", section, key, write));
        }

        public static void WriteInteger(string section, string key, int value)
        {
            //Global.AddLog(LoggerType.Function, LogMsgType.Info, string.Format("IniFile.WriteInteger()"));
            if (!_isOpen || string.IsNullOrEmpty(_path))
            {
                Global.AddLog(LoggerType.Main, LogMsgType.Info, string.Format("写入INI参数失败. Section={0}  key={1}  defaultValue={2}", section, key, value));
                return;
            }
            string write = value.ToString();
            WritePrivateProfileString(section, key, write, _path);
            Global.AddLog(LoggerType.Main, LogMsgType.Info, string.Format("写入INI参数. Section={0}  key={1}  value={2}", section, key, write));
        }

        public static void WriteString(string section, string key, string value)
        {
            //Global.AddLog(LoggerType.Function, LogMsgType.Info, string.Format("IniFile.WriteString()"));
            if (!_isOpen || string.IsNullOrEmpty(_path))
            {
                Global.AddLog(LoggerType.Main, LogMsgType.Info, string.Format("写入INI参数失败. Section={0}  key={1}  defaultValue={2}", section, key, value));
                return;
            }
            string write = value == null ? string.Empty : value.Trim();
            WritePrivateProfileString(section, key, write, _path);
            Global.AddLog(LoggerType.Main, LogMsgType.Info, string.Format("写入INI参数. Section={0}  key={1}  value={2}", section, key, write));
        }

        public static void WriteDouble(string section, string key, double value)
        {
            //Global.AddLog(LoggerType.Function, LogMsgType.Info, string.Format("IniFile.WriteDouble()"));
            if (!_isOpen || string.IsNullOrEmpty(_path))
            {
                Global.AddLog(LoggerType.Main, LogMsgType.Info, string.Format("写入INI参数失败. Section={0}  key={1}  defaultValue={2}", section, key, value));
                return;
            }
            string write = value.ToString();
            WritePrivateProfileString(section, key, write, _path);
            Global.AddLog(LoggerType.Main, LogMsgType.Info, string.Format("写入INI参数. Section={0}  key={1}  value={2}", section, key, write));
        }
        #endregion



    }
}


using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Xml;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using VisionUBei.Log;
using VisionUBei.config;

namespace VisionUBei
{
    public static class Global
    {
        public static UBeiConfig Config;
        public static string appPath;
        public static string ConfigFilesPath="ConfigFiles\\System";
        public static string SytemConfigFile= "System.ini";
        public static string LogConfigFile= "Log.xml";
        public static LogProcessor _mLog4Net;
        public static bool Init()
        {
            bool flag = true;
            try
            {
       
                appPath = System.Environment.CurrentDirectory.Substring(0, System.Environment.CurrentDirectory.Length - 3);
                ConfigFilesPath = appPath + ConfigFilesPath;

         

                //Config = Util.ReadConfig(ConfigFilesPath, SytemConfigFile);

                _mLog4Net = LogProcessor.Instance;
                string logPath = "";
                string logName = "";
                int logSavingPeriod = 0;


                //系统配置初始化
                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.Load(ConfigFilesPath + "\\" + LogConfigFile);

                    XmlNode xmlNode = xmlDoc.SelectSingleNode("Log");
                    xmlNode = xmlNode.SelectSingleNode("LogConfig");
                    logPath = xmlNode.SelectSingleNode("Path").InnerText;
                    logName = xmlNode.SelectSingleNode("Name").InnerText;
                    logSavingPeriod = Convert.ToInt32(xmlNode.SelectSingleNode("SavingPeriod").InnerText);
                }
                catch
                {
                    MessageBox.Show("Uvision System Log Init Fail");

                    Environment.Exit(0);
                }
                if (_mLog4Net.Init(logPath, logName, logSavingPeriod) == false)
                {
                    MessageBox.Show("Log Instance Init Fail");

                    Environment.Exit(0);
                }
                //PLC初始化
                #region PLC初始化
                //if (_mUVisionSystem.Initialize_PLC() == false)
                //{
                //    MessageBox.Show("UvisionSystem Initialize_PLC Fail");

                //    Environment.Exit(0);
                //}
                #endregion



                //读取model
                //  Global.Config.model_parameter.Read_INI_File(p);




            }
            catch (Exception)
            {
                return false;
            }
            return flag;
        }

      
       
        public static void SetFormScreen(Form form, int screen = 0)
        {
            if (Screen.AllScreens.Length <= 1)
                return;
            if (Screen.AllScreens.Length < screen)
                return;
            form.Location = new System.Drawing.Point(Screen.AllScreens[screen].Bounds.X + (Screen.AllScreens[screen].Bounds.Width - form.Width) / 2, Screen.AllScreens[screen].Bounds.Y + (Screen.AllScreens[screen].Bounds.Height - form.Height) / 2);
        }
        public static void AddLog(LoggerType type, LogMsgType info, string message)
        {

        }
        public static void Close()
        {
            try
            {
                if (Config != null)
                {
                    Config.Clear();
                    Config = null;
                }
                AddLog(LoggerType.Main, LogMsgType.Info, "Gloabl全局资源清理成功.");
            }
            catch (Exception e)
            {
                Global.AddLog(LoggerType.Main, LogMsgType.Error, "发生异常,位置: Global.Close(),提示:" + e.Message);
            }
        }
    }
}

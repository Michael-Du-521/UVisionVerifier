//using VisionUBei.PlcModule.UVision;
using VisionUBei.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace VisionUBei.config
{
    public class UBeiConfig
    {

        //Common
        public string ModelConfigPath { get; set; }
        public string LogConfigFile { get; set; }

        //Plc Config
        //public PlcCommConfig PlcConfig { get; set; }
   
        //Log Config
        public string LogDirPath { get; set; }


        /// <summary>
        /// 配置对象 UbeiConfig
        /// </summary>
        public UBeiConfig()
        {

            Global.AddLog(LoggerType.Main, LogMsgType.Info, "新建配置对象UBeiConfig.");
         
        }
        /// <summary>
        /// 清空 配置对象 UbeiConfig
        /// </summary>
        public void Clear()
        {
       
            Global.AddLog(LoggerType.Main, LogMsgType.Info, "释放配置对象UBeiConfig.");
      
        }
    }
}

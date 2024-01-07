using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UVisionVerifier
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            try
            {
                if (!CheckProcess())
                {
                    Environment.Exit(0);
                    return;
                }
                //程序主窗口启动
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormUVisionVerifier());
            }
            
            catch (Exception e)
            {
                MessageBox.Show("发现异常,位置：Program.Main(),提示：" + e.Message, "错误提示");
            }
            finally
            {
                Environment.Exit(0);//关闭所有线程
            }
        
        }


        /// <summary>
        ///   检测系统中是否已经运行了本程序
        /// </summary>
        /// <returns>true 系统中已运行了本程序;false 系统中未运行本程序</returns>
        static private bool CheckProcess()
        {
            string name = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            if (System.Diagnostics.Process.GetProcessesByName(name).Length > 1)
            {
                MessageBox.Show("系统已存在该进程,请先关闭再开启！", "提示");
                return false;
            }
            return true;
        }

    }
}

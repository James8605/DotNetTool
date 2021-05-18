using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: log4net.Config.XmlConfigurator(Watch = true, ConfigFile = "log4net.config")]
namespace DotNetTool
{
    public class LogHelper
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger("log");

        public static void Info(string log)
        {
            _logger.Info(log);
        }

        public static void Error(Exception ex)
        {
            _logger.Error(ex);
        }
    }
}

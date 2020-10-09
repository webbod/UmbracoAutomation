using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Umbraco.SiteBuilder.Services
{
    public static class RunOnce
    {
        private static string SemaphorePath()
        {
            var root = HttpRuntime.AppDomainAppPath;
            return Path.Combine(root, @"App_Data\TEMP\SiteBuilder.AlreadyRun");
        }

        public static void CheckIfAlreadyRun()
        {
            if (File.Exists(SemaphorePath()))
            {
                throw new OperationCanceledException();
            }
        }

        public static void RecordFirstRun()
        {
            File.WriteAllText(SemaphorePath(), $"run on {DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}");
        }

    }
}

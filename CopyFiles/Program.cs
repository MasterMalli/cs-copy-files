using System;
using System.Configuration;

namespace CopyFiles
{
    /// <summary>
    /// A program that takes a list of SKUs, then uses each SKU in this list to search a folder structure ("copyFrom" directory).
    /// Each file it finds within the folder structure that includes the same SKU in its file name will be copied to the new location.
    /// </summary>
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Retrieve paths set by user within App.config.
            string csvPath = ConfigurationManager.AppSettings["CsvPath"];
            string copyTo = ConfigurationManager.AppSettings["CopyTo"];
            string from = ConfigurationManager.AppSettings["CopyFrom"];
            string logPath = ConfigurationManager.AppSettings["LogPath"];

            BatchCopy batchCopy = new BatchCopy(fileLogLocation: logPath, copyFrom: from);
            // Read CSV - this CSV will contain list of SKUs to copy.
            batchCopy.ReadList(csvPath);
            // Search 'copy from' directory using above list, and copy files across to new location.
            batchCopy.Run(copyTo);
        }
    }
}

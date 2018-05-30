using System.IO;

namespace CopyFiles
{
    class BatchCopy
    {
        public string FileLogLocation { get; private set; }
        private string[] _list;
        CopyFile copyFile;

        public BatchCopy(string fileLogLocation, string copyFrom)
        {
            FileLogLocation = fileLogLocation;
            // Create copyFile object and pass into the constructor the log location and copy from location.
            copyFile = new CopyFile(logFileLocation: FileLogLocation, chosenCopyFrom: copyFrom);
        }

        /// <summary>
        /// Reads each line of the CSV file, adding each line to a private array, ready to be searched against on the "Run" method of this class.
        /// </summary>
        /// <param name="path">The full path of the CSV containing the list of SKUs to search against.</param>
        public void ReadList(string path)
        {
            // Add each line in the list of SKUs to an array.
            _list = File.ReadAllLines(path);
        }

        /// <summary>
        /// Searches the 'copy from' directory for each SKU from the previously read CSV, and copies across all files bearing the name of that SKU.
        /// </summary>
        /// <param name="copyToDirectory">The full path of where the files will be copied to.</param>
        public void Run(string copyToDirectory)
        {
            foreach(var listItem in _list)
            {
                // Perform search.
                copyFile.Search(listItem.ToUpper());

                // If search returns true, copy across to new location.
                if(copyFile.SearchTermHasResults == true)
                    copyFile.CopyAcross(listItem, copyToDirectory);
            }
        }
    }
}

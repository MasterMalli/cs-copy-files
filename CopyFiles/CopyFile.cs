using System;
using System.Linq;
using System.IO;
using System.Windows.Forms;

namespace CopyFiles
{
    class CopyFile
    {
        private string _locationRoot;
        private string _firstAndSecond;
        private int _chosenFile;
        private char _first;
        private string[] _files;
        public string LogFileLocation { get; set; }
        public string Sku { get; set; }
        public string CopyFromLocation { get; set; }
        public bool SearchTermHasResults { get; private set; }

        public CopyFile(string logFileLocation, string chosenCopyFrom)
        {
            LogFileLocation = logFileLocation;
            _locationRoot = chosenCopyFrom;
        }

        /// <summary>
        /// Searches the 'copy from' location for the specified SKU, and updates a global boolean variable if there are results.
        /// </summary>
        /// <param name="sku"></param>
        public void Search(string sku)
        {
            Sku = sku.ToUpper();
            _first = Sku[0];
            _firstAndSecond = Sku.Substring(0, 2);
            CopyFromLocation = Path.Combine(_locationRoot + _first + "\\" + _firstAndSecond);
            string skuSubString = Sku.Substring(0, Sku.Length);
            try
            {
                _files = Directory.GetFiles(CopyFromLocation).Where(a => a.Contains(skuSubString, StringComparison.OrdinalIgnoreCase)).ToArray();

                if (_files.Length <= 0)
                {
                    SearchTermHasResults = false;
                    string noResults = $"Search returned no results for {Sku}";
                    Console.WriteLine(noResults);
                    File.AppendAllText(LogFileLocation, noResults + Environment.NewLine);
                }
                else
                {
                    SearchTermHasResults = true;
                    if (_files.Length == 1)
                    {
                        string fileFound = $"1 file found for {Sku}. ";
                        Console.WriteLine(fileFound);
                        File.AppendAllText(LogFileLocation, fileFound);
                    }
                    else
                    {
                        string fileFound = _files.Length + $" files found for {Sku}. ";
                        Console.WriteLine(fileFound);
                        File.AppendAllText(LogFileLocation, fileFound);
                    }
                }
            }
            catch (Exception)
            {
                SearchTermHasResults = false;
                string noResults = $"Search returned no results for {Sku}";
                Console.WriteLine(noResults);
                File.AppendAllText(LogFileLocation, noResults + Environment.NewLine);
            }
        }

        /// <summary>
        /// Copies each file found that includes the name of the SKU that was searched for - renaming them with a numerical suffix.
        /// "AutoChooseFile" method not implemented for this.
        /// </summary>
        /// <param name="fileName">The name of the SKU that will be copied to the new location.</param>
        /// <param name="copyToDir">The directory which all the files found containing the specified SKU name will be copied to.</param>
        public void CopyAcross(string fileName, string copyToDir = @"D:\Pictures\EPS\")
        {
            Console.WriteLine("Now Copying... ");
            File.AppendAllText(LogFileLocation, "Now Copying... ");
            try
            {
                int i = 1;
                foreach(var file in _files)
                {
                    string source = Path.Combine(CopyFromLocation + "\\" + Path.GetFileName(file));
                    string destination = Path.Combine(copyToDir + fileName + "_" + i + Path.GetExtension(file));
                    Directory.CreateDirectory(copyToDir);
                    File.Copy(source, destination, overwrite: true);
                    i++;
                }
                Console.WriteLine("Success." + Environment.NewLine + "----------");
                File.AppendAllText(LogFileLocation, "Success." + Environment.NewLine);
            }
            catch (Exception)
            {
                File.AppendAllText(LogFileLocation, "Error." + Environment.NewLine + $"Was not able to copy files for {fileName}." + Environment.NewLine);
                throw new InvalidOperationException($"Error. Was not able to copy files for {fileName}.");
            }
        }

        /// <summary>
        /// Where there is more than one file in the search results, and you only want to copy across 1 file, this method will auto choose which file to copy across.
        /// </summary>
        /// <param name="files"></param>
        public void AutoChooseFile(string[] files)
        {
            var autoChosen = false;
            int i = 0;

            // Check for filename just containing SKU and no suffix/prefix; if found auto choose this file.
            foreach (var file in files)
            {
                int fileLength = Path.GetFileNameWithoutExtension(file).Length;

                if (Sku.Length == fileLength)
                {
                    _chosenFile = i;
                    autoChosen = true;
                }
                else
                {
                    i++;
                }
            }
            // If the above yields no results, check to see if a file name has a suffix of '1', if there is, auto choose this file.
            if(autoChosen == false)
            {
                i = 0;
                foreach(var file in files)
                {
                    string filename = Path.GetFileNameWithoutExtension(file);
                    int fileLength = filename.Length;
                    int subCount = fileLength - Sku.Length;
                    string substring = filename.Substring(Sku.Length, subCount);
                    if (substring.Contains("1"))
                    {
                        _chosenFile = i;
                        autoChosen = true;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            // If the above yields no results, check to see if a file name contains the word 'main', if there is, auto choose this file.
            if (autoChosen == false)
            {
                i = 0;
                foreach (var file in files)
                {
                    string filename = Path.GetFileNameWithoutExtension(file);
                    int fileLength = filename.Length;
                    int subCount = fileLength - Sku.Length;
                    if (filename.Substring(Sku.Length, subCount).Contains("main"))
                    {
                        _chosenFile = i;
                        autoChosen = true;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            // If the above yields no results, check to see if a file name contains the word 'front', if there is, auto choose this file.
            if (autoChosen == false)
            {
                i = 0;
                foreach (var file in files)
                {
                    string filename = Path.GetFileNameWithoutExtension(file);
                    int fileLength = filename.Length;
                    int subCount = fileLength - Sku.Length;
                    if (filename.Substring(Sku.Length, subCount).Contains("front"))
                    {
                        _chosenFile = i;
                        autoChosen = true;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            // If the above yields no results, auto choose the first file.
            else if (autoChosen == false)
            {
                _chosenFile = 0;
            }

            string filesFound = files.Length + " files found. ";
            Console.WriteLine(filesFound);
            File.AppendAllText(LogFileLocation, filesFound);
        }

        /// <summary>
        /// Where there is more than one file in the search results, and you only want to copy across 1 file, this method will give you the choice of which file to copy across.
        /// </summary>
        /// <param name="files"></param>
        public void ChooseFile(string[] files)
        {
            int i = 0;
            Clipboard.SetText(Sku);
            string filesFound = files.Length + " files found.";
            Console.WriteLine(filesFound);
            string pleaseChoose = "Please choose a file to copy";
            Console.WriteLine(pleaseChoose);
            File.AppendAllText(LogFileLocation, filesFound + Environment.NewLine + pleaseChoose + Environment.NewLine);

            foreach (var file in files)
            {
                Console.WriteLine($"[{i}] : {Path.GetFileName(file)}");
                File.AppendAllText(LogFileLocation, $"[{i}] : {Path.GetFileName(file)}" + Environment.NewLine);
                i++;
            }
            for (int b = 0; b < 1;)
            {
                string chosenFile = Console.ReadLine();
                if (int.Parse(chosenFile) >= files.Length)
                {
                    b = 0;
                    Console.WriteLine("Invalid choice - please choose again.");
                }
                else
                {
                    int.TryParse(chosenFile, out _chosenFile);
                    b = 2;
                }
            }
        }

    }
    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }
    }
}

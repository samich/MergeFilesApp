using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergeFilesApp
{
    internal class ProcessFiles
    {

        //directory paths
        static string baseDir = "C:\\CombinedLetters";
        static string admissionDir = baseDir + "\\Input\\Admission";
        static string scholarshipDir = baseDir + "\\Input\\Scholarship";
        static string outputDir = baseDir + "\\Output";
        static string archiveDir = baseDir + "\\Archive";

        //list to store common file names
        ArrayList commonFiles = new ArrayList();

        /// <summary>
        /// Starting point of the processing. Checks the time first then checks for directories of past 7 days.
        /// Processes a directory if it needs to be processed.
        /// </summary>
        public void start()
        {
            //get the current hour
            int currentHour = Convert.ToInt32(DateTime.Now.ToString("HH"));

            //ask for confirmation if its not 10 AM.
            if (currentHour != 10)
            {
                Console.WriteLine("Application is scheduled to run at 10 AM. Are you sure you want to run at this time?");
                Console.WriteLine("Press y to continue or any other key to exit.");

                Console.WriteLine();
                char c = Console.ReadKey().KeyChar;
                Console.WriteLine();

                //exit program if any key other than y is entered
                if (c != 'y')
                {
                    Environment.Exit(0);
                }

            }

            //count how many file needs to be archived
            int archiveCount = 0;

            //check for past 7 days directories
            for (int i = 0; i > -6; i--)
            {

                //get directory name
                string dirName = getDirectoryName(i);

                //process the directory if it needs to be
                if (needsProcessing(dirName))
                {    
                    //method call to process the directory
                    processDirectory(dirName);
                    archiveCount++;
                }

            }

            //display message if there is no any unarchived directory
            if (archiveCount == 0) {

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No any unarchived directory found.");
                Console.ResetColor();

            }

        }


        /// <summary>
        /// Generates a directory name.
        /// </summary>
        /// <param name="offset">Difference from today, eg. -1 for yesterday.</param>
        /// <returns>directory name in yyyyMMdd format.</returns>
        public string getDirectoryName(int offset)
        {

            //get the day, month and year by adding offset number to it.
            //eg. if the offset is -1, get the date for yesterday.
            string month = DateTime.Today.AddDays(offset).ToString("MM");
            string day = DateTime.Today.AddDays(offset).ToString("dd");
            string year = DateTime.Today.AddDays(offset).ToString("yyyy");

            return year + month + day;
        }

        /// <summary>
        /// Checks if a directory needs to be processed by checking if it exists and has not been processed already.
        /// </summary>
        /// <param name="dirName">Directory name to check if it needs to be processed. </param>
        /// <returns>boolean value</returns>
        public bool needsProcessing(string dirName)
        {

            //directory name in admission directory
            string admissionDirPath = admissionDir + "\\" + dirName;

            //check if it exists.
            if (!Directory.Exists(admissionDirPath))
            {
                return false;
            }
            else
            {
                //if it exists, look for the archive-confirmation.txt file in that directory
                string archivedFilePath = admissionDirPath + "\\archive-confirmation.txt";

                //if that file exists, that means the directory has been archived already.
                if (File.Exists(archivedFilePath))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

        }

        /// <summary>
        /// Archives a directory and generates the report. Creates a confirmation file in the input directory.
        /// </summary>
        /// <param name="dirName">Directory name that needs to be archived.</param>
        public void processDirectory(String dirName)
        {

            //get instance of the LetterService class
            LetterService service = new LetterService();

            //paths for admission and scholarship directories
            string admissionDirPath = admissionDir + "\\" + dirName;
            string scholarshipDirPath = scholarshipDir + "\\" + dirName;

            //get all files in the admission directory
            string[] admissionLetters = Directory.GetFiles(admissionDirPath);

            //process each file
            foreach (string admissionLetter in admissionLetters)
            {

                //get file name
                string fileNameFull = Path.GetFileName(admissionLetter);

                //since the file name is admission-xxxxxxxx.txt, split the name string to get only the part after -.
                string fileName = fileNameFull.Split("-")[1];

                //get the file name without extension
                string fileNameWithoutExt = fileName.Split(".")[0];

                //file paths for admission and scholarship file
                string admissionFilePath = admissionDirPath + "\\" + fileNameFull;
                string scholarshipFilePath = scholarshipDirPath + "\\scholarship-" + fileName;

                //file paths for output and archive directory
                string resultFilePath = outputDir + "\\" + fileName;
                string archiveFilePath = archiveDir + "\\" + fileNameFull;

                //read all text of the admission file
                string text = File.ReadAllText(admissionFilePath);

                //write that text to the file with same name in archive directory.
                File.WriteAllText(archiveFilePath, text);

                //check if the scholarship file for the same student exists
                if (File.Exists(scholarshipFilePath))
                {
                    //scholarship file exists, combine it  
                    service.CombineTwoLetters(admissionFilePath, scholarshipFilePath, resultFilePath);
                    
                    //add the common files to the commonFiles ArrayList
                    commonFiles.Add(fileNameWithoutExt);

                }

            }

            //method call to generate archive.txt file
            generateReport(dirName, commonFiles);

            //method call to generate archive-confirmation.txt file 
            createArchiveConfirmationFile(dirName);

        }

        /// <summary>
        /// Generates report of archived files.
        /// Creates archive.txt file in the output directory.
        /// </summary>
        /// <param name="dirName">Directory name which is archived. </param>
        /// <param name="list">Arraylist that stores list of files that have been archived.</param>
        public void generateReport(String dirName, ArrayList list)
        {

            //path for archive.txt file
            string archiveFilePath = outputDir + "\\archive.txt";

            //extract year, month and day from the directory name
            string year = dirName.Substring(0, 4);
            string month = dirName.Substring(4, 2);
            string day = dirName.Substring(6, 2);

            //write the header text on archive.txt file
            File.WriteAllText(archiveFilePath, month + "/" + day + "/" + year + " Report");
            File.AppendAllText(archiveFilePath, "\n-----------------------\n");
            File.AppendAllText(archiveFilePath, "Number of Combined Letters: " + commonFiles.Count + "\n");

            //get file names from the list and write on the archive.txt file
            foreach (string fileName in list)
            {
                File.AppendAllText(archiveFilePath, "\t" + fileName + "\n");
            }

            //write confirmation message on Console by setting text color.
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Directory " + dirName + " archived.");
            Console.ResetColor();

        }

        /// <summary>
        /// Prints contents of the archive.txt file in console.
        /// </summary>
        public void printArchiveFile()
        {

            //path for the archive file
            string archiveFilePath = outputDir + "\\archive.txt";

            //if the file exists, print its contents on console
            if (File.Exists(archiveFilePath))
            {
                StreamReader reader = new StreamReader(archiveFilePath);
                string text = reader.ReadToEnd();
                Console.WriteLine(text);
            }
            else
            {
                //print error message if it does not exist.
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: No such file. Perhaps the directory is not archived."); 
                Console.ResetColor();
            }

        }

        /// <summary>
        /// Creates a text file in the input directory with timestamp.
        /// The program will look for this file before processing.
        /// </summary>
        /// <param name="dirName">Input directory name where the text file will be generated.</param>
        public void createArchiveConfirmationFile(String dirName)
        {
            //path for the archive-confirmation.txt file
            string archiveConfirmationFilePath = admissionDir + "\\" + dirName + "\\archive-confirmation.txt";

            //get current date and time
            string date = DateTime.Now.ToString();

            //write it on the file
            File.WriteAllText(archiveConfirmationFilePath, "Directory archived on:\n" + date);

        }

    }
        
}

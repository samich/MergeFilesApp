using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergeFilesApp
{
    public class LetterService : ILetterService
    {
        public void CombineTwoLetters(string inputFile1, string inputFile2, string resultFile)
        {

            //read text from the inputFile1
            string text = File.ReadAllText(inputFile1);

            //write the inputFile1 text to the resultfile
            File.WriteAllText(resultFile, text);

            //append a separator line to the resultFile
            File.AppendAllText(resultFile, "\n--------------\n");

            //read the content of inputFile2 
            text = File.ReadAllText(inputFile2);

            //append the content inputFile2 content to the resultFile
            File.AppendAllText(resultFile, text);

        }
    }
}

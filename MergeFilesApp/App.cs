using System;

namespace MergeFilesApp 
{
    public class App 
    {
        /// <summary>
        /// Entry point of the program
        /// </summary>
        static void Main(String[] args) 
        {
            
            //get instance of the class
            App program = new App();

            //call methods to print menu and process the user selection
            program.printMenu();
            program.switchChoice();
        }

        /// <summary>
        /// Prints the menu on program start.
        /// </summary>
        public void printMenu() 
        {

            Console.WriteLine("Select an option from the menu ( 1 - 2 )");
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("1. Archive files");
            Console.WriteLine("2. Read Archive file");
        }

        /// <summary>
        /// Reads user input for the menu choice.
        /// </summary>
        /// <returns>int value</returns>
        public int getUserChoice()
        {

            int choice = -1;

            //continue the loop if the user enters an invalid choice
            while (choice < 1 || choice > 2)
            {

                try
                {
                    Console.WriteLine("Please enter your choice (1 - 2):");

                    //read the input and convert it to the int
                    choice = Convert.ToInt32(Console.ReadLine());                    

                }
                catch (Exception e)
                {
                    //display exception message
                    Console.WriteLine(e.Message);
                }

            }

            return choice;
        }

        /// <summary>
        /// Processes the user choice. Starts the processing or prints archive file.
        /// </summary>
        public void switchChoice()
        {

            int choice = getUserChoice();

            switch (choice)
            {
                case 1:
                    //method call to start processing
                    new ProcessFiles().start();
                    break;
                case 2:
                    //method call to print archive file 
                    new ProcessFiles().printArchiveFile();
                    break;
            }

        }

    }
}
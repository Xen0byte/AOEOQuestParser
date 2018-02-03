using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace AOEOQuestParser
{
    public class Logic
    {
        // Gets the source folder location from user input. This is where the source *quest files are read from.
        public static string GetSourceFolderLocation()
        {
            string questLocation;

            Console.WriteLine("Please type or paste in the path of the folder where your *quest files are located!");
            questLocation = Console.ReadLine();
            Console.WriteLine();

            return questLocation;
        }

        // Gets the destination folder location from user input. This is where the processed *quest files will be stored.
        public static string SetDestinationFolderLocation()
        {
            string questDestination = "C:\\";
            string customDestinationCheck;

            string[] yesSelections = { "y", "Y", "yes", "Yes", "YES" };
            string[] noSelections = { "n", "N", "no", "No", "NO" };

            Console.WriteLine("The default destination folder is \"C:\\\". Would you like to change the destination folder?");
            Console.WriteLine("Please press [Y]es or [N]o to continue.");
            customDestinationCheck = Console.ReadKey(true).KeyChar.ToString();
            Console.WriteLine();

            while (!Array.Exists(yesSelections, element => element == customDestinationCheck) && !Array.Exists(noSelections, element => element == customDestinationCheck))
            {
                Console.WriteLine("Invalid selection. Please press [Y]es or [N]o to continue.");
                customDestinationCheck = Console.ReadKey(true).KeyChar.ToString();
                Console.WriteLine();
            }

            if (Array.Exists(yesSelections, element => element == customDestinationCheck))
            {
                Console.WriteLine("Please type or paste in the path of the folder where your would like your parsed *quest files to be saved!");
                questDestination = Console.ReadLine();
                Console.WriteLine();
                Console.WriteLine("The custom destination folder you have selected is \"" + questDestination + "\".");
            }
            else if (Array.Exists(noSelections, element => element == customDestinationCheck))
            {
                Console.WriteLine("You have not set a custom destination folder. Your destination folder will be \"C:\\\"." + "\n");
            }

            return questDestination;
        }

        // Builds an array of all the *quest files found in the source folder and all its subdirectories.
        public static string[] GetAllFilesForProcessing(string questPath)
        {
            string[] questFiles = new string[0];
            int questFileCounter = 0;

            try
            {
                questFiles = Directory.GetFiles(questPath, "*.quest", SearchOption.AllDirectories);
            }
            catch (Exception e)
            {
                Console.WriteLine(e + "\n");
                Console.WriteLine("\"" + questPath + "\"" + " is an invalid path. Press ENTER to exit." + "\n");
            }

            foreach (string i in questFiles)
            {
                Console.WriteLine("{0}", i);
                questFileCounter++;
            }

            Console.WriteLine("\n" + questFileCounter + " files found." + "\n");

            return questFiles;
        }

        // Builds an array of all the relative paths to the *quest files found in the source folder and all its subdirectories.
        // This array will be used to save the processed files in the destination folder.
        public static string[] GetRelativePaths(string[] questArray, string questLocation)
        {
            string[] relativePaths = new string[0];
            List<string> relativePathsList = new List<string>();
            int questFileCounter = 0;

            foreach (string i in questArray)
            {
                relativePathsList.Add(questArray[questFileCounter].Remove(0, questLocation.Length + 1));
                questFileCounter++;
            }

            Console.WriteLine(questFileCounter + " relative paths stored." + "\n");

            relativePaths = relativePathsList.ToArray();

            return relativePaths;
        }

        // This is a debug method to get all the unique elements with descendants.
        // In the processed *quest file, elements with no descendants will become attributes.
        public static List<string> GetElementsWithDescendants(string[] questArray)
        {
            List<string> elementsWithDescendants = new List<string>();

            foreach (string i in questArray)
            {
                XDocument questFile = XDocument.Load(i);

                foreach (XElement element in questFile.Descendants())
                {
                    if (element.Descendants().Count() > 0)
                    {
                        if (!elementsWithDescendants.Contains(element.Name.ToString()))
                        {
                            elementsWithDescendants.Add(element.Name.ToString());
                        }
                    }
                }
            }

            elementsWithDescendants.Sort();

            foreach (string elementWithDescendants in elementsWithDescendants)
            {
                Console.WriteLine(elementWithDescendants);
            }

            Console.WriteLine();

            return elementsWithDescendants;
        }

        // Converts all the *quest files found in the source folder to a format compatible with the FireSinging AOEO open-source server.
        // This is where the magic happens.
        public static void ProcessQuestFiles(string[] questArray, string[] relativePaths)
        {
            //the bulk of the code goes here
        }

        // Writes the processed *quest files to the destination folder using the files' relative paths.
        public static void WriteFiles(string questDestination)
        {
            //write *quest files to new format
        }
    }
}
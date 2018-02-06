using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace AOEOQuestParser
{
    public class Logic
    {
        // Asks the user to run the application in Debug Mode.
        // The debug methods will execute before a conditional breakpoint on the quest parsing algorithm.
        public static bool DebugMode()
        {
            bool debugMode = false;
            string customDestinationCheck;

            string[] yesSelections = { "y", "Y", "yes", "Yes", "YES" };
            string[] noSelections = { "n", "N", "no", "No", "NO" };

            Console.WriteLine("Would you like to start the application in Debug Mode?" + "\n");
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
                debugMode = true;
            }

            return debugMode;
        }

        // Gets the source folder location from user input. This is where the source *quest files are read from.
        public static string GetSourceFolderLocation()
        {
            string questLocation;

            Console.WriteLine("Please drag-and-drop onto this window the folder where your *quest files are located!");
            questLocation = Console.ReadLine();
            Console.WriteLine();

            return questLocation;
        }

        // Gets the destination folder location from user input. This is where the processed *quest files will be stored.
        public static string SetDestinationFolderLocation(bool debugMode)
        {
            string questDestination = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToString();

            if (!debugMode)
            {
                string customDestinationCheck;

                string[] yesSelections = { "y", "Y", "yes", "Yes", "YES" };
                string[] noSelections = { "n", "N", "no", "No", "NO" };

                Console.WriteLine("The default destination folder is the Documents folder.");
                Console.WriteLine("Would you like to change the destination folder?" + "\n");
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
                    Console.WriteLine("You have not set a custom destination folder.");
                    Console.WriteLine("Your destination folder will be the Documents folder." + "\n");
                }

                questDestination += "\\.parsed-quest-files";
                Directory.CreateDirectory(questDestination);
            }

            return questDestination;
        }

        // Sets the temporary file to which partially parsed data will be written.
        public static string SetTempFile(string questDestination)
        {
            string tempFile = questDestination + "\\temp.xml";
            return tempFile;
        }

        // Builds an array of all the *quest files found in the source folder and all its subdirectories.
        public static string[] GetAllFilesForProcessing(string questPath)
        {
            List<string> questFiles = new List<string>();
            int questFileCounter = 0;

            try
            {
                foreach (string questFile in Directory.GetFiles(questPath, "*.quest", SearchOption.AllDirectories))
                {
                    questFiles.Add(questFile);
                }
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

            return questFiles.ToArray();
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
                relativePathsList.Add(questArray[questFileCounter].Remove(0, questLocation.Length));
                questFileCounter++;
            }

            relativePaths = relativePathsList.ToArray();

            return relativePaths;
        }

        #region Debug Methods
        // This is a debug method to get all the unique elements with descendants, for building the Parser class.
        // In the processed *quest file, elements with no descendants will become attributes.
        public static List<string> GetElementsWithDescendants(string[] questArray)
        {
            List<string> elementsWithDescendants = new List<string>();

            foreach (string i in questArray)
            {
                XDocument questFile = XDocument.Load(i);

                foreach (XElement element in questFile.Descendants())
                {
                    if (element.Name.ToString() != "quest" && element.Descendants().Count() > 0 && element.Parent.Name.ToString() == "quest")
                    {
                        if (!elementsWithDescendants.Contains(element.Name.ToString()))
                        {
                            elementsWithDescendants.Add(element.Name.ToString());
                        }
                    }
                }
            }

            elementsWithDescendants.Sort();

            Console.WriteLine("[DEBUG] Elements With Descendants:");
            Console.WriteLine("----------------------------------");

            foreach (string elementWithDescendants in elementsWithDescendants)
            {
                Console.WriteLine(elementWithDescendants);
            }

            Console.WriteLine();

            return elementsWithDescendants;
        }

        // This is a debug method to get all instances of a single element among all quest files.
        // This method will be used to determine the maximum depth.
        // This method helps with making a judgement call on what the most efficient way would be to parse the element to the new format.
        public static List<string> GetAllInstancesOfElement(string[] questArray)
        {
            List<string> allInstancesOfElement = new List<string>();
            XElement mostComplexElement = new XElement("null");
            int highestComplexity = 0;
            string fileFound = "";

            Console.WriteLine("This method gets all instances among all quest files of the specified XML element.");
            Console.WriteLine("Type in an element name and press Enter to continue, or leave blank and press Enter to skip.");
            string elementSelected = Console.ReadLine();
            Console.WriteLine();

            foreach (string i in questArray)
            {
                XDocument questFile = XDocument.Load(i);

                foreach (XElement element in questFile.Descendants())
                {
                    if (element.Name.ToString() == elementSelected)
                    {
                        if (!allInstancesOfElement.Contains(element.ToString()))
                        {
                            allInstancesOfElement.Add(element.ToString());

                            if (element.Descendants().Count() > highestComplexity)
                            {
                                highestComplexity = element.Descendants().Count();
                                mostComplexElement = element;
                                fileFound = i;
                            }
                        }
                    }
                }
            }

            if (allInstancesOfElement.Count() > 0)
            {
                Console.WriteLine("[DEBUG] All Instances Of Element: " + elementSelected);
                Console.WriteLine("---------------------------------");

                foreach (string instanceOfElement in allInstancesOfElement)
                {
                    Console.WriteLine(instanceOfElement + "\n");
                }

                Console.WriteLine("[DEBUG] Element With Highest Complexity Of Type: " + elementSelected);
                Console.WriteLine("------------------------------------------------");
                Console.WriteLine(mostComplexElement.ToString() + "\n");
                Console.WriteLine("Found in file: " + fileFound);
            }
            else
            {
                if (elementSelected.Length == 0)
                {
                    Console.WriteLine("[DEBUG] Step Skipped or Element Not Found");
                }
                else if (elementSelected.Length > 0)
                {
                    Console.WriteLine("\n" + "[DEBUG] Step Skipped or Element Not Found");
                }
            }

            return allInstancesOfElement;
        }
        #endregion

        // Converts all the *quest files found in the source folder to a format compatible with the FireSinging AOEO open-source server.
        // This is where the magic happens.
        public static void ProcessQuestFiles(string[] questArray, string questDestination, string[] relativePaths, string tempFile)
        {
            Parser.WriteQuestToFile(questArray, questDestination, relativePaths, tempFile);
        }

        // Creates a temporary XML file to write the parsed data to.
        public static void WriteTempFile(string tempFile)
        {
            XDocument tempXDoc = new XDocument();
            tempXDoc.Add(new XElement("quest"));
            tempXDoc.Save(tempFile);
        }

        // Deletes the temporary XML file, after the parsed data has been copied to a *quest file.
        public static void EraseTempFile(string tempFile)
        {
            File.Delete(tempFile);
        }

        // Writes the processed *quest files to the destination folder using the files' relative paths.
        public static void WriteFiles(string questDestination, string relativePath, string tempFile)
        {
            (new FileInfo(questDestination + relativePath)).Directory.Create();
            File.WriteAllLines(questDestination + relativePath, File.ReadAllLines(tempFile).Skip(1).ToArray());
        }
    }
}
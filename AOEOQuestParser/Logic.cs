using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOEOQuestParser
{
    public class Logic
    {
        public static string GetSourceFolderLocation()
        {
            string questLocation;

            Console.WriteLine("Please type in the path of the folder where your *quest files are located!");
            questLocation = Console.ReadLine();
            Console.WriteLine();

            return questLocation;
        }

        public static string SetDestinationFolderLocation()
        {
            string questDestination = "C:\\";
            string customDestinationCheck;

            string[] yesSelections = { "y", "Y", "yes", "Yes", "YES" };
            string[] noSelections = { "n", "N", "no", "No", "NO" };

            Console.WriteLine("The default destination folder is \"C:\\\". Would you like to change the destination folder?");
            Console.WriteLine("Please press [Y]es or [N]o to continue.");
            customDestinationCheck = Console.ReadLine();
            Console.WriteLine();

            while (!Array.Exists(yesSelections, element => element == customDestinationCheck) && !Array.Exists(noSelections, element => element == customDestinationCheck))
            {
                Console.WriteLine("Invalid selection. Please press [Y]es or [N]o to continue.");
                customDestinationCheck = Console.ReadLine();
                Console.WriteLine();
            }

            if (Array.Exists(yesSelections, element => element == customDestinationCheck))
            {
                Console.WriteLine("Please type in the path of the folder where your would like your parsed *quest files to be saved!");
                questDestination = Console.ReadLine();
                Console.WriteLine();
                Console.WriteLine("The custom destination folder you have selected is \"" + questDestination + "\".");
            }
            else if (Array.Exists(noSelections, element => element == customDestinationCheck))
            {
                Console.WriteLine("You have not set a custom destination folder. Your destination folder will be \"C:\\\".");
            }

            return questDestination;
        }
    }
}
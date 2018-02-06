using System;

namespace AOEOQuestParser
{
    class Program
    {
        static void Main(string[] args)
        {
            bool debugMode = Logic.DebugMode();

            string questLocation = Logic.GetSourceFolderLocation();

            string questDestination = Logic.SetDestinationFolderLocation(debugMode);

            string tempFile = Logic.SetTempFile(questDestination);

            string[] questFiles = Logic.GetAllFilesForProcessing(questLocation);

            string[] relativePaths = Logic.GetRelativePaths(questFiles, questLocation);

            #region Debug Methods
            if (debugMode)
            {
                Logic.GetElementsWithDescendants(questFiles);
                Logic.GetAllInstancesOfElement(questFiles);
            }
            #endregion

            if (!debugMode)
            {
                Logic.ProcessQuestFiles(questFiles, questDestination, relativePaths, tempFile);
            }

            Console.WriteLine("\n" + "Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
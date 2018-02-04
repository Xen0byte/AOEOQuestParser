using System;

namespace AOEOQuestParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string questLocation = Logic.GetSourceFolderLocation();

            string questDestination = Logic.SetDestinationFolderLocation();

            string tempFile = Logic.SetTempFile(questDestination);

            string[] questFiles = Logic.GetAllFilesForProcessing(questLocation);

            string[] relativePaths = Logic.GetRelativePaths(questFiles, questLocation);

            #region Debug Method To Get Unique Elements With Descendants
            // Logic.GetElementsWithDescendants(questFiles);
            #endregion

            Logic.ProcessQuestFiles(questFiles, questDestination, relativePaths, tempFile);

            Console.WriteLine("\n" + "Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
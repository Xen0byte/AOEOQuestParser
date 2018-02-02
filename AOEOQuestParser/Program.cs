using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOEOQuestParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string questLocation = Logic.GetSourceFolderLocation();

            string questDestination = Logic.SetDestinationFolderLocation();

            Logic.GetAllFilesForProcessing(questLocation);

            Console.ReadLine();
        }
    }
}
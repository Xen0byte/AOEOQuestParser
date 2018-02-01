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
            Logic.GetSourceFolderLocation();

            Logic.SetDestinationFolderLocation();

            Console.ReadLine();
        }
    }
}
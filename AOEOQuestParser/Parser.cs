using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AOEOQuestParser
{
    class Parser
    {
        //1. Processes elements from the source file individually or in groups of similar elements.
        //2. Adds the outputs one by one to instances of a temporary XML file.
        //3. Builds the final parsed *quest files from the temporary XML file instances.
        public static void WriteQuestToFile(string[] questArray, string questDestination, string[] relativePaths, string tempFile)
        {
            int processedFilesCounter = 0;

            foreach (string currentQuestFile in questArray)
            {
                Logic.WriteTempFile(tempFile);

                quest(currentQuestFile, tempFile);
                nodescendants(currentQuestFile, tempFile);

                Logic.WriteFiles(questDestination, relativePaths[processedFilesCounter], tempFile);
                Logic.EraseTempFile(tempFile);

                processedFilesCounter++;
                Console.Write($"\rprocessed: {processedFilesCounter} out of {questArray.Length} quest files ({(Convert.ToSingle(processedFilesCounter) / Convert.ToSingle(questArray.Length) * 100):0.00}%)");
            }
        }

        // Writes the quest element to the temporary XML file. This is the root element.
        public static void quest(string currentQuestFile, string tempFile)
        {
            XDocument questFileInstance = XDocument.Load(currentQuestFile);
            XElement questElement = questFileInstance.Descendants().First();
            XNamespace nameSpace = "http://www.w3.org/2000/";

            questElement.Attribute("{" + nameSpace + "xmlns/}" + "xsi").Remove();
            questElement.Attribute("{" + nameSpace + "xmlns/}" + "xsd").Remove();
            questElement.RemoveNodes();
            questElement.Save(tempFile);
        }

        // Writes all direct descendants with no child elements of the root element from the source file as attributes of the root element from the temporary file.
        // Elements with no value are ignored.
        public static void nodescendants(string currentQuestFile, string tempFile)
        {
            XDocument questFileInstance = XDocument.Load(currentQuestFile);
            List<KeyValuePair<string, string>> descendantsList = new List<KeyValuePair<string, string>>();

            foreach (XElement element in questFileInstance.Descendants())
            {
                if (element.Descendants().Count() == 0 && element.AncestorsAndSelf().Count() == 2 && element.Value != "")
                {
                    descendantsList.Add(new KeyValuePair<string, string>(element.Name.ToString(), element.Value.ToString()));
                }
            }

            XDocument tempXDocInstance = XDocument.Load(tempFile);
            XElement questElement = tempXDocInstance.Descendants().First();

            foreach (KeyValuePair<string, string> newAttribute in descendantsList)
            {
                questElement.Add(new XAttribute(newAttribute.Key, newAttribute.Value));
            }

            tempXDocInstance.Save(tempFile);
        }

        public static void aiflagvariables()
        {

        }

        public static void aislidervariables()
        {

        }

        public static void aivariable()
        {

        }

        public static void alliancepoints()
        {

        }

        public static void and()
        {

        }

        public static void buildunit()
        {

        }

        public static void capitalresource()
        {

        }

        public static void capitaltech()
        {

        }

        public static void civilization()
        {

        }

        public static void collectmaterial()
        {

        }

        public static void collectresource()
        {

        }

        public static void completegame()
        {

        }

        public static void consumable()
        {

        }

        public static void consumematerial()
        {

        }

        public static void convertunit()
        {

        }

        public static void counter()
        {

        }

        public static void diplomacysettings()
        {

        }

        public static void dummy()
        {

        }

        public static void events()
        {

        }

        public static void gamecurrency()
        {

        }

        public static void grouping()
        {

        }

        public static void kill()
        {

        }

        public static void level()
        {

        }

        public static void mapvariables()
        {

        }

        public static void material()
        {

        }

        public static void nuggetoverrides()
        {

        }

        public static void objectives()
        {

        }

        public static void onaccept()
        {

        }

        public static void or()
        {

        }

        public static void overrides()
        {

        }

        public static void ownsequipment()
        {

        }

        public static void ownsunit()
        {

        }

        public static void playersettings()
        {

        }

        public static void population()
        {

        }

        public static void prereqs()
        {

        }

        public static void protectunit()
        {

        }

        public static void protounit()
        {

        }

        public static void questcomplete()
        {

        }

        public static void questgiver()
        {

        }

        public static void questgivers()
        {

        }

        public static void questreturners()
        {

        }

        public static void queststatus()
        {

        }

        public static void random()
        {

        }

        public static void randommap()
        {

        }

        public static void reduceunitsto()
        {

        }

        public static void repairunit()
        {

        }

        public static void rewards()
        {

        }

        public static void secondaryobjectives()
        {

        }

        public static void secondaryrewards()
        {

        }

        public static void spawngroup()
        {

        }

        public static void spawnunit()
        {

        }

        public static void targets()
        {

        }

        public static void techstatus()
        {

        }

        public static void timer()
        {

        }

        public static void timereffects()
        {

        }

        public static void trait()
        {

        }

        public static void tribute()
        {

        }

        public static void unitdiscovered()
        {

        }

        public static void unitinarea()
        {

        }

        public static void unitnearunit()
        {

        }

        public static void unitprobability()
        {

        }

        public static void values()
        {

        }

        public static void victoryconditions()
        {

        }

        public static void wingame()
        {

        }

        public static void xp()
        {

        }
    }
}
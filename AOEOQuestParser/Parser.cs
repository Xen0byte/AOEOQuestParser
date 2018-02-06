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
                playersettings(currentQuestFile, tempFile);

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

        public static void diplomacysettings()
        {

        }

        public static void objectives()
        {

        }

        public static void onaccept()
        {

        }

        // Writes the aiflagvariables element and all it's descendants as a direct child of the root element.
        public static void playersettings(string currentQuestFile, string tempFile)
        {
            XDocument questFileInstance = XDocument.Load(currentQuestFile);
            List<KeyValuePair<string, string>> aivariables = new List<KeyValuePair<string, string>>();

            foreach (XElement element in questFileInstance.Descendants())
            {
                if (element.Name.ToString() == "aivariable")
                {
                    if (element.Parent.Name.ToString() == "aiflagvariables")
                    {
                        if (element.Descendants("key").Count() > 0 && element.Descendants("value").Count() > 0)
                        {
                            aivariables.Add(new KeyValuePair<string, string>(element.Descendants("key").First().Value, element.Descendants("value").First().Value));
                        }
                        else if (element.Descendants("key").Count() == 0 && element.Descendants("value").Count() > 0)
                        {
                            aivariables.Add(new KeyValuePair<string, string>("", element.Descendants("value").First().Value));
                        }
                        else if (element.Descendants("key").Count() > 0 && element.Descendants("value").Count() == 0)
                        {
                            aivariables.Add(new KeyValuePair<string, string>(element.Descendants("key").First().Value, ""));
                        }
                    }
                }
            } ////////////////////////////////////////////////////////// FIXXXXXXXXXX MEEEEEEEEEEEEEEEEE !!!!!!!! C01_M32_Sicyon_ProtectHall.quest

            XDocument tempXDocInstance = XDocument.Load(tempFile);
            XElement aiflagvariablesElement = tempXDocInstance.Descendants("aiflagvariables").First();

            foreach (KeyValuePair<string, string> aivariable in aivariables)
            {
                aiflagvariablesElement.Add(new XElement("aivariable", new XAttribute("key", aivariable.Key), new XAttribute("value", aivariable.Value)));

                foreach (XAttribute attribute in aiflagvariablesElement.Attributes())
                {
                    if (attribute.Value == "")
                    {
                        aiflagvariablesElement.Attribute(attribute.Name).Remove();
                    }
                }
            }

            tempXDocInstance.Save(tempFile);
        }

        public static void prereqs()
        {

        }

        public static void questgivers()
        {

        }

        public static void questreturners()
        {

        }

        public static void randommap()
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

        public static void targets()
        {

        }

        public static void timer()
        {

        }

        public static void timereffects()
        {

        }

        public static void victoryconditions()
        {

        }
    }
}
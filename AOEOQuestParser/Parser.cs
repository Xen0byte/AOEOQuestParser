using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AOEOQuestParser
{
    class Parser
    {
        static int nodescendantsCounter = 0;
        static int playersettingsCounter = 0;

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

            Console.WriteLine("\n" + nodescendantsCounter + " nodescendants | " + playersettingsCounter + " playersettings");
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
                nodescendantsCounter++;
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

        // Writes the playersettings element and all it's descendants as a direct child of the root element.
        public static void playersettings(string currentQuestFile, string tempFile)
        {
            XDocument questFileInstance = XDocument.Load(currentQuestFile);

            List<KeyValuePair<string, string>> aivariablesFlag = new List<KeyValuePair<string, string>>();
            List<KeyValuePair<string, string>> aivariablesSlider = new List<KeyValuePair<string, string>>();
            List<KeyValuePair<string, string>> noDescendants = new List<KeyValuePair<string, string>>();

            List<XElement> playersettingsElements = new List<XElement>();
            List<XElement> elementsToWrite = new List<XElement>();

            foreach (XElement element in questFileInstance.Descendants())
            {
                if (element.Name.ToString() == "playersettings" && element.Parent.Name.ToString() == "quest")
                {
                    playersettingsElements.Add(element);
                }
            }

            foreach (XElement element in playersettingsElements)
            {
                foreach (XElement subElement in element.Descendants())
                {
                    if (subElement.Name.ToString() == "aivariable")
                    {
                        if (subElement.Parent.Name.ToString() == "aiflagvariables")
                        {
                            if (subElement.Descendants("key").Count() > 0 && subElement.Descendants("value").Count() > 0)
                            {
                                aivariablesFlag.Add(new KeyValuePair<string, string>(subElement.Descendants("key").First().Value, subElement.Descendants("value").First().Value));
                            }
                            else if (subElement.Descendants("key").Count() == 0 && subElement.Descendants("value").Count() > 0)
                            {
                                aivariablesFlag.Add(new KeyValuePair<string, string>("", subElement.Descendants("value").First().Value));
                            }
                            else if (subElement.Descendants("key").Count() > 0 && subElement.Descendants("value").Count() == 0)
                            {
                                aivariablesFlag.Add(new KeyValuePair<string, string>(subElement.Descendants("key").First().Value, ""));
                            }
                        }

                        if (subElement.Parent.Name.ToString() == "aislidervariables")
                        {
                            if (subElement.Descendants("key").Count() > 0 && subElement.Descendants("value").Count() > 0)
                            {
                                aivariablesSlider.Add(new KeyValuePair<string, string>(subElement.Descendants("key").First().Value, subElement.Descendants("value").First().Value));
                            }
                            else if (subElement.Descendants("key").Count() == 0 && subElement.Descendants("value").Count() > 0)
                            {
                                aivariablesSlider.Add(new KeyValuePair<string, string>("", subElement.Descendants("value").First().Value));
                            }
                            else if (subElement.Descendants("key").Count() > 0 && subElement.Descendants("value").Count() == 0)
                            {
                                aivariablesSlider.Add(new KeyValuePair<string, string>(subElement.Descendants("key").First().Value, ""));
                            }
                        }
                    }

                    if (subElement.Parent.Name.ToString() == "playersettings" && subElement.Descendants().Count() == 0 && subElement.Value != "")
                    {
                        noDescendants.Add(new KeyValuePair<string, string>(subElement.Name.ToString(), subElement.Value.ToString()));
                    }
                    else if (subElement.Parent.Name.ToString() == "playersettings" &&
                        subElement.Descendants().Count() > 0 &&
                        subElement.Name.ToString() != "aiflagvariables" && subElement.Name.ToString() != "aislidervariables")
                    {
                        Console.WriteLine("\n" + "[ERROR] The playersettings\\" + subElement.Name.ToString() + "element has not been fully processed." + "\n");
                    }
                }

                element.RemoveNodes();

                if (aivariablesFlag.Count() > 0)
                {
                    element.Add(new XElement("aiflagvariables"));
                }

                if (aivariablesSlider.Count() > 0)
                {
                    element.Add(new XElement("aislidervariables"));
                }

                if (noDescendants.Count() > 0)
                {
                    foreach (KeyValuePair<string, string> newAttribute in noDescendants)
                    {
                        element.Add(new XAttribute(newAttribute.Key, newAttribute.Value));
                    }
                }

                foreach (KeyValuePair<string, string> aivariable in aivariablesFlag)
                {
                    element.Descendants("aiflagvariables").First().Add(new XElement("aivariable", new XAttribute("key", aivariable.Key), new XAttribute("value", aivariable.Value)));

                    foreach (XAttribute attribute in element.Attributes())
                    {
                        if (attribute.Value == "")
                        {
                            element.Attribute(attribute.Name).Remove();
                        }
                    }
                }

                foreach (KeyValuePair<string, string> aivariable in aivariablesSlider)
                {
                    element.Descendants("aislidervariables").First().Add(new XElement("aivariable", new XAttribute("key", aivariable.Key), new XAttribute("value", aivariable.Value)));

                    foreach (XAttribute attribute in element.Attributes())
                    {
                        if (attribute.Value == "")
                        {
                            element.Attribute(attribute.Name).Remove();
                        }
                    }
                }

                elementsToWrite.Add(element);
                aivariablesFlag = new List<KeyValuePair<string, string>>();
                aivariablesSlider = new List<KeyValuePair<string, string>>();
                noDescendants = new List<KeyValuePair<string, string>>();
            }

            XDocument tempXDocInstance = XDocument.Load(tempFile);
            XElement questElement = tempXDocInstance.Descendants().First();

            foreach (XElement parsedElement in elementsToWrite)
            {
                questElement.Add(parsedElement);
                playersettingsCounter++;
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
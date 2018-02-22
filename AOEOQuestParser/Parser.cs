using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AOEOQuestParser
{
    class Parser
    {
        #region Static Counter Variables
        static int nodescendantsCounter = 0;
        static int playersettingsCounter = 0;
        static int timersCounter = 0;
        static int timereffectsCounter = 0;
        static int victoryconditionsCounter = 0;
        static int randommapCounter = 0;
        static int onacceptCounter = 0;
        static int questgiverCounter = 0;
        static int targetCounter = 0;
        static int questreturnerCounter = 0;
        static int prereqCounter = 0;
        static int rewardCounter = 0;
        static int secondaryrewardCounter = 0;
        static int diplomacysettingsCounter = 0;
        static int objectiveCounter = 0;
        static int secondaryobjectiveCounter = 0;
        #endregion

        //1. Processes elements from the source file individually or in groups of similar elements.
        //2. Adds the outputs one by one to instances of a temporary XML file.
        //3. Builds the final parsed *quest files from the temporary XML file instances.
        public static void WriteQuestToFile(string[] questArray, string questDestination, string[] relativePaths, string tempFile)
        {
            int processedFilesCounter = 0;

            foreach (string currentQuestFile in questArray)
            {
                Logic.WriteTempFile(tempFile);

                #region Methods To Add Quest Element And All Direct Descendants
                quest(currentQuestFile, tempFile);
                nodescendants(currentQuestFile, tempFile);
                playersettings(currentQuestFile, tempFile);
                timer(currentQuestFile, tempFile);
                timereffects(currentQuestFile, tempFile);
                victoryconditions(currentQuestFile, tempFile);
                randommap(currentQuestFile, tempFile);
                onaccept(currentQuestFile, tempFile);
                questgivers(currentQuestFile, tempFile);
                targets(currentQuestFile, tempFile);
                questreturners(currentQuestFile, tempFile);
                prereqs(currentQuestFile, tempFile);
                rewards(currentQuestFile, tempFile);
                secondaryrewards(currentQuestFile, tempFile);
                diplomacysettings(currentQuestFile, tempFile);
                objectives(currentQuestFile, tempFile);
                secondaryobjectives(currentQuestFile, tempFile);
                #endregion

                Logic.WriteFiles(questDestination, relativePaths[processedFilesCounter], tempFile);
                Logic.EraseTempFile(tempFile);

                processedFilesCounter++;
                Console.Write($"\rprocessed: {processedFilesCounter} out of {questArray.Length} quest files ({(Convert.ToSingle(processedFilesCounter) / Convert.ToSingle(questArray.Length) * 100):0.00}%)");
            }

            Console.WriteLine("\n" + "\n" + nodescendantsCounter + " nodescendants | " + playersettingsCounter + " playersettings | " + timersCounter + " timers | " + timereffectsCounter + " timereffects");
            Console.WriteLine(victoryconditionsCounter + " victoryconditions | " + randommapCounter + " randommaps | " + onacceptCounter + " onaccepts | " + questgiverCounter + " questgivers");
            Console.WriteLine(targetCounter + " targets | " + questreturnerCounter + " questreturners | " + prereqCounter + " prereqs | " + rewardCounter + " rewards");
            Console.WriteLine(secondaryrewardCounter + " secondaryrewards | " + diplomacysettingsCounter + " diplomacysettings | " + objectiveCounter + " objectives | " + secondaryobjectiveCounter + " secondaryobjectives");
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

        // Writes the diplomacysettings element and all it's descendants as a direct child of the root element.
        public static void diplomacysettings(string currentQuestFile, string tempFile)
        {
            XDocument questFileInstance = XDocument.Load(currentQuestFile);

            List<XElement> diplomacysettings = new List<XElement>();
            List<XElement> elementsToWrite = new List<XElement>();

            foreach (XElement element in questFileInstance.Descendants())
            {
                if (element.Name.ToString() == "diplomacysettings" && element.Parent.Name.ToString() == "quest" && element.Descendants().Count() > 0)
                {
                    diplomacysettings.Add(element);
                }
            }

            if (diplomacysettings.Count() > 0)
            {
                foreach (XElement diplomacysetting in diplomacysettings)
                {
                    elementsToWrite.Add(diplomacysetting);
                }
            }

            XDocument tempXDocInstance = XDocument.Load(tempFile);
            XElement questElement = tempXDocInstance.Descendants().First();

            foreach (XElement parsedElement in elementsToWrite)
            {
                questElement.Add(parsedElement);
                diplomacysettingsCounter++;
            }

            tempXDocInstance.Save(tempFile);
        }

        // Writes the objectives element and all it's descendants as a direct child of the root element.
        public static void objectives(string currentQuestFile, string tempFile)
        {
            XDocument questFileInstance = XDocument.Load(currentQuestFile);

            List<XElement> objectives = new List<XElement>();
            List<XElement> descriptions = new List<XElement>();
            List<XElement> elementsToWrite = new List<XElement>();

            foreach (XElement element in questFileInstance.Descendants())
            {
                if (element.Name.ToString() == "objectives" && element.Parent.Name.ToString() == "quest" && element.Descendants().Count() > 0)
                {
                    objectives.Add(element);
                }
            }

            foreach (XElement objective in objectives)
            {
                foreach (XElement descendant in objective.Descendants())
                {
                    if (descendant.Descendants().Count() > 0 && descendant.Name.ToString() != "values" && descendant.Name.ToString() != "or" && descendant.Name.ToString() != "and")
                    {
                        foreach (XElement subdescendant in descendant.Descendants())
                        {
                            descendant.Add(new XAttribute(subdescendant.Name.ToString(), subdescendant.Value.ToString()));
                        }

                        descendant.RemoveNodes();
                    }

                    else if (descendant.Descendants().Count() == 0 && descendant.Name.ToString() == "description" && descendant.Value.ToString() != "" && (descendant.Parent.Name.ToString() == "objectives" || descendant.Parent.Name.ToString() == "or" || descendant.Parent.Name.ToString() == "and"))
                    {
                        descendant.Parent.Add(new XAttribute(descendant.Name.ToString(), descendant.Value.ToString()));
                    }

                    else if (descendant.Name.ToString() != "values" && descendant.Name.ToString() != "or" && descendant.Name.ToString() != "and" && descendant.Name.ToString() != "description")
                    {
                        Console.WriteLine("\n" + "[ERROR] The objectives\\*\\" + descendant.Name.ToString() + " element has not been fully processed.");
                        Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                    }
                }

                foreach (XElement descendant in objective.Descendants())
                {
                    if (descendant.Name.ToString() == "description")
                    {
                        descriptions.Add(descendant);
                    }
                }

                foreach (XElement description in descriptions)
                {
                    description.Remove();
                }

                descriptions = new List<XElement>();
                elementsToWrite.Add(objective);
            }

            XDocument tempXDocInstance = XDocument.Load(tempFile);
            XElement questElement = tempXDocInstance.Descendants().First();

            foreach (XElement parsedElement in elementsToWrite)
            {
                questElement.Add(parsedElement);
                objectiveCounter++;
            }

            tempXDocInstance.Save(tempFile);
        }

        // Writes the onaccept element and all it's descendants as a direct child of the root element.
        public static void onaccept(string currentQuestFile, string tempFile)
        {
            XDocument questFileInstance = XDocument.Load(currentQuestFile);

            List<KeyValuePair<string, string>> questgiverDescendants = new List<KeyValuePair<string, string>>();

            List<XElement> noDescendants = new List<XElement>();
            List<XElement> onacceptElements = new List<XElement>();
            List<XElement> questgivers = new List<XElement>();
            List<XElement> elementsToWrite = new List<XElement>();

            foreach (XElement element in questFileInstance.Descendants())
            {
                if (element.Name.ToString() == "onaccept" && element.Parent.Name.ToString() == "quest" && element.Descendants().Count() > 0)
                {
                    onacceptElements.Add(element);
                }
            }

            foreach (XElement onaccept in onacceptElements)
            {
                foreach (XElement subElement in onaccept.Descendants())
                {
                    if (subElement.Parent.Name.ToString() == "onaccept" && subElement.Descendants().Count() == 0 && subElement.Value != "")
                    {
                        if (subElement.Name.ToString() == "protip" || subElement.Name.ToString() == "enableprotip")
                        {
                            subElement.Add(new XAttribute("tip", subElement.Value.ToString()));

                            XElement tip = new XElement(subElement.Name.ToString());

                            foreach (XAttribute attribute in subElement.Attributes())
                            {
                                tip.Add(new XAttribute(attribute.Name.ToString(), attribute.Value.ToString()));
                            }

                            noDescendants.Add(new XElement(tip));
                        }

                        else if (subElement.Name.ToString() == "unlockregion")
                        {
                            subElement.Add(new XAttribute("region", subElement.Value.ToString()));

                            XElement region = new XElement(subElement.Name.ToString());

                            foreach (XAttribute attribute in subElement.Attributes())
                            {
                                region.Add(new XAttribute(attribute.Name.ToString(), attribute.Value.ToString()));
                            }

                            noDescendants.Add(new XElement(region));
                        }

                        else if (subElement.Name.ToString() == "blueprint")
                        {
                            subElement.Add(new XAttribute("unit", subElement.Value.ToString()));

                            XElement blueprint = new XElement(subElement.Name.ToString());

                            foreach (XAttribute attribute in subElement.Attributes())
                            {
                                blueprint.Add(new XAttribute(attribute.Name.ToString(), attribute.Value.ToString()));
                            }

                            noDescendants.Add(new XElement(blueprint));
                        }

                        else
                        {
                            Console.WriteLine("\n" + "[ERROR] The onaccept\\" + subElement.Name.ToString() + " element has not been fully processed.");
                            Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                        }
                    }

                    else if (subElement.Parent.Name.ToString() == "onaccept" && subElement.Descendants().Count() > 0 && subElement.Name.ToString() == "questgiver")
                    {
                        XElement questgiverInstance = new XElement("questgiver");
                        questgiverInstance = subElement;

                        foreach (XElement descendant in subElement.Descendants())
                        {
                            if (descendant.Parent.Name.ToString() == "questgiver" && descendant.Descendants().Count() == 0 && descendant.Value.ToString() != "")
                            {
                                questgiverDescendants.Add(new KeyValuePair<string, string>(descendant.Name.ToString(), descendant.Value.ToString()));
                            }
                            else if (descendant.Parent.Name.ToString() != "questgiver")
                            {
                                Console.WriteLine("\n" + "[ERROR] The onaccept\\questgiver\\" + descendant.Name.ToString() + " element has not been fully processed.");
                                Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                            }
                        }

                        if (questgiverDescendants.Count() > 0)
                        {
                            foreach (KeyValuePair<string, string> questgiverDescendant in questgiverDescendants)
                            {
                                questgiverInstance.Add(new XAttribute(questgiverDescendant.Key, questgiverDescendant.Value));
                            }
                        }

                        questgiverInstance.RemoveNodes();
                        questgivers.Add(questgiverInstance);
                        questgiverInstance = new XElement("questgiver");
                        questgiverDescendants = new List<KeyValuePair<string, string>>();
                    }

                    else if (subElement.Parent.Name.ToString() == "onaccept" && subElement.Descendants().Count() > 0 && subElement.Name.ToString() != "questgiver")
                    {
                        Console.WriteLine("\n" + "[ERROR] The onaccept\\" + subElement.Name.ToString() + " element has not been fully processed.");
                        Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                    }
                }

                onaccept.RemoveNodes();

                if (noDescendants.Count() > 0)
                {
                    foreach (XElement nodescendant in noDescendants)
                    {
                        onaccept.Add(nodescendant);
                    }
                }

                if (questgivers.Count() > 0)
                {
                    foreach (XElement questgiver in questgivers)
                    {
                        onaccept.Add(questgiver);
                    }
                }

                elementsToWrite.Add(onaccept);
                noDescendants = new List<XElement>();
                questgivers = new List<XElement>();
            }

            XDocument tempXDocInstance = XDocument.Load(tempFile);
            XElement questElement = tempXDocInstance.Descendants().First();

            foreach (XElement parsedElement in elementsToWrite)
            {
                questElement.Add(parsedElement);
                onacceptCounter++;
            }

            tempXDocInstance.Save(tempFile);
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
                if (element.Name.ToString() == "playersettings" && element.Parent.Name.ToString() == "quest" && element.Descendants().Count() > 0)
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
                    else if (subElement.Parent.Name.ToString() == "playersettings" && subElement.Descendants().Count() > 0 && subElement.Name.ToString() != "aiflagvariables" && subElement.Name.ToString() != "aislidervariables")
                    {
                        Console.WriteLine("\n" + "[ERROR] The playersettings\\" + subElement.Name.ToString() + " element has not been fully processed.");
                        Console.WriteLine("[FILE] " + currentQuestFile + "\n");
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

        // Writes the prereqs element and all it's descendants as a direct child of the root element.
        public static void prereqs(string currentQuestFile, string tempFile)
        {
            XDocument questFileInstance = XDocument.Load(currentQuestFile);

            List<XElement> prereqs = new List<XElement>();
            List<XElement> elementsToWrite = new List<XElement>();

            foreach (XElement element in questFileInstance.Descendants())
            {
                if (element.Name.ToString() == "prereqs" && element.Parent.Name.ToString() == "quest" && element.Descendants().Count() > 0)
                {
                    prereqs.Add(element);
                }
            }

            foreach (XElement prereq in prereqs)
            {
                foreach (XElement subElement in prereq.Descendants())
                {
                    if (subElement.Descendants().Count() > 0 && (subElement.Name.ToString() == "questcomplete" ||
                        subElement.Name.ToString() == "queststatus" ||
                        subElement.Name.ToString() == "level" ||
                        subElement.Name.ToString() == "civilization"))
                    {
                        foreach (XElement descendant in subElement.Descendants())
                        {
                            subElement.Add(new XAttribute(descendant.Name.ToString(), descendant.Value.ToString()));
                        }

                        subElement.RemoveNodes();
                    }

                    else if (subElement.Descendants().Count() > 0 && (subElement.Name.ToString() != "questcomplete" &&
                        subElement.Name.ToString() != "queststatus" &&
                        subElement.Name.ToString() != "level" &&
                        subElement.Name.ToString() != "civilization" &&
                        subElement.Name.ToString() != "values" &&
                        subElement.Name.ToString() != "or" &&
                        subElement.Name.ToString() != "and"))
                    {
                        Console.WriteLine("\n" + "[ERROR] The prereqs\\*\\" + subElement.Name.ToString() + " element has not been fully processed.");
                        Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                    }
                }

                elementsToWrite.Add(prereq);
            }

            XDocument tempXDocInstance = XDocument.Load(tempFile);
            XElement questElement = tempXDocInstance.Descendants().First();

            foreach (XElement parsedElement in elementsToWrite)
            {
                questElement.Add(parsedElement);
                prereqCounter++;
            }

            tempXDocInstance.Save(tempFile);
        }

        // Writes the questgivers element and all it's descendants as a direct child of the root element.
        public static void questgivers(string currentQuestFile, string tempFile)
        {
            XDocument questFileInstance = XDocument.Load(currentQuestFile);

            List<XElement> questgiverElements = new List<XElement>();
            List<XElement> noDescendants = new List<XElement>();
            List<XElement> elementsToWrite = new List<XElement>();

            foreach (XElement element in questFileInstance.Descendants())
            {
                if (element.Name.ToString() == "questgivers" && element.Parent.Name.ToString() == "quest" && element.Descendants().Count() > 0)
                {
                    questgiverElements.Add(element);
                }
            }

            foreach (XElement questgiver in questgiverElements)
            {
                foreach (XElement subElement in questgiver.Descendants())
                {
                    if (subElement.Parent.Name.ToString() == "questgivers" && subElement.Descendants().Count() == 0 && subElement.Value.ToString() != "")
                    {
                        subElement.Add(new XAttribute("unit", subElement.Value.ToString()));

                        XElement unit = new XElement(subElement.Name.ToString());

                        foreach (XAttribute attribute in subElement.Attributes())
                        {
                            unit.Add(new XAttribute(attribute.Name.ToString(), attribute.Value.ToString()));
                        }

                        noDescendants.Add(new XElement(unit));
                    }
                    else if (subElement.Parent.Name.ToString() == "questgivers" && subElement.Descendants().Count() > 0)
                    {
                        Console.WriteLine("\n" + "[ERROR] The questgivers\\" + subElement.Name.ToString() + " element has not been fully processed.");
                        Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                    }
                }

                questgiver.RemoveNodes();

                if (noDescendants.Count() > 0)
                {
                    foreach (XElement nodescendant in noDescendants)
                    {
                        questgiver.Add(nodescendant);
                    }
                }

                elementsToWrite.Add(questgiver);
                noDescendants = new List<XElement>();
            }

            XDocument tempXDocInstance = XDocument.Load(tempFile);
            XElement questElement = tempXDocInstance.Descendants().First();

            foreach (XElement parsedElement in elementsToWrite)
            {
                questElement.Add(parsedElement);
                questgiverCounter++;
            }

            tempXDocInstance.Save(tempFile);
        }

        // Writes the questreturners element and all it's descendants as a direct child of the root element.
        public static void questreturners(string currentQuestFile, string tempFile)
        {
            XDocument questFileInstance = XDocument.Load(currentQuestFile);

            List<XElement> questreturners = new List<XElement>();
            List<XElement> noDescendants = new List<XElement>();
            List<XElement> elementsToWrite = new List<XElement>();

            foreach (XElement element in questFileInstance.Descendants())
            {
                if (element.Name.ToString() == "questreturners" && element.Parent.Name.ToString() == "quest" && element.Descendants().Count() > 0)
                {
                    questreturners.Add(element);
                }
            }

            foreach (XElement questreturner in questreturners)
            {
                foreach (XElement subElement in questreturner.Descendants())
                {
                    if (subElement.Parent.Name.ToString() == "questreturners" && subElement.Descendants().Count() == 0 && subElement.Value.ToString() != "")
                    {
                        subElement.Add(new XAttribute("unit", subElement.Value.ToString()));

                        XElement unit = new XElement(subElement.Name.ToString());

                        foreach (XAttribute attribute in subElement.Attributes())
                        {
                            unit.Add(new XAttribute(attribute.Name.ToString(), attribute.Value.ToString()));
                        }

                        noDescendants.Add(new XElement(unit));
                    }
                    else if (subElement.Parent.Name.ToString() == "questreturners" && subElement.Descendants().Count() > 0)
                    {
                        Console.WriteLine("\n" + "[ERROR] The questreturners\\" + subElement.Name.ToString() + " element has not been fully processed.");
                        Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                    }
                }

                questreturner.RemoveNodes();

                if (noDescendants.Count() > 0)
                {
                    foreach (XElement nodescendant in noDescendants)
                    {
                        questreturner.Add(nodescendant);
                    }
                }

                elementsToWrite.Add(questreturner);
                noDescendants = new List<XElement>();
            }

            XDocument tempXDocInstance = XDocument.Load(tempFile);
            XElement questElement = tempXDocInstance.Descendants().First();

            foreach (XElement parsedElement in elementsToWrite)
            {
                questElement.Add(parsedElement);
                questreturnerCounter++;
            }

            tempXDocInstance.Save(tempFile);
        }

        // Writes the randommap element and all it's descendants as a direct child of the root element.
        public static void randommap(string currentQuestFile, string tempFile)
        {
            XDocument questFileInstance = XDocument.Load(currentQuestFile);

            List<KeyValuePair<string, string>> noDescendants = new List<KeyValuePair<string, string>>();
            List<XElement> descendants = new List<XElement>();

            List<XElement> randommapElements = new List<XElement>();
            List<XElement> elementsToWrite = new List<XElement>();

            foreach (XElement element in questFileInstance.Descendants())
            {
                if (element.Name.ToString() == "randommap" && element.Parent.Name.ToString() == "quest" && element.Descendants().Count() > 0)
                {
                    randommapElements.Add(element);
                }
            }

            foreach (XElement randommap in randommapElements)
            {
                foreach (XElement subElement in randommap.Descendants())
                {
                    if (subElement.Parent.Name.ToString() == "randommap" && subElement.Descendants().Count() == 0 && subElement.Value != "")
                    {
                        noDescendants.Add(new KeyValuePair<string, string>(subElement.Name.ToString(), subElement.Value.ToString()));
                    }

                    else if (subElement.Parent.Name.ToString() == "randommap" && subElement.Descendants().Count() > 0)
                    {
                        if (subElement.Descendants().Count() > 0 && (subElement.Name.ToString() == "mapvariables" || subElement.Name.ToString() == "nuggetoverrides"))
                        {
                            List<XElement> descList = new List<XElement>();

                            foreach (XElement descendant in subElement.Descendants())
                            {
                                if (descendant.Descendants().Count() == 0)
                                {
                                    XElement desc = new XElement(descendant.Name.ToString());

                                    foreach (XAttribute attribute in descendant.Attributes())
                                    {
                                        desc.Add(new XAttribute(attribute.Name.ToString(), attribute.Value.ToString()));
                                    }

                                    if (descendant.Value.ToString() != "")
                                    {
                                        desc.Add(new XAttribute("value", descendant.Value.ToString()));
                                    }

                                    descList.Add(desc);
                                }

                                else
                                {
                                    Console.WriteLine("\n" + "[ERROR] The randommap\\*\\" + descendant.Name.ToString() + " element has not been fully processed.");
                                    Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                                }
                            }

                            subElement.RemoveNodes();

                            if (descList.Count() > 0)
                            {
                                foreach (XElement mapvardesc in descList)
                                {
                                    subElement.Add(new XElement(mapvardesc));
                                }
                            }

                            descList = new List<XElement>();
                        }

                        descendants.Add(new XElement(subElement));
                    }

                    else if (subElement.Descendants().Count() > 0)
                    {
                        Console.WriteLine("\n" + "[ERROR] The randommap\\" + subElement.Name.ToString() + " element has not been fully processed.");
                        Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                    }
                }

                randommap.RemoveNodes();

                if (noDescendants.Count() > 0)
                {
                    foreach (KeyValuePair<string, string> newAttribute in noDescendants)
                    {
                        randommap.Add(new XAttribute(newAttribute.Key, newAttribute.Value));
                    }

                    foreach (XAttribute attribute in randommap.Attributes())
                    {
                        if (attribute.Value == "")
                        {
                            randommap.Attribute(attribute.Name).Remove();
                        }
                    }
                }

                if (descendants.Count() > 0)
                {
                    foreach (XElement descendant in descendants)
                    {
                        randommap.Add(descendant);
                    }
                }

                elementsToWrite.Add(randommap);
                noDescendants = new List<KeyValuePair<string, string>>();
            }

            XDocument tempXDocInstance = XDocument.Load(tempFile);
            XElement questElement = tempXDocInstance.Descendants().First();

            foreach (XElement parsedElement in elementsToWrite)
            {
                questElement.Add(parsedElement);
                randommapCounter++;
            }

            tempXDocInstance.Save(tempFile);
        }

        // Writes the rewards element and all it's descendants as a direct child of the root element.
        // The regex for finding broken traits is: <trait visible="true">(\s+)(\w+)(\s+)<traitlevel>
        // The regex for replacing broken traits is: <trait visible="true">\1<id>\2</id>\3<traitlevel>
        public static void rewards(string currentQuestFile, string tempFile)
        {
            XDocument questFileInstance = XDocument.Load(currentQuestFile);

            List<XElement> rewards = new List<XElement>();
            List<XElement> elementsToWrite = new List<XElement>();

            foreach (XElement element in questFileInstance.Descendants())
            {
                if (element.Name.ToString() == "rewards" && element.Parent.Name.ToString() == "quest" && element.Descendants().Count() > 0)
                {
                    rewards.Add(element);
                }
            }

            foreach (XElement reward in rewards)
            {
                XElement newRewards = new XElement("rewards");

                foreach (XAttribute attribute in reward.Attributes())
                {
                    newRewards.Add(new XAttribute(attribute.Name.ToString(), attribute.Value.ToString()));
                }

                foreach (XElement descendant in reward.Elements())
                {
                    if (descendant.Descendants().Count() == 0 && descendant.Parent.Name.ToString() == "rewards" && descendant.Value.ToString() != "")
                    {
                        XElement nodescendants = new XElement(descendant.Name.ToString());

                        foreach (XAttribute attribute in descendant.Attributes())
                        {
                            nodescendants.Add(new XAttribute(attribute.Name.ToString(), attribute.Value.ToString()));
                        }

                        if (descendant.Name.ToString() == "protip")
                        {
                            nodescendants.Add(new XAttribute("tip", descendant.Value.ToString()));
                        }
                        else if (descendant.Name.ToString() == "enableprotip")
                        {
                            nodescendants.Add(new XAttribute("tip", descendant.Value.ToString()));
                        }
                        else if (descendant.Name.ToString() == "xp")
                        {
                            nodescendants.Add(new XAttribute("amount", descendant.Value.ToString()));
                        }
                        else if (descendant.Name.ToString() == "blueprint")
                        {
                            nodescendants.Add(new XAttribute("unit", descendant.Value.ToString()));
                        }
                        else if (descendant.Name.ToString() == "advisor")
                        {
                            nodescendants.Add(new XAttribute("unit", descendant.Value.ToString()));
                        }
                        else if (descendant.Name.ToString() == "loottable")
                        {
                            nodescendants.Add(new XAttribute("type", descendant.Value.ToString()));
                        }
                        else if (descendant.Name.ToString() == "lockregion")
                        {
                            nodescendants.Add(new XAttribute("region", descendant.Value.ToString()));
                        }
                        else if (descendant.Name.ToString() == "unlockregion")
                        {
                            nodescendants.Add(new XAttribute("region", descendant.Value.ToString()));
                        }
                        else if (descendant.Name.ToString() == "mailreward")
                        {
                            nodescendants.Add(new XAttribute("reward", descendant.Value.ToString()));
                        }
                        else if (descendant.Name.ToString() == "trait")
                        {
                            nodescendants.Add(new XAttribute("id", descendant.Value.ToString()));
                        }
                        else if (descendant.Name.ToString() == "gamecurrency")
                        {
                            nodescendants.Add(new XAttribute("type", descendant.Value.ToString()));
                        }
                        else
                        {
                            Console.WriteLine("\n" + "[ERROR] The rewards\\" + descendant.Name.ToString() + " element has not been fully processed.");
                            Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                        }

                        newRewards.Add(new XElement(nodescendants));
                    }

                    else if (descendant.Descendants().Count() == 0 && descendant.Parent.Name.ToString() == "rewards" && descendant.Value.ToString() == "")
                    {
                        if (descendant.Attributes().Count() > 0)
                        {
                            newRewards.Add(new XElement(descendant));
                        }
                    }

                    else if (descendant.Descendants().Count() > 0 && descendant.Parent.Name.ToString() == "rewards" && descendant.Name.ToString() != "or")
                    {
                        XElement directdescendant = new XElement(descendant.Name.ToString());

                        foreach (XAttribute attribute in descendant.Attributes())
                        {
                            directdescendant.Add(new XAttribute(attribute.Name.ToString(), attribute.Value.ToString()));
                        }

                        foreach (XElement subdescendant in descendant.Descendants())
                        {
                            directdescendant.Add(new XAttribute(subdescendant.Name.ToString(), subdescendant.Value.ToString()));
                        }

                        newRewards.Add(new XElement(directdescendant));
                    }

                    else if (descendant.Descendants().Count() > 0 && descendant.Parent.Name.ToString() == "rewards" && descendant.Name.ToString() == "or")
                    {
                        XElement newOr = new XElement(descendant.Name.ToString());

                        foreach (XElement subdescendant in descendant.Elements())
                        {
                            if (subdescendant.Descendants().Count() == 0 && subdescendant.Parent.Name.ToString() == "or" && subdescendant.Value.ToString() != "")
                            {
                                XElement nodescendants = new XElement(subdescendant.Name.ToString());

                                foreach (XAttribute attribute in subdescendant.Attributes())
                                {
                                    nodescendants.Add(new XAttribute(attribute.Name.ToString(), attribute.Value.ToString()));
                                }

                                if (subdescendant.Name.ToString() == "protip")
                                {
                                    nodescendants.Add(new XAttribute("tip", subdescendant.Value.ToString()));
                                }
                                else if (subdescendant.Name.ToString() == "enableprotip")
                                {
                                    nodescendants.Add(new XAttribute("tip", subdescendant.Value.ToString()));
                                }
                                else if (subdescendant.Name.ToString() == "xp")
                                {
                                    nodescendants.Add(new XAttribute("amount", subdescendant.Value.ToString()));
                                }
                                else if (subdescendant.Name.ToString() == "blueprint")
                                {
                                    nodescendants.Add(new XAttribute("unit", subdescendant.Value.ToString()));
                                }
                                else if (subdescendant.Name.ToString() == "advisor")
                                {
                                    nodescendants.Add(new XAttribute("unit", subdescendant.Value.ToString()));
                                }
                                else if (subdescendant.Name.ToString() == "loottable")
                                {
                                    nodescendants.Add(new XAttribute("type", subdescendant.Value.ToString()));
                                }
                                else if (subdescendant.Name.ToString() == "lockregion")
                                {
                                    nodescendants.Add(new XAttribute("region", subdescendant.Value.ToString()));
                                }
                                else if (subdescendant.Name.ToString() == "unlockregion")
                                {
                                    nodescendants.Add(new XAttribute("region", subdescendant.Value.ToString()));
                                }
                                else if (subdescendant.Name.ToString() == "mailreward")
                                {
                                    nodescendants.Add(new XAttribute("reward", subdescendant.Value.ToString()));
                                }
                                else if (subdescendant.Name.ToString() == "trait")
                                {
                                    nodescendants.Add(new XAttribute("id", subdescendant.Value.ToString()));
                                }
                                else if (subdescendant.Name.ToString() == "gamecurrency")
                                {
                                    nodescendants.Add(new XAttribute("type", subdescendant.Value.ToString()));
                                }
                                else
                                {
                                    Console.WriteLine("\n" + "[ERROR] The rewards\\" + subdescendant.Name.ToString() + " element has not been fully processed.");
                                    Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                                }

                                newOr.Add(new XElement(nodescendants));
                            }

                            else if (subdescendant.Descendants().Count() == 0 && subdescendant.Parent.Name.ToString() == "or" && subdescendant.Value.ToString() == "")
                            {
                                if (subdescendant.Attributes().Count() > 0)
                                {
                                    newOr.Add(new XElement(subdescendant));
                                }
                            }

                            else if (subdescendant.Descendants().Count() > 0 && subdescendant.Parent.Name.ToString() == "or")
                            {
                                XElement directdescendant = new XElement(subdescendant.Name.ToString());

                                foreach (XAttribute attribute in subdescendant.Attributes())
                                {
                                    directdescendant.Add(new XAttribute(attribute.Name.ToString(), attribute.Value.ToString()));
                                }

                                foreach (XElement subsubdescendant in subdescendant.Descendants())
                                {
                                    directdescendant.Add(new XAttribute(subsubdescendant.Name.ToString(), subsubdescendant.Value.ToString()));
                                }

                                newOr.Add(new XElement(directdescendant));
                            }

                            else
                            {
                                Console.WriteLine("\n" + "[ERROR] The rewards\\or\\" + descendant.Name.ToString() + " element has not been fully processed.");
                                Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                            }
                        }

                        newRewards.Add(new XElement(newOr));
                    }

                    else
                    {
                        Console.WriteLine("\n" + "[ERROR] The rewards\\" + descendant.Name.ToString() + " element has not been fully processed.");
                        Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                    }
                }

                elementsToWrite.Add(newRewards);
            }

            XDocument tempXDocInstance = XDocument.Load(tempFile);
            XElement questElement = tempXDocInstance.Descendants().First();

            foreach (XElement parsedElement in elementsToWrite)
            {
                questElement.Add(parsedElement);
                rewardCounter++;
            }

            tempXDocInstance.Save(tempFile);
        }

        // Writes the secondaryobjectives element and all it's descendants as a direct child of the root element.
        public static void secondaryobjectives(string currentQuestFile, string tempFile)
        {
            XDocument questFileInstance = XDocument.Load(currentQuestFile);

            List<XElement> secondaryobjectives = new List<XElement>();
            List<XElement> descriptions = new List<XElement>();
            List<XElement> elementsToWrite = new List<XElement>();

            foreach (XElement element in questFileInstance.Descendants())
            {
                if (element.Name.ToString() == "secondaryobjectives" && element.Parent.Name.ToString() == "quest" && element.Descendants().Count() > 0)
                {
                    secondaryobjectives.Add(element);
                }
            }

            foreach (XElement secondaryobjective in secondaryobjectives)
            {
                foreach (XElement descendant in secondaryobjective.Descendants())
                {
                    if (descendant.Descendants().Count() > 0 && descendant.Name.ToString() != "values" && descendant.Name.ToString() != "or" && descendant.Name.ToString() != "and")
                    {
                        foreach (XElement subdescendant in descendant.Descendants())
                        {
                            descendant.Add(new XAttribute(subdescendant.Name.ToString(), subdescendant.Value.ToString()));
                        }

                        descendant.RemoveNodes();
                    }

                    else if (descendant.Descendants().Count() == 0 && descendant.Name.ToString() == "description" && descendant.Value.ToString() != "" && (descendant.Parent.Name.ToString() == "secondaryobjectives" || descendant.Parent.Name.ToString() == "or" || descendant.Parent.Name.ToString() == "and"))
                    {
                        descendant.Parent.Add(new XAttribute(descendant.Name.ToString(), descendant.Value.ToString()));
                    }

                    else if (descendant.Name.ToString() != "values" && descendant.Name.ToString() != "or" && descendant.Name.ToString() != "and" && descendant.Name.ToString() != "description")
                    {
                        Console.WriteLine("\n" + "[ERROR] The secondaryobjective\\*\\" + descendant.Name.ToString() + " element has not been fully processed.");
                        Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                    }
                }

                foreach (XElement descendant in secondaryobjective.Descendants())
                {
                    if (descendant.Name.ToString() == "description")
                    {
                        descriptions.Add(descendant);
                    }
                }

                foreach (XElement description in descriptions)
                {
                    description.Remove();
                }

                descriptions = new List<XElement>();
                elementsToWrite.Add(secondaryobjective);
            }

            XDocument tempXDocInstance = XDocument.Load(tempFile);
            XElement questElement = tempXDocInstance.Descendants().First();

            foreach (XElement parsedElement in elementsToWrite)
            {
                questElement.Add(parsedElement);
                secondaryobjectiveCounter++;
            }

            tempXDocInstance.Save(tempFile);
        }

        // Writes the secondaryrewards element and all it's descendants as a direct child of the root element.
        public static void secondaryrewards(string currentQuestFile, string tempFile)
        {
            XDocument questFileInstance = XDocument.Load(currentQuestFile);

            List<XElement> secondaryrewards = new List<XElement>();
            List<XElement> subElements = new List<XElement>();
            List<XElement> elementsToWrite = new List<XElement>();

            foreach (XElement element in questFileInstance.Descendants())
            {
                if (element.Name.ToString() == "secondaryrewards" && element.Parent.Name.ToString() == "quest" && element.Descendants().Count() > 0)
                {
                    secondaryrewards.Add(element);
                }
            }

            foreach (XElement secondaryreward in secondaryrewards)
            {
                foreach (XElement subElement in secondaryreward.Descendants())
                {
                    if (subElement.Parent.Name.ToString() == "secondaryrewards" && (subElement.Attributes().Count() > 0 || subElement.Value.ToString() != ""))
                    {
                        if (subElement.Descendants().Count() > 0)
                        {
                            foreach (XElement descendant in subElement.Descendants())
                            {
                                if (descendant.Descendants().Count() == 0)
                                {
                                    subElement.Add(new XAttribute(descendant.Name.ToString(), descendant.Value.ToString()));
                                }

                                else
                                {
                                    Console.WriteLine("\n" + "[ERROR] The secondaryrewards\\*\\" + descendant.Name.ToString() + " element has not been fully processed.");
                                    Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                                }
                            }

                            subElement.RemoveNodes();
                            subElements.Add(subElement);
                        }

                        else if (subElement.Descendants().Count() == 0)
                        {
                            XElement newElement = new XElement(subElement.Name.ToString());

                            foreach (XAttribute attribute in subElement.Attributes())
                            {
                                newElement.Add(new XAttribute(attribute.Name.ToString(), attribute.Value.ToString()));
                            }

                            newElement.Add(new XAttribute(subElement.Name.ToString(), subElement.Value.ToString()));

                            subElements.Add(newElement);
                        }
                    }
                }

                secondaryreward.RemoveNodes();

                if (subElements.Count() > 0)
                {
                    foreach (XElement element in subElements)
                    {
                        secondaryreward.Add(element);
                    }
                }

                subElements = new List<XElement>();
                elementsToWrite.Add(secondaryreward);
            }

            XDocument tempXDocInstance = XDocument.Load(tempFile);
            XElement questElement = tempXDocInstance.Descendants().First();

            foreach (XElement parsedElement in elementsToWrite)
            {
                questElement.Add(parsedElement);
                secondaryrewardCounter++;
            }

            tempXDocInstance.Save(tempFile);
        }

        // Writes the targets element and all it's descendants as a direct child of the root element.
        public static void targets(string currentQuestFile, string tempFile)
        {
            XDocument questFileInstance = XDocument.Load(currentQuestFile);

            List<XElement> targets = new List<XElement>();
            List<XElement> directnodes = new List<XElement>();
            List<XElement> overrides = new List<XElement>();

            List<XElement> randomunitprobability = new List<XElement>();
            List<XElement> randomtargets = new List<XElement>();
            List<XElement> randomprotounit = new List<XElement>();
            List<XElement> randomoverrides = new List<XElement>();

            List<XElement> elementsToWrite = new List<XElement>();

            foreach (XElement element in questFileInstance.Descendants())
            {
                if (element.Name.ToString() == "targets" && element.Parent.Name.ToString() == "quest" && element.Descendants().Count() > 0)
                {
                    targets.Add(element);
                }
            }

            foreach (XElement target in targets)
            {
                foreach (XElement subElement in target.Descendants())
                {
                    if (subElement.Parent.Name.ToString() == "targets" && subElement.Value.ToString() != "" && (subElement.Name.ToString() == "grouping" || subElement.Name.ToString() == "protounit" || subElement.Name.ToString() == "random"))
                    {
                        directnodes.Add(subElement);
                    }

                    else if (subElement.Parent.Name.ToString() == "targets" && subElement.Value.ToString() != "" && (subElement.Name.ToString() != "grouping" && subElement.Name.ToString() != "protounit"))
                    {
                        Console.WriteLine("\n" + "[ERROR] The targets\\" + subElement.Name.ToString() + " element has not been fully processed.");
                        Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                    }
                }

                foreach (XElement directnode in directnodes)
                {
                    if (directnode.Name.ToString() == "grouping")
                    {
                        foreach (XElement descendant in directnode.Descendants())
                        {
                            if (descendant.Parent.Name.ToString() == "grouping" && descendant.Descendants().Count() == 0)
                            {
                                directnode.Add(new XAttribute(descendant.Name.ToString(), descendant.Value.ToString()));
                            }

                            else if (descendant.Parent.Name.ToString() == "grouping" && descendant.Descendants().Count() > 0)
                            {
                                Console.WriteLine("\n" + "[ERROR] The targets\\grouping\\" + descendant.Name.ToString() + " element has not been fully processed.");
                                Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                            }
                        }

                        directnode.RemoveNodes();
                    }

                    else if (directnode.Name.ToString() == "protounit")
                    {
                        foreach (XElement descendant in directnode.Descendants())
                        {
                            if (descendant.Parent.Name.ToString() == "protounit" && descendant.Descendants().Count() == 0)
                            {
                                directnode.Add(new XAttribute(descendant.Name.ToString(), descendant.Value.ToString()));
                            }

                            else if (descendant.Parent.Name.ToString() == "protounit" && descendant.Descendants().Count() > 0 && descendant.Name.ToString() == "overrides")
                            {
                                foreach (XElement subdescendent in descendant.Descendants())
                                {
                                    descendant.Add(new XAttribute(subdescendent.Name.ToString(), subdescendent.Value.ToString()));
                                }

                                descendant.RemoveNodes();
                                overrides.Add(descendant);
                            }

                            else if (descendant.Parent.Name.ToString() == "protounit" && descendant.Descendants().Count() > 0 && descendant.Name.ToString() != "overrides")
                            {
                                Console.WriteLine("\n" + "[ERROR] The targets\\protounit\\" + descendant.Name.ToString() + " element has not been fully processed.");
                                Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                            }
                        }

                        directnode.RemoveNodes();

                        if (overrides.Count() > 0)
                        {
                            foreach (XElement overridenode in overrides)
                            {
                                directnode.Add(new XElement(overridenode));
                            }
                        }

                        overrides = new List<XElement>();
                    }

                    else if (directnode.Name.ToString() == "random")
                    {
                        foreach (XElement subSubElement in directnode.Descendants())
                        {
                            if (subSubElement.Parent.Name.ToString() == "random" && subSubElement.Descendants().Count() == 0 && subSubElement.Value.ToString() != "")
                            {
                                directnode.Add(new XAttribute(subSubElement.Name.ToString(), subSubElement.Value.ToString()));
                            }

                            else if (subSubElement.Parent.Name.ToString() == "random" && subSubElement.Name.ToString() == "unitprobability")
                            {
                                randomunitprobability.Add(subSubElement);
                            }

                            else if (subSubElement.Parent.Name.ToString() == "random" && subSubElement.Name.ToString() == "targets" && subSubElement.Value.ToString() != "")
                            {
                                foreach (XElement subSubSubElement in subSubElement.Descendants())
                                {
                                    if (subSubSubElement.Parent.Name.ToString() == "targets" && subSubSubElement.Name.ToString() == "protounit" && subSubSubElement.Value.ToString() != "")
                                    {
                                        foreach (XElement subSubSubSubElement in subSubSubElement.Descendants())
                                        {
                                            if (subSubSubSubElement.Parent.Name.ToString() == "protounit" && subSubSubSubElement.Descendants().Count() == 0 && subSubSubSubElement.Value.ToString() != "")
                                            {
                                                subSubSubElement.Add(new XAttribute(subSubSubSubElement.Name.ToString(), subSubSubSubElement.Value.ToString()));
                                            }

                                            else if (subSubSubSubElement.Parent.Name.ToString() == "protounit" && subSubSubSubElement.Descendants().Count() > 0 && subSubSubSubElement.Value.ToString() != "" && subSubSubSubElement.Name.ToString() == "overrides")
                                            {
                                                foreach (XElement subSubSubSubSubElement in subSubSubSubElement.Descendants())
                                                {
                                                    if (subSubSubSubSubElement.Parent.Name.ToString() == "overrides" && subSubSubSubSubElement.Descendants().Count() == 0)
                                                    {
                                                        subSubSubSubElement.Add(new XAttribute(subSubSubSubSubElement.Name.ToString(), subSubSubSubSubElement.Value.ToString()));
                                                    }

                                                    else if (subSubSubSubSubElement.Parent.Name.ToString() == "overrides" && subSubSubSubSubElement.Descendants().Count() > 0)
                                                    {
                                                        Console.WriteLine("\n" + "[ERROR] The targets\\random\\targets\\protounit\\overrides\\" + subSubSubSubElement.Name.ToString() + " element has not been fully processed.");
                                                        Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                                                    }
                                                }

                                                subSubSubSubElement.RemoveNodes();
                                                randomoverrides.Add(subSubSubSubElement);
                                            }

                                            else if (subSubSubSubElement.Parent.Name.ToString() == "protounit" && subSubSubSubElement.Descendants().Count() > 0 && subSubSubSubElement.Value.ToString() != "" && subSubSubSubElement.Name.ToString() != "overrides")
                                            {
                                                Console.WriteLine("\n" + "[ERROR] The targets\\random\\targets\\" + subSubSubSubElement.Name.ToString() + " element has not been fully processed.");
                                                Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                                            }
                                        }

                                        subSubSubElement.RemoveNodes();

                                        if (randomoverrides.Count() > 0)
                                        {
                                            foreach (XElement randomoverride in randomoverrides)
                                            {
                                                subSubSubElement.Add(new XElement(randomoverride));
                                            }
                                        }

                                        randomoverrides = new List<XElement>();

                                        randomprotounit.Add(subSubSubElement);
                                    }
                                }

                                subSubElement.RemoveNodes();

                                if (randomprotounit.Count() > 0)
                                {
                                    foreach (XElement randomproto in randomprotounit)
                                    {
                                        subSubElement.Add(new XElement(randomproto));
                                    }
                                }

                                randomprotounit = new List<XElement>();

                                randomtargets.Add(subSubElement);
                            }

                            else if (subSubElement.Parent.Name.ToString() == "random" && subSubElement.Value.ToString() != "" && (subSubElement.Name.ToString() != "unitprobability" && subSubElement.Name.ToString() != "targets"))
                            {
                                Console.WriteLine("\n" + "[ERROR] The targets\\random\\" + subSubElement.Name.ToString() + " element has not been fully processed.");
                                Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                            }
                        }

                        directnode.RemoveNodes();

                        if (randomunitprobability.Count() > 0)
                        {
                            foreach (XElement randomprod in randomunitprobability)
                            {
                                directnode.Add(new XElement(randomprod));
                            }
                        }

                        randomunitprobability = new List<XElement>();

                        if (randomtargets.Count() > 0)
                        {
                            foreach (XElement randomtarget in randomtargets)
                            {
                                directnode.Add(new XElement(randomtarget));
                            }
                        }

                        randomtargets = new List<XElement>();
                    }
                }

                elementsToWrite.Add(target);
                directnodes = new List<XElement>();
            }

            XDocument tempXDocInstance = XDocument.Load(tempFile);
            XElement questElement = tempXDocInstance.Descendants().First();

            foreach (XElement parsedElement in elementsToWrite)
            {
                questElement.Add(parsedElement);
                targetCounter++;
            }

            tempXDocInstance.Save(tempFile);
        }

        // Writes the timer element and all it's descendants as a direct child of the root element.
        public static void timer(string currentQuestFile, string tempFile)
        {
            XDocument questFileInstance = XDocument.Load(currentQuestFile);

            List<KeyValuePair<string, string>> noDescendants = new List<KeyValuePair<string, string>>();
            List<KeyValuePair<string, string>> timerEvents = new List<KeyValuePair<string, string>>();

            List<XElement> timerElements = new List<XElement>();
            List<XElement> elementsToWrite = new List<XElement>();

            foreach (XElement element in questFileInstance.Descendants())
            {
                if (element.Name.ToString() == "timer" && element.Parent.Name.ToString() == "quest" && element.Descendants().Count() > 0)
                {
                    timerElements.Add(element);
                }
            }

            foreach (XElement timer in timerElements)
            {
                foreach (XElement subElement in timer.Descendants())
                {
                    if (subElement.Name.ToString() == "timerevent" && subElement.Parent.Name.ToString() == "events" && subElement.Attributes().Count() > 0)
                    {
                        if (subElement.Attributes("event").First().Name.ToString() == "event" && subElement.Value.ToString() != "")
                        {
                            timerEvents.Add(new KeyValuePair<string, string>(subElement.Attribute("event").Value.ToString(), subElement.Value.ToString()));
                        }
                        else if (subElement.Attributes("event").First().Name.ToString() != "event" && subElement.Value.ToString() != "")
                        {
                            timerEvents.Add(new KeyValuePair<string, string>("", subElement.Value.ToString()));
                        }
                        else if (subElement.Attributes("event").First().Name.ToString() == "event" && subElement.Value.ToString() == "")
                        {
                            if (subElement.Attribute("event").Value.ToString() != "")
                            {
                                timerEvents.Add(new KeyValuePair<string, string>(subElement.Attribute("event").Value.ToString(), ""));
                            }
                        }
                    }
                    else if (subElement.Parent.Name.ToString() == "timer" && subElement.Descendants().Count() == 0 && subElement.Value != "")
                    {
                        noDescendants.Add(new KeyValuePair<string, string>(subElement.Name.ToString(), subElement.Value.ToString()));
                    }
                    else if (subElement.Parent.Name.ToString() == "timer" && subElement.Descendants().Count() > 0 && subElement.Name.ToString() != "events")
                    {
                        Console.WriteLine("\n" + "[ERROR] The timer\\" + subElement.Name.ToString() + " element has not been fully processed.");
                        Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                    }
                }

                timer.RemoveNodes();

                if (noDescendants.Count() > 0)
                {
                    foreach (KeyValuePair<string, string> newAttribute in noDescendants)
                    {
                        timer.Add(new XAttribute(newAttribute.Key, newAttribute.Value));
                    }
                }

                if (timerEvents.Count() > 0)
                {
                    timer.Add(new XElement("events"));

                    foreach (KeyValuePair<string, string> timerEvent in timerEvents)
                    {
                        timer.Descendants("events").First().Add(new XElement("timerevent", new XAttribute("event", timerEvent.Key), new XAttribute("time", timerEvent.Value)));

                        foreach (XAttribute attribute in timer.Attributes())
                        {
                            if (attribute.Value == "")
                            {
                                timer.Attribute(attribute.Name).Remove();
                            }
                        }

                        foreach (XElement timerevent in timer.Descendants())
                        {
                            foreach (XAttribute attribute in timerevent.Attributes())
                            {
                                if (attribute.Value == "")
                                {
                                    timerevent.Attribute(attribute.Name).Remove();
                                }
                            }
                        }
                    }
                }

                elementsToWrite.Add(timer);
                noDescendants = new List<KeyValuePair<string, string>>();
                timerEvents = new List<KeyValuePair<string, string>>();
            }

            XDocument tempXDocInstance = XDocument.Load(tempFile);
            XElement questElement = tempXDocInstance.Descendants().First();

            foreach (XElement parsedElement in elementsToWrite)
            {
                questElement.Add(parsedElement);
                timersCounter++;
            }

            tempXDocInstance.Save(tempFile);
        }

        // Writes the timereffects element and all it's descendants as a direct child of the root element.
        public static void timereffects(string currentQuestFile, string tempFile)
        {
            XDocument questFileInstance = XDocument.Load(currentQuestFile);

            List<KeyValuePair<string, string>> noDescendantsUnit = new List<KeyValuePair<string, string>>();
            List<KeyValuePair<string, string>> noDescendantsGroup = new List<KeyValuePair<string, string>>();

            List<XElement> timerEffectsList = new List<XElement>();
            List<XElement> elementsToWrite = new List<XElement>();

            foreach (XElement element in questFileInstance.Descendants())
            {
                if (element.Name.ToString() == "timereffects" && element.Parent.Name.ToString() == "quest" && element.Descendants().Count() > 0)
                {
                    timerEffectsList.Add(element);
                }
            }

            foreach (XElement timerEffect in timerEffectsList)
            {
                foreach (XElement subElement in timerEffect.Descendants())
                {
                    if (subElement.Parent.Name.ToString() == "spawnunit" && subElement.Descendants().Count() == 0 && subElement.Value != "")
                    {
                        noDescendantsUnit.Add(new KeyValuePair<string, string>(subElement.Name.ToString(), subElement.Value.ToString()));
                    }
                    else if (subElement.Parent.Name.ToString() == "spawnunit" && subElement.Descendants().Count() > 0)
                    {
                        Console.WriteLine("\n" + "[ERROR] The timer\\" + subElement.Name.ToString() + " element has not been fully processed.");
                        Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                    }

                    if (subElement.Parent.Name.ToString() == "spawngroup" && subElement.Descendants().Count() == 0 && subElement.Value != "")
                    {
                        noDescendantsGroup.Add(new KeyValuePair<string, string>(subElement.Name.ToString(), subElement.Value.ToString()));
                    }
                    else if (subElement.Parent.Name.ToString() == "spawngroup" && subElement.Descendants().Count() > 0)
                    {
                        Console.WriteLine("\n" + "[ERROR] The timereffect\\" + subElement.Name.ToString() + " element has not been fully processed.");
                        Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                    }
                }

                XElement spawnunitElement = new XElement("spawnunitElement");
                XElement spawngroupElement = new XElement("spawngroupElement");

                if (timerEffect.Descendants("spawnunit").Count() > 0)
                {
                    spawnunitElement = timerEffect.Descendants("spawnunit").First();
                    spawnunitElement.RemoveNodes();
                }

                if (timerEffect.Descendants("spawngroup").Count() > 0)
                {
                    spawngroupElement = timerEffect.Descendants("spawngroup").First();
                    spawngroupElement.RemoveNodes();
                }

                if (noDescendantsUnit.Count() > 0)
                {
                    foreach (KeyValuePair<string, string> newAttribute in noDescendantsUnit)
                    {
                        spawnunitElement.Add(new XAttribute(newAttribute.Key, newAttribute.Value));
                    }
                }

                if (noDescendantsGroup.Count() > 0)
                {
                    foreach (KeyValuePair<string, string> newAttribute in noDescendantsGroup)
                    {
                        spawngroupElement.Add(new XAttribute(newAttribute.Key, newAttribute.Value));
                    }
                }

                elementsToWrite.Add(timerEffect);
                noDescendantsUnit = new List<KeyValuePair<string, string>>();
                noDescendantsGroup = new List<KeyValuePair<string, string>>();
            }

            XDocument tempXDocInstance = XDocument.Load(tempFile);
            XElement questElement = tempXDocInstance.Descendants().First();

            foreach (XElement parsedElement in elementsToWrite)
            {
                questElement.Add(parsedElement);
                timereffectsCounter++;
            }

            tempXDocInstance.Save(tempFile);
        }

        // Writes the victoryconditions element and all it's descendants as a direct child of the root element.
        public static void victoryconditions(string currentQuestFile, string tempFile)
        {
            XDocument questFileInstance = XDocument.Load(currentQuestFile);

            List<XElement> noDescendants = new List<XElement>();
            List<XElement> victoryconditionsList = new List<XElement>();
            List<XElement> elementsToWrite = new List<XElement>();

            foreach (XElement element in questFileInstance.Descendants())
            {
                if (element.Name.ToString() == "victoryconditions" && element.Parent.Name.ToString() == "quest" && element.Descendants().Count() > 0)
                {
                    victoryconditionsList.Add(element);
                }
            }

            foreach (XElement victorycondition in victoryconditionsList)
            {
                foreach (XElement subElement in victorycondition.Descendants())
                {
                    if (subElement.Parent.Name.ToString() == "victoryconditions" && subElement.Descendants().Count() == 0 && subElement.Value.ToString() != "")
                    {
                        subElement.Add(new XAttribute("type", subElement.Value.ToString()));

                        XElement condition = new XElement(subElement.Name.ToString());

                        foreach (XAttribute attribute in subElement.Attributes())
                        {
                            condition.Add(new XAttribute(attribute.Name.ToString(), attribute.Value.ToString()));
                        }

                        noDescendants.Add(new XElement(condition));
                    }

                    else
                    {
                        Console.WriteLine("\n" + "[ERROR] The victoryconditions\\" + subElement.Name.ToString() + " element has not been fully processed.");
                        Console.WriteLine("[FILE] " + currentQuestFile + "\n");
                    }
                }

                victorycondition.RemoveNodes();

                if (noDescendants.Count() > 0)
                {
                    foreach (XElement newDescendant in noDescendants)
                    {
                        victorycondition.Add(new XElement(newDescendant));
                    }
                }

                elementsToWrite.Add(victorycondition);
                noDescendants = new List<XElement>();
            }

            XDocument tempXDocInstance = XDocument.Load(tempFile);
            XElement questElement = tempXDocInstance.Descendants().First();

            foreach (XElement parsedElement in elementsToWrite)
            {
                questElement.Add(parsedElement);
                victoryconditionsCounter++;
            }

            tempXDocInstance.Save(tempFile);
        }
    }
}
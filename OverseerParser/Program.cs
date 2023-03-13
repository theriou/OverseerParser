using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OverseerParser
{
    class Program
    {
        public class Quests
        {
            public int OverseerId { get; set; }
            public string OverseerName { get; set; }
            public string OverseerTier { get; set; }
            public double OverseerSuccess { get; set; }
            public string OverseerQuestType { get; set; }
        }

        public class Agents
        {
            public int AgentId { get; set; }
            public string AgentName { get; set; }
            public string AgentRarity { get; set; }
            public string AgentRetireConvert { get; set; }
            public string AgentJobs { get; set; }
            public string AgentTraits { get; set; }
        }

        public static string OverQuestTyping(int overseerType)
        {
            //The Types of Quest
            string overseerTyping = String.Empty;

            switch (overseerType)
            {
                case 1:
                    overseerTyping = "Plunder";
                    break;
                case 2:
                    overseerTyping = "Stealth";
                    break;
                case 3:
                    overseerTyping = "Military";
                    break;
                case 4:
                    overseerTyping = "Crafting";
                    break;
                case 5:
                    overseerTyping = "Harvesting";
                    break;
                case 6:
                    overseerTyping = "Research";
                    break;
                case 7:
                    overseerTyping = "Diplomacy";
                    break;
                case 8:
                    overseerTyping = "Trade";
                    break;
                case 9:
                    overseerTyping = "Exploration";
                    break;
                case 10:
                    overseerTyping = "Recruitment";
                    break;
                case 11:
                    overseerTyping = "Recovery";
                    break;
                case 12:
                    overseerTyping = "Conversion";
                    break;
                default:
                    overseerTyping = "";
                    break;

            }

            return overseerTyping;
        }

        public static string OverQuestDifficulty(int overseerQuestDifficulty)
        {
            //The Tiers of Difficulty - The level 1-5 chosen are based on your Jobs Level
            string overseerDifficulty = String.Empty;

            switch (overseerQuestDifficulty)
            {
                case 1:
                    overseerDifficulty = "Easy";
                    break;
                case 2:
                    overseerDifficulty = "Common";
                    break;
                case 3:
                    overseerDifficulty = "Uncommon";
                    break;
                case 4:
                    overseerDifficulty = "Rare";
                    break;
                case 5:
                    overseerDifficulty = "Elite";
                    break;
                default:
                    overseerDifficulty = "";
                    break;
            }

            return overseerDifficulty;
        }

        static void Main(string[] args)
        {

            List<Quests> overseerQuests = new List<Quests>();
            string fieldType = string.Empty,
                value = string.Empty,
                dbstrDataFinal = string.Empty,
                ovrFieldType = string.Empty,
                ovrDataNameFinal = string.Empty,
                ovrQuestsFile = "OvrQstClient.txt",
                dbstrFile = "dbstr_us.txt",
                agentJobFile = "OvrJobClient.txt",
                agentMiniFile = "OvrMiniClient.txt",
                agentMiniTraitFile = "OvrMiniTraitClient.txt";

            //Overseer Quest block
            if (File.Exists(ovrQuestsFile) && (File.Exists(dbstrFile)))
            {
                var dbstrLines = File.ReadAllLines(dbstrFile);
                var overseerQLines = File.ReadAllLines(ovrQuestsFile);

                //DBStr to List
                for (int i = 0; i < dbstrLines.Length; i++)
                {
                    var dbstrFields = dbstrLines[i].Split('^');
                    fieldType = dbstrFields[1];
                    if (fieldType == "56")
                    {
                        overseerQuests.Add(new Quests() { OverseerName = dbstrFields[2], OverseerId = int.Parse(dbstrFields[0]) });
                    }
                }

                //OvrQstClient to List
                for (int i = 0; i < overseerQLines.Length; i++)
                {
                    var ovrFields = overseerQLines[i].Split('^');

                    overseerQuests.Where(w => w.OverseerId == int.Parse(ovrFields[0])).ToList().ForEach(s => s.OverseerTier = OverQuestDifficulty(int.Parse(ovrFields[2])));
                    overseerQuests.Where(w => w.OverseerId == int.Parse(ovrFields[0])).ToList().ForEach(s => s.OverseerSuccess = (double.Parse(ovrFields[5]) * (double.Parse(ovrFields[4]) + 1)));
                    overseerQuests.Where(w => w.OverseerId == int.Parse(ovrFields[0])).ToList().ForEach(s => s.OverseerQuestType = OverQuestTyping(int.Parse(ovrFields[1])));
                }

                //Sort by ID->Tier->Name
                List<Quests> sortedQuests = overseerQuests.OrderBy(x => x.OverseerId).ThenBy(x => x.OverseerTier).ThenBy(x => x.OverseerName).ToList();

                //Write to File and Console
                using (StreamWriter writer = new StreamWriter("overseerquests.txt"))
                {
                    foreach (var quests in sortedQuests)
                    {
                        Console.WriteLine(string.Format($"{quests.OverseerId} | {quests.OverseerName} | {quests.OverseerTier} | {quests.OverseerQuestType} | {Math.Round(quests.OverseerSuccess)}%"));
                        writer.WriteLine(string.Format($"{quests.OverseerId} | {quests.OverseerName} | {quests.OverseerTier} | {quests.OverseerQuestType} | {Math.Round(quests.OverseerSuccess)}%"));
                    }
                }
            }
            else
            {
                //Files Not Found Error
                Console.WriteLine("Overseer Quest or DBStr files were not found");
            }

            //Overseer Agent Block
            if (File.Exists(agentJobFile) && (File.Exists(dbstrFile)) && (File.Exists(agentMiniFile)) && (File.Exists(agentMiniTraitFile)))
            {
                Dictionary<ulong, string> overseerAgent = new Dictionary<ulong, string>();
                Dictionary<ulong, string> overseerTraitName = new Dictionary<ulong, string>();
                var dbStrLines = File.ReadAllLines(dbstrFile);

                for (int i = 0; i < dbStrLines.Length; i++)
                {
                    var dbStrFields = dbStrLines[i].Split('^');
                    if (dbStrFields[1] == "53")
                    {
                        overseerAgent.Add(ulong.Parse(dbStrFields[0]), dbStrFields[2]);
                    }
                    if (dbStrFields[1] == "54")
                    {
                        overseerTraitName.Add(ulong.Parse(dbStrFields[0]), dbStrFields[2]);
                    }
                }

                var ovrObtainList = File.ReadAllLines(agentMiniFile);

                for (int i = 0; i < ovrObtainList.Length; i++)
                {
                    var ovrFields = ovrObtainList[i].Split('^');
                    ulong ovrAgentID = ulong.Parse(ovrFields[0]);

                    if (overseerAgent.ContainsKey(ovrAgentID))
                    {
                    }
                    else
                    {
                        overseerAgent.Add(ulong.Parse(ovrFields[0]), "");
                    }

                    if (ovrFields[1] == "5")
                    {
                        overseerAgent[ovrAgentID] += " | Elite" + (ovrFields[3] == "0" ? "" : " | Iconic") + (ovrFields[1] == "2" || ovrFields[4] == "1" ? "" : " | Convert To/Retire");
                    }
                    if (ovrFields[1] == "4")
                    {
                        overseerAgent[ovrAgentID] += " | Rare" + (ovrFields[3] == "0" ? "" : " | Iconic") + (ovrFields[1] == "2" || ovrFields[4] == "1" ? "" : " | Convert To/Retire");
                    }
                    if (ovrFields[1] == "3")
                    {
                        overseerAgent[ovrAgentID] += " | Uncommon" + (ovrFields[3] == "0" ? "" : " | Iconic") + (ovrFields[1] == "2" || ovrFields[4] == "1" ? "" : " | Convert To/Retire");
                    }
                    if (ovrFields[1] == "2")
                    {
                        overseerAgent[ovrAgentID] += " | Common" + (ovrFields[3] == "0" ? "" : " | Iconic") + (ovrFields[1] == "2" || ovrFields[4] == "1" ? "" : " | Convert To/Retire");
                    }
                }

                var ovrJobsList = File.ReadAllLines(agentJobFile);

                for (int i = 0; i < ovrJobsList.Length; i++)
                {
                    var ovrJobs = ovrJobsList[i].Split('^');
                    ulong ovrJobNum = ulong.Parse(ovrJobs[1]);

                    if (overseerAgent.ContainsKey(ovrJobNum))
                    {
                    }
                    else
                    {
                        overseerAgent.Add(ulong.Parse(ovrJobs[0]), "");
                    }

                    if (ovrJobs[0] == "1")
                    {
                        overseerAgent[ovrJobNum] += " | Marauder " + ovrJobs[5];
                    }
                    if (ovrJobs[0] == "2")
                    {
                        overseerAgent[ovrJobNum] += " | Spy " + ovrJobs[5];
                    }
                    if (ovrJobs[0] == "3")
                    {
                        overseerAgent[ovrJobNum] += " | Soldier " + ovrJobs[5];
                    }
                    if (ovrJobs[0] == "4")
                    {
                        overseerAgent[ovrJobNum] += " | Artisan " + ovrJobs[5];
                    }
                    if (ovrJobs[0] == "5")
                    {
                        overseerAgent[ovrJobNum] += " | Harvester " + ovrJobs[5];
                    }
                    if (ovrJobs[0] == "6")
                    {
                        overseerAgent[ovrJobNum] += " | Scholar " + ovrJobs[5];
                    }
                    if (ovrJobs[0] == "7")
                    {
                        overseerAgent[ovrJobNum] += " | Diplomat " + ovrJobs[5];
                    }
                    if (ovrJobs[0] == "8")
                    {
                        overseerAgent[ovrJobNum] += " | Merchant " + ovrJobs[5];
                    }
                    if (ovrJobs[0] == "9")
                    {
                        overseerAgent[ovrJobNum] += " | Explorer " + ovrJobs[5];
                    }
                }

                var ovrTraitsList = File.ReadAllLines(agentMiniTraitFile);

                for (int i = 0; i < ovrTraitsList.Length; i++)
                {
                    var ovrTraits = ovrTraitsList[i].Split('^');
                    ulong ovrTrait = ulong.Parse(ovrTraits[0]);
                    ulong ovrTraitAgent = ulong.Parse(ovrTraits[1]);

                    if (overseerAgent.ContainsKey(ovrTraitAgent))
                    {
                    }
                    else
                    {
                        overseerAgent.Add(ulong.Parse(ovrTraits[0]), "");
                    }

                    overseerAgent[ovrTraitAgent] += " | " + overseerTraitName[ovrTrait];

                }

                using (StreamWriter writer = new StreamWriter("agentnames.txt"))
                {
                    foreach (var result in overseerAgent)
                    {
                        writer.WriteLine(result.Key + " | " + result.Value);
                        Console.WriteLine(result.Key + " | " + result.Value);
                    }
                }

            }

            Console.ReadLine();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using CheckListToolWPF.Properties;
using CheckListToolWPF.Model;

namespace CheckListToolWPF
{


    namespace CommitCheckList
    {
        public class XmlManagerController
        {
            private static readonly string configPath = Directory.GetCurrentDirectory() + @"\ConfigFile.xml";
            private static readonly string checkListPathFromConfig = Settings.Default.CheckListPath;
            private static readonly string usageXmlPath = Settings.Default.UsersCommitsPath;
            private static readonly string checkListGroupXmlPath = Settings.Default.CheckListGroupPath;
            private static readonly string defaultCheckListXmlPath = checkListGroupXmlPath + @"\DefaultCheckList.xml";
            private static readonly string excelQuestionsXmlPath = Settings.Default.ImpactAnalysisConfig;

            public static string GetDeveloperName()
            {
                var questList = new Dictionary<int, string>();
                XmlDocument xml = new XmlDocument();
                xml.Load(usageXmlPath);
                var xmlCode1s = xml.SelectNodes("/Developers/Developer/Code1");
                foreach (var xmlCode1 in xmlCode1s)
                {
                    var code1Node = (xmlCode1 as XmlNode);
                    if (code1Node!= null)
                    {
                        if(code1Node.InnerText.Equals(Environment.UserName))
                        {
                            var developerName = (xmlCode1 as XmlNode)?.NextSibling;
                            if(developerName != null)
                            {
                                return developerName.InnerText;                            }
                        }
                    }
                }

                xml.Save(usageXmlPath);
                return String.Empty;
            }



            public static string GetIpAddress()
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                return String.Empty;
            }

            public static Dictionary<int, string> GetExcelQuestions()
            {
                var questList = new Dictionary<int, string>();
                XmlDocument xml = new XmlDocument();
                xml.Load(excelQuestionsXmlPath);
                var xmlExcelQuestons = xml.SelectNodes("/Questions/Question/QuestionDescription");
                foreach (var xmlExcelQueston in xmlExcelQuestons)
                {
                    var columnNumber = (xmlExcelQueston as XmlNode)?.NextSibling;
                    if (columnNumber != null)
                    {
                        if (!questList.ContainsKey(int.Parse(columnNumber.InnerText)))
                        {
                            questList.Add((columnNumber.InnerText != String.Empty) ? int.Parse(columnNumber.InnerText) : 0, (xmlExcelQueston as XmlNode)?.InnerText);
                        }
                    }
                }

                xml.Save(excelQuestionsXmlPath);
                return questList;
            }

            public static List<string> GetQuestions()
            {
                var questList = new List<string>();
                foreach (var file in Directory.GetFiles(checkListGroupXmlPath))
                {
                    string quest;
                    XmlDocument xml = new XmlDocument();
                    xml.Load(file);
                    XmlNode xmlportalRun = xml.SelectSingleNode("/GroupChecks/Question");
                    if (xmlportalRun != null)
                    {
                        quest = xmlportalRun.InnerText;
                        if (quest != String.Empty)
                            questList.Add(quest);
                    }
                    xml.Save(file);
                }
                return questList;
            }
            public static Dictionary<string, bool> dictionaryChecks = new Dictionary<string, bool>();

            public static List<CheckModel> GetChecks()
            {
                var checkList = new List<CheckModel>();
                foreach (var file in Directory.GetFiles(checkListGroupXmlPath))
                {
                    string quest = null;
                    XmlDocument xml = new XmlDocument();
                    xml.Load(file);
                    XmlNode xmlportalRun = xml.SelectSingleNode("/GroupChecks/Question");
                    if (xmlportalRun != null)
                    {
                        quest = xmlportalRun.InnerText;
                    }
                    bool questIn;


                    if ((dictionaryChecks.TryGetValue(quest, out questIn) && questIn) || (file == defaultCheckListXmlPath))
                    {
                        var matchingElements = xml.SelectNodes("/GroupChecks/Checks/Check/CheckDescription");
                        if (matchingElements != null)
                        {
                            foreach (var matchingElement in matchingElements)
                            {
                                CheckModel check = new CheckModel();
                                if (matchingElement != null)
                                {
                                    check.CheckDescription = (matchingElement as XmlNode)?.InnerText;
                                    var nextSibling = (matchingElement as XmlNode)?.NextSibling;
                                    if (nextSibling != null)
                                    {
                                        check.CheckToolTip = nextSibling?.InnerText;
                                    }

                                    if (check.CheckDescription != String.Empty)
                                    {
                                        check.CheckFilePath = file;
                                        checkList.Add(check);
                                    }
                                }
                            }
                        }
                    }
                    xml.Save(file);
                }
                return checkList;
            }

            //Get data Path
            public static string GetCheckListPath()
            {
                string checkListPath = String.Empty;
                XmlDocument xml = new XmlDocument();
                xml.Load(configPath);
                XmlNode xmlportalRun = xml.SelectSingleNode("/Config/CheckListPath");
                if (xmlportalRun != null)
                {
                    checkListPath = xmlportalRun.InnerText;
                }
                return checkListPath;
            }

            public static string GetCheckListGroupPath()
            {
                string checkListGroupPath = String.Empty;
                XmlDocument xml = new XmlDocument();
                xml.Load(configPath);
                XmlNode xmlportalRun = xml.SelectSingleNode("/Config/CheckListGroupPath");
                if (xmlportalRun != null)
                {
                    checkListGroupPath = xmlportalRun.InnerText;
                }
                return checkListGroupPath;
            }

            public static string GetUsersCommitsPath()
            {
                string usersCommitsPath = String.Empty;
                XmlDocument xml = new XmlDocument();
                xml.Load(configPath);
                XmlNode xmlportalRun = xml.SelectSingleNode("/Config/UsersCommitsPath");
                if (xmlportalRun != null)
                {
                    usersCommitsPath = xmlportalRun.InnerText;
                }
                return usersCommitsPath;
            }

            public static void SetCheckResult(CheckModel checkModel)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(checkModel.CheckFilePath);
                XmlNode xmlPortalRun = xml.SelectSingleNode("/GroupChecks/Checks/Check[CheckDescription ='" + checkModel.CheckDescription + "']/CoveredCount");
                if (checkModel.IsDoneCheckBox)
                {
                    xmlPortalRun = xml.SelectSingleNode("/GroupChecks/Checks/Check[CheckDescription ='" + checkModel.CheckDescription + "']/CoveredCount");
                }
                else
                {
                    xmlPortalRun = xml.SelectSingleNode("/GroupChecks/Checks/Check[CheckDescription ='" + checkModel.CheckDescription + "']/NotRelevantCount");

                }

                if (xmlPortalRun != null)
                    xmlPortalRun.InnerText = (int.Parse(xmlPortalRun.InnerText) + 1).ToString();
                xml.Save(checkModel.CheckFilePath);
            }
        }
    }

}

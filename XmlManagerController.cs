using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using CheckListToolWPF.Properties;
using CheckListToolWPF.Model;
using System.Windows;

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
                try
                {
                    var questList = new Dictionary<int, string>();
                    XmlDocument xml = new XmlDocument();
                    xml.Load(usageXmlPath);
                    var xmlCode1s = xml.SelectNodes("/Developers/Developer/Code1");
                    foreach (var xmlCode1 in xmlCode1s)
                    {
                        var code1Node = (xmlCode1 as XmlNode);
                        if (code1Node != null)
                        {
                            if (code1Node.InnerText.Equals(Environment.UserName))
                            {
                                var developerName = (xmlCode1 as XmlNode)?.NextSibling;
                                if (developerName != null)
                                {
                                    return developerName.InnerText;
                                }
                            }
                        }
                    }

                    xml.Save(usageXmlPath);
                }
                catch(Exception e)
                {
                    Log.Write("Error was catched in GetDeveloperName(): " + e);
                    MessageBox.Show("Something went wrong, take a look at the log!");
                    Application.Current.Shutdown();
                }
                return String.Empty;
            }

            public static Dictionary<int, string> GetExcelQuestions()
            {
                var questList = new Dictionary<int, string>();
                try
                {
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
                }
                catch(Exception e)
                {
                    Log.Write("Error was catched in GetExcelQuestions(): " + e);
                    MessageBox.Show("Something went wrong, take a look at the log!");
                    Application.Current.Shutdown();
                }
                return questList;
            }

            public static List<string> GetQuestions()
            {
                var questList = new List<string>();
                try
                { 
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
                }
                catch (Exception e)
                {
                    Log.Write("Error was catched in GetQuestions(): " + e);
                    MessageBox.Show("Something went wrong, take a look at the log!");
                    Application.Current.Shutdown();
                }
                return questList;
            }
            public static Dictionary<string, bool> dictionaryChecks = new Dictionary<string, bool>();

            public static List<CheckModel> GetChecks()
            {
                var checkList = new List<CheckModel>();
                try
                {
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
                }
                catch (Exception e)
                {
                    Log.Write("Error was catched in GetChecks(): " + e);
                    MessageBox.Show("Something went wrong, take a look at the log!");
                    Application.Current.Shutdown();
                }
                return checkList;
            }

            public static void SetCheckResult(CheckModel checkModel)
            {
                try
                {

                    XmlDocument xml = new XmlDocument();

                    if (IsFileReady(checkModel.CheckFilePath))
                    {
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
                catch(Exception e)
                {
                    Log.Write("Error was catched in SetCheckResult(): " + e);
                    MessageBox.Show("Something went wrong, take a look at the log!");
                    Application.Current.Shutdown();
                }
            }

            public static bool FileIsReady(string filePath)
            {
                //This will lock the execution until the file is ready
                //TODO: Add some logic to make it async and cancelable
                while (!IsFileReady(filePath)) { }

                return true;
            }

            public static bool IsFileReady(string filePath)
            {
                // If the file can be opened for exclusive access it means that the file
                // is no longer locked by another process.
                try
                {
                    using (FileStream inputStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                        return inputStream.Length > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }

}

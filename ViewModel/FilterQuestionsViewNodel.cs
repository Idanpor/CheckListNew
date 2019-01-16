using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CheckListToolWPF.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using CheckListToolWPF.CommitCheckList;
using CheckListToolWPF.Properties;
using System.Threading;

namespace CheckListToolWPF.ViewModel
{
    public class FilterQuestionsViewNodel : NotifyPropertyChange
    {
        public ObservableCollection<QuestionModel> QuestionList { get; set; }

        public ObservableCollection<CheckModel> CheckList { get; set; }

        public ObservableCollection<QuestionForExcel> ImpactList { get; set; }

        public DelegateCommand ContinueCommand { get; private set; }

        public DelegateCommand BackCommand { get; private set; }

        public DelegateCommand FinishCommand { get; private set; }

        public DelegateCommand CancelCommand { get; private set; }

        private int formHeight;
        public int FormHeight
        {
            get
            {
                return formHeight;
            }

            set
            {
                formHeight = value;
                OnPropertyChanged();
            }
        }

        private bool canContinue;
        public bool CanContinue
        {
            get
            {
                return canContinue;
            }

            set
            {
                canContinue = value;
                OnPropertyChanged();
            }
        }

        public bool CanBack { get { return !canContinue; } }

        

        private string workItem;
        public string WorkItem
        {
            get
            {
                return workItem;
            }

            set
            {
                workItem = value;
                OnPropertyChanged();
            }
        }

        private string title;
        public string Title
        {
            get
            {
                return "Checklist & Impact Analysis Tool - " + "Version "  + Settings.Default.Version;
            }
        }

        private string iconPath;
        public string IconPath
        {
            get
            {
                return Settings.Default.IconPath;
            }

            set
            {
                iconPath = value;
                OnPropertyChanged();
            }
        }

        private string leftArrow;
        public string LeftArrow
        {
            get
            {
                return Settings.Default.Left_Arrow;
            }

            set
            {
                leftArrow = value;
                OnPropertyChanged();
            }
        }

        private string rightArrow;
        public string RightArrow
        {
            get
            {
                return Settings.Default.Right_Arrow;
            }

            set
            {
                rightArrow = value;
                OnPropertyChanged();
            }
        }

        public FilterQuestionsViewNodel(List<QuestionModel> questionModelList, List<QuestionForExcel> impactModelList)
        {
            FormHeight = 700;
            QuestionList =
  new ObservableCollection<QuestionModel>();

            questionModelList.ForEach(e => QuestionList.Add(e));

            ImpactList =
new ObservableCollection<QuestionForExcel>();

            impactModelList.ForEach(e => ImpactList.Add(e));

            CheckList = new ObservableCollection<CheckModel>();
            canContinue = true;
            ContinueCommand = new DelegateCommand(Continue, CanContinue);
            BackCommand = new DelegateCommand(Back, CanBack);
            FinishCommand = new DelegateCommand(Finish);
            CancelCommand = new DelegateCommand(Cancel);
        }

        private void Cancel()
        {
            Microsoft.Shell.SingleInstance<App>.Cleanup();
            System.Windows.Application.Current.Shutdown();
        }

        public delegate void ImpactInExcel();
        private void Finish()
        {
            ImpactInExcel caller = new ImpactInExcel(SeTImpactInExcel);

            caller.BeginInvoke(null, null);

            if (SetCheckResults())
            {
                MessageBox.Show("Now your commit will be safer!");
                SpinWait.SpinUntil(() => Finished);
                System.Windows.Application.Current.Shutdown();
            };
        }

        private void Back()
        {
            FormHeight = 700;
            CanContinue = true;
        }

        private void Continue()
        {
            int count = 0;
            var checklistModel = SetCheckList();

            if (checklistModel == null)
            {
                return;
            }

            if (WorkItem == null || WorkItem == String.Empty || WorkItem.Length != 6)
            {
                MessageBox.Show("Please enter correct (6 digits) workItem Id!");
                return;
            }

            var checklistCopy = new List<CheckModel>(CheckList);
            CheckList.Clear();

            checklistModel.ForEach(check =>
            {
                    check.GroupNameDone = "GroupNameDone" + count;
                    check.GroupNameNoRelevant = "GroupNameNoRelevant" + count;
                    var existCheck = checklistCopy.FirstOrDefault(c => c.CheckDescription.Equals(check.CheckDescription));
                    check.IsDoneCheckBox = (existCheck != null) ? existCheck.IsDoneCheckBox : false;
                    count++;
                    CheckList.Add(check);
            });

            FormHeight = 460;

            CanContinue = false;
        }

        internal List<CheckModel> SetCheckList()
        {
            Dictionary<string, bool> dic = new Dictionary<string, bool>();
            foreach (QuestionModel quest in QuestionList)
            {
                string currentQuest = (quest as QuestionModel).Question;
                if (!dic.ContainsKey(currentQuest))
                {
                    dic.Add(currentQuest ?? throw new InvalidOperationException(), (quest as QuestionModel).IsRelevantQuestion);
                }
            }
            XmlManagerController.dictionaryChecks = dic;

            return XmlManagerController.GetChecks();
        }

        public bool Finished{get;set;}

        internal void SeTImpactInExcel()
        {
            if (!ExcelManagerController.IsFileLocked())
            {
                ExcelManagerController.OpenAndSet(1, DateTime.Now.ToShortDateString());
                ExcelManagerController.OpenAndSet(2, XmlManagerController.GetDeveloperName());
                ExcelManagerController.OpenAndSet(3, WorkItem);
                foreach (var quest in ImpactList)
                {
                    ExcelManagerController.OpenAndSet(quest.ExcelColumnNumber, quest.QuestionResult == QuestResult.Yes ? "Y" : "N");
                }
                ExcelManagerController.Close();
            }
            if (Settings.Default.SendExcelByMail)
            {
                CreateMailItem(WorkItem);
            }
            Finished = true;
        }

        private void CreateMailItem(string workItemNum)
        {
            Microsoft.Office.Interop.Outlook.Application app = new Microsoft.Office.Interop.Outlook.Application();
            Microsoft.Office.Interop.Outlook.MailItem mailItem = app.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
            mailItem.Subject = "Impact Analysis - New Item wad added: " + workItemNum;
            mailItem.To = Settings.Default.MailRecipients;
            mailItem.Body = "WorkItem number : " + workItem + " was added!";
            mailItem.Attachments.Add(Settings.Default.ImpactAnalysisExcelPath);
            mailItem.Display(false);
            mailItem.Send();
        }

        internal bool SetCheckResults()
        {
            foreach(CheckModel check in CheckList)
            {
                XmlManagerController.SetCheckResult(check);
            }

            return true;
        }
    }
}

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

namespace CheckListToolWPF.ViewModel
{
    public class FilterQuestionsViewNodel : NotifyPropertyChange
    {
        public ObservableCollection<QuestionModel> QuestionList { get; set; }

        public ObservableCollection<CheckModel> CheckList { get; set; }

        public ObservableCollection<QuestionForExcel> ImpactList { get; set; }

        public DelegateCommand SubmitCommand { get; private set; }

       

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

        private string iconPath;
        public string IconPath
        {
            get
            {
                return Settings.Default.IconPath;
            }

            set
            {
                workItem = value;
                OnPropertyChanged();
            }
        }

        public FilterQuestionsViewNodel(List<QuestionModel> questionModelList, List<QuestionForExcel> impactModelList)
        {
            FormHeight = 600;
            QuestionList =
  new ObservableCollection<QuestionModel>();

            questionModelList.ForEach(e => QuestionList.Add(e));

            ImpactList =
new ObservableCollection<QuestionForExcel>();

            impactModelList.ForEach(e => ImpactList.Add(e));

            CheckList = new ObservableCollection<CheckModel>();
        }

        internal List<CheckModel> SetCheckList()
        {
            Dictionary<string, bool> dic = new Dictionary<string, bool>();
            foreach (QuestionModel quest in QuestionList)
            {
                bool isFilled = true;
                if (quest.QuestionResult == QuestResult.Nothing)
                {
                    MessageBox.Show("Please fill all the questions");
                    isFilled = false;
                    return null;
                }
                dic.Add((quest as QuestionModel)?.Question ?? throw new InvalidOperationException(), (quest as QuestionModel).IsRelevantQuestion);

            }
            XmlManagerController.dictionaryChecks = dic;



            return XmlManagerController.GetChecks();//.ForEach(e=> CheckList.Add(e));
        }

        internal bool SeTImpactInExcel()
        {
            bool isFilled = true;
            foreach (var quest in ImpactList)
            {
                if (((QuestionForExcel)quest).QuestionResult == QuestResult.Nothing)
                {
                    MessageBox.Show("Please fill all the questions");
                    isFilled = false;
                    break;
                }

            }

            if (WorkItem == null)
            {
                MessageBox.Show("Please fill the workItem Id");
                isFilled = false;
            }

            if (isFilled && !ExcelManagerController.IsFileLocked())
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
            return isFilled;
        }

        internal bool SetCheckResults()
        {
            if(CheckList.Where(e=>e.CheckResult == CheckResult.Nothing).ToList().Count > 0)
            {
                MessageBox.Show("Please fill in all the Checklist");
                return false;
            }
            foreach(CheckModel check in CheckList)
            {
                XmlManagerController.SetCheckResult(check);
            }

            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckListToolWPF.Model
{
    public enum QuestResult
    {
        Yes,
        No,
        Nothing
    }

    public class QuestionModel : NotifyPropertyChange
    {
        public bool IsYesCheckBox { get; set; }

        public bool IsNoCheckBox { get; set; }

        public QuestResult QuestionResult { get { return IsYesCheckBox ? QuestResult.Yes : IsNoCheckBox ? QuestResult.No : QuestResult.Nothing; } }

        public string Question { get; set; }

        public bool IsRelevantQuestion { get { return IsYesCheckBox; } }

        private string groupNameYes;
        public string GroupNameYes
        {
            get
            {
                return groupNameYes;
            }

            set
            {
                groupNameYes = value;
                OnPropertyChanged();
            }
        }

        private string groupNameNo;
        public string GroupNameNo
        {
            get
            {
                return groupNameNo;
            }

            set
            {
                groupNameNo = value;
                OnPropertyChanged();
            }
        }
    }
}

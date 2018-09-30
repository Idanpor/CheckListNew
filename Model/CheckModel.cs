using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckListToolWPF.Model
{
    public enum CheckResult
    {
        Done, NotRelevant, Nothing
    }

    public class CheckModel  :NotifyPropertyChange
    {
        public string CheckDescription { set; get; }
        public string CheckFilePath { set; get; }
        public string CheckToolTip { set; get; }

        private string groupNameDone;
        public string GroupNameDone
        {
            get
            {
                return groupNameDone;
            }

            set
            {
                groupNameDone = value;
                OnPropertyChanged();
            }
        }

        private string groupNameNoRelevant;
        public string GroupNameNoRelevant
        {
            get
            {
                return groupNameNoRelevant;
            }

            set
            {
                groupNameNoRelevant = value;
                OnPropertyChanged();
            }
        }

        public bool IsDoneCheckBox { get; set; }

        public bool IsNotRelevantCheckBox { get; set; }

        public CheckResult CheckResult { get { return IsDoneCheckBox ? CheckResult.Done : IsNotRelevantCheckBox ? CheckResult.NotRelevant : CheckResult.Nothing; } }

        public bool IsRelevantQuestion { get { return IsDoneCheckBox; } }
    }
}

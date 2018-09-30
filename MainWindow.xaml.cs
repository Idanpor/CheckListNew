using CheckListToolWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CheckListToolWPF.Model;
using CheckListToolWPF.CommitCheckList;

namespace CheckListToolWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            int count = 0;
            var questionModelList = new List<QuestionModel>();
            var questList = new List<string>();
            questList = XmlManagerController.GetQuestions();
            questList.ForEach(e =>
            {
                questionModelList.Add(new QuestionModel() { Question = e, GroupNameYes = "GroupYes" + count,GroupNameNo = "GroupNo" + count });
                count++;
            });

            var impactModelList = new List<QuestionForExcel>();
            var excelQuestList = XmlManagerController.GetExcelQuestions();
            foreach (var excelQuest in excelQuestList)
            {
                impactModelList.Add(new QuestionForExcel() { ExcelColumnNumber = excelQuest.Key, Question = excelQuest.Value });
            }


            DataContext = new FilterQuestionsViewNodel(questionModelList, impactModelList);
        }

        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            var checklistModel = (DataContext as FilterQuestionsViewNodel).SetCheckList();

            if(checklistModel == null || !(DataContext as FilterQuestionsViewNodel).SeTImpactInExcel())
            {
                return;
            }


            checklistModel.ForEach(check =>
            {
                check.GroupNameDone = "GroupNameDone" + count;
                check.GroupNameNoRelevant = "GroupNameNoRelevant" + count;
                count++;
                (DataContext as FilterQuestionsViewNodel).CheckList.Add(check);
            });

            

            questlist.Visibility = Visibility.Collapsed;
            checklist.Visibility = Visibility.Visible;
            submit_button.Visibility = Visibility.Collapsed;
            finish_button.Visibility = Visibility.Visible;
            impactSection.Visibility = Visibility.Collapsed;

            (DataContext as FilterQuestionsViewNodel).FormHeight = 350;
            header.Content = "                 Checklist";
        }
        private void Finish_Button_Click(object sender, RoutedEventArgs e)
        {
            if((DataContext as FilterQuestionsViewNodel).SetCheckResults())
            {
                MessageBox.Show("Now your commit will be safer!");
                System.Windows.Application.Current.Shutdown();
            }
        }
        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        

    }
}

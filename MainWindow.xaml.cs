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
using System.Threading;

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



        private void Continue_Button_Click(object sender, RoutedEventArgs e)
        {
            BackButton.Visibility = Visibility.Visible;
            ContinueButton.Visibility = Visibility.Hidden;
            (DataContext as FilterQuestionsViewNodel).CanContinue = false;
            (DataContext as FilterQuestionsViewNodel).FormHeight = 460;
            questlist.Visibility = Visibility.Collapsed;
            checklist.Visibility = Visibility.Visible;
            submit_button.Visibility = Visibility.Collapsed;
            finish_button.Visibility = Visibility.Visible;

            impactSection.Visibility = Visibility.Collapsed;

            Title1.Visibility = Visibility.Collapsed;
            Title2.Visibility = Visibility.Visible;
            sparator.Visibility = Visibility.Collapsed;
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            BackButton.Visibility = Visibility.Hidden;
            ContinueButton.Visibility = Visibility.Visible;
            (DataContext as FilterQuestionsViewNodel).CanContinue = true;
            (DataContext as FilterQuestionsViewNodel).FormHeight = 700;
            questlist.Visibility = Visibility.Visible;
            checklist.Visibility = Visibility.Collapsed;
            submit_button.Visibility = Visibility.Visible;
            finish_button.Visibility = Visibility.Collapsed;

            impactSection.Visibility = Visibility.Visible;

            Title1.Visibility = Visibility.Visible;
            Title2.Visibility = Visibility.Collapsed;
            sparator.Visibility = Visibility.Visible;
        }

        private void Submit_Button_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            var checklistModel = (DataContext as FilterQuestionsViewNodel).SetCheckList();

            if(checklistModel == null)
            {
                return;
            }

            if((DataContext as FilterQuestionsViewNodel).WorkItem == null || (DataContext as FilterQuestionsViewNodel).WorkItem == String.Empty)
            {
                MessageBox.Show("Please enter workItem Id");
                return;
            }

            checklistModel.ForEach(check =>
            {
                check.GroupNameDone = "GroupNameDone" + count;
                check.GroupNameNoRelevant = "GroupNameNoRelevant" + count;
                count++;
                (DataContext as FilterQuestionsViewNodel).CheckList.Add(check);
            });


            BackButton.Visibility = Visibility.Visible;
            questlist.Visibility = Visibility.Collapsed;
            checklist.Visibility = Visibility.Visible;
            submit_button.Visibility = Visibility.Collapsed;
            finish_button.Visibility = Visibility.Visible;
            impactSection.Visibility = Visibility.Collapsed;

            Title1.Visibility = Visibility.Collapsed;
            Title2.Visibility = Visibility.Visible;
            sparator.Visibility = Visibility.Collapsed;

            (DataContext as FilterQuestionsViewNodel).FormHeight = 460;

            (DataContext as FilterQuestionsViewNodel).CanContinue = false;
        }

        public delegate void ImpactInExcel();


        private void Finish_Button_Click(object sender, RoutedEventArgs e)
        {
            ImpactInExcel caller = new ImpactInExcel((DataContext as FilterQuestionsViewNodel).SeTImpactInExcel);


            caller.BeginInvoke(null,null);
            
            //(DataContext as FilterQuestionsViewNodel).SeTImpactInExcel();

            if ((DataContext as FilterQuestionsViewNodel).SetCheckResults())
            {
                MessageBox.Show("Now your commit will be safer!");
                SpinWait.SpinUntil(() => (DataContext as FilterQuestionsViewNodel).Finished);
                System.Windows.Application.Current.Shutdown();
            }
        }
        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        

    }
}

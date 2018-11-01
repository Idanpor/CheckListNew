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
         List<QuestionModel> questionModelList;
        List<QuestionForExcel> impactModelList;
        public MainWindow()
        {
            InitializeComponent();

            GetQuestionsFromXmls();

            DataContext = new FilterQuestionsViewNodel(questionModelList, impactModelList);
        }

        private void GetQuestionsFromXmls()
        {
            questionModelList = new List<QuestionModel>();
            var questList = new List<string>();
            questList = XmlManagerController.GetQuestions();
            questList.ForEach(e =>
            {
                questionModelList.Add(new QuestionModel() { Question = e });
            });

            impactModelList = new List<QuestionForExcel>();
            var excelQuestList = XmlManagerController.GetExcelQuestions();
            foreach (var excelQuest in excelQuestList)
            {
                impactModelList.Add(new QuestionForExcel() { ExcelColumnNumber = excelQuest.Key, Question = excelQuest.Value });
            }
        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        

    }
}

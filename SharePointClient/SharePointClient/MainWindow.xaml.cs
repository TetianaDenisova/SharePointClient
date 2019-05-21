using SharePointClient.Controller;
using System.Collections.ObjectModel;
using System.Windows;
namespace SharePointClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string currentListName;
        private ITaskController TaskController;
        private ObservableCollection<Task> todoList;
        public MainWindow()
        {
            InitializeComponent();

            TaskController.Login();

        }

        private void Upload_List(object sender, RoutedEventArgs e)
        {
            currentListName = ShowModalWindow("ВВедіть назву списку");
            if (currentListName == string.Empty) return;
            todoList = TaskController.DownloadList(currentListName);
            listView.ItemsSource = todoList;
            tbListName.Text = currentListName;
        }

        private static string ShowModalWindow(string info)
        {
            ModalWindow modalWindow = new ModalWindow();
            modalWindow.Info = info;
            if (modalWindow.ShowDialog() == true)
            {
                return modalWindow.Result;
            }

            return string.Empty;
        }

        private void Create_New_List_Click(object sender, RoutedEventArgs e)
        {
            currentListName = ShowModalWindow("Введіть назву нового списку");
            tbListName.Text = currentListName;
            if (currentListName == string.Empty) return;

            listView.ItemsSource = TaskController.CreateList(currentListName);
        }

        private void Create_New_Task(object sender, RoutedEventArgs e)
        {
            NewTaskWindow modalWindow = new NewTaskWindow();
            if (modalWindow.ShowDialog() == true)
            {
                listView.ItemsSource = TaskController.AddItem(modalWindow.EditTask); ;
            }
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedValue == null)
            {
                MessageBox.Show("Please, select item to edit");
                return;
            }
            NewTaskWindow modalWindow = new NewTaskWindow();
            var selectedTask = (Task)listView.SelectedItem;
            modalWindow.EditTask = selectedTask;
            if (modalWindow.ShowDialog() == true)
            {
                listView.ItemsSource = TaskController.UpdateItem(selectedTask.Title, modalWindow.EditTask, selectedTask.Id);
            }
        }

        private void Remove_Task_Click(object sender, RoutedEventArgs e)
        {
            var selectedTask = (Task)listView.SelectedItem;
            TaskController.RemoveItem(selectedTask.Id);
        }
    }
}


using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
namespace SharePointClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string currentListName;
        private ObservableCollection<Task> todoList;
        public ObservableCollection<Task> TodoList
        {
            get
            {
                if (todoList == null) { todoList = new ObservableCollection<Task>(); }
                return todoList;
            }
            set
            {
                todoList = value;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            SharePointService.Login();

        }

        private void Upload_List(object sender, RoutedEventArgs e)
        {
            currentListName = ShowModalWindow("ВВедіть назву списку");
            if (currentListName == string.Empty) return;
            todoList = SharePointService.UploadList(currentListName);
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

            SharePointService.CreateList(currentListName);
            listView.ItemsSource = TodoList;
        }

        private void Create_New_Task(object sender, RoutedEventArgs e)
        {
            NewTaskWindow modalWindow = new NewTaskWindow();
            if (modalWindow.ShowDialog() == true)
            {
                SharePointService.AddItem(modalWindow.EditTask);
                TodoList.Add(modalWindow.EditTask);
                listView.ItemsSource = TodoList;
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
                SharePointService.UpdateListItem(selectedTask.Title, modalWindow.EditTask);
                var oldTask = TodoList.Single(x => x.Id == selectedTask.Id);
                var index = TodoList.IndexOf(oldTask);
                TodoList[index] = modalWindow.EditTask;
                listView.ItemsSource = TodoList;
            }
        }

        private void Remove_Task_Click(object sender, RoutedEventArgs e)
        {
            var selectedTask = (Task)listView.SelectedItem;
            var oldTask = TodoList.Single(x => x.Id == selectedTask.Id);
            var index = TodoList.IndexOf(oldTask);
            TodoList.RemoveAt(index);
            SharePointService.RemoveItem(index);
        }
    }
}


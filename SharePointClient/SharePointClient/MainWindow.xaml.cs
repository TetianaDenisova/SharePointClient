using Microsoft.SharePoint.Client;
using System;
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
        private ClientContext context;
        private string currentListName;
        private Web web;
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
            Login();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            currentListName = ShowModalWindow("ВВедіть назву списку");
            if (currentListName == string.Empty) return;
            todoList = GetAllTasks(currentListName);
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

        public ObservableCollection<Task> GetAllTasks(string todoListName)
        {
            tbListName.Text = todoListName;
            string xmlQuery = "<View><RowLimit>100</RowLimit></View>";
            List oList = context.Web.Lists.GetByTitle(todoListName);
            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = xmlQuery;
            var items = oList.GetItems(camlQuery);
            context.Load(items);
            context.ExecuteQuery();
            var tasks = (items.ToList()
                .Select(task => new Task
                (task["Title"].ToString(),
                 task["Status"].ToString(),
                 task["Priority"].ToString(),
                 double.Parse(task["PercentComplete"].ToString()),
                 task["Description"].ToString(),
                 DateTime.Parse(task["DueDate"].ToString())
                 ))
                .ToList());
            return new ObservableCollection<Task>(tasks);
        }

        private void AddItem(Task newTask)
        {
            List todoList = context.Web.Lists.GetByTitle(currentListName);
            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
            ListItem newItem = todoList.AddItem(itemCreateInfo);
            newItem["Title"] = newTask.Title;
            newItem["Status"] = newTask.Status;
            newItem["Priority"] = newTask.Priority;
            newItem["DueDate"] = newTask.DueDate;
            newItem["PercentComplete"] = newTask.PercentComplete;
            newItem["Description"] = newTask.Description;
            newItem.Update();
            context.ExecuteQuery();
            TodoList.Add(newTask);
        }

        private void UpdateListItem(string taskName, Task newTask)
        {
            string xmlQuery = "<View><RowLimit>100</RowLimit></View>";
            List oList = context.Web.Lists.GetByTitle(currentListName);
            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = xmlQuery;

            ListItemCollection collListItem = oList.GetItems(camlQuery);

            context.Load(collListItem,
                items => items.Where(
                    list => list.DisplayName == taskName).Include(
                    item => item.Id));

            context.ExecuteQuery();
            if (collListItem.Count > 0)
            {
                var idForUpdate = collListItem[0].Id;
                ListItem oldItem = oList.GetItemById(idForUpdate);

                oldItem["Title"] = newTask.Title;
                oldItem["Status"] = newTask.Status;
                oldItem["Priority"] = newTask.Priority;
                oldItem["DueDate"] = newTask.DueDate;
                oldItem["PercentComplete"] = newTask.PercentComplete;
                oldItem["Description"] = newTask.Description;
                oldItem.Update();

                context.ExecuteQuery();
            }
        }

        private void CreateList(string listName)
        {
            ListCreationInformation creationInfo = new ListCreationInformation();
            creationInfo.Title = listName;
            creationInfo.TemplateType = (int)ListTemplateType.Tasks;
            List list = web.Lists.Add(creationInfo);


            context.ExecuteQuery();
            list.Update();

            Field field = list.Fields.AddFieldAsXml("<Field DisplayName='Description' Type='Text' />",
                                                       true,
                                                       AddFieldOptions.DefaultValue);
            context.ExecuteQuery();
        }
        private void DeleteList(string listName)
        {
            List list = web.Lists.GetByTitle(listName);
            list.DeleteObject();

            context.ExecuteQuery();
        }
        private void UpdateTitle(string newTitle)
        {
            web.Title = newTitle;
            web.Update();
            context.ExecuteQuery();
        }

        private void GetTitle()
        {
            context.Load(web, w => w.Title, w => w.Description, w => w.Fields);
            context.ExecuteQuery();
        }

        private void Login()
        {
            context = new ClientContext("https://skybowdev.sharepoint.com/sites/dev_tko");

            context.Credentials = new SharePointOnlineCredentials("tetiana.kozlovska@skybow.com", pbox1.SecurePassword.Copy());
            web = context.Web;
            context.Load(web);
            context.ExecuteQuery();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            currentListName = ShowModalWindow("Введіть назву нового списку");

            if (currentListName == string.Empty) return;

            CreateList(currentListName);
            var list = GetAllTasks(currentListName);
            listView.ItemsSource = list;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            NewTaskWindow modalWindow = new NewTaskWindow();
            if (modalWindow.ShowDialog() == true)
            {
                AddItem(modalWindow.EditTask);
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
                UpdateListItem(selectedTask.Title, modalWindow.EditTask);
                var oldTask = TodoList.Single(x => x.Id == selectedTask.Id);
                var index = TodoList.IndexOf(oldTask);
                TodoList[index] = modalWindow.EditTask;
                listView.ItemsSource = TodoList;
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var selectedTask = (Task)listView.SelectedItem;
            var oldTask = TodoList.Single(x => x.Id == selectedTask.Id);
            var index = TodoList.IndexOf(oldTask);
            TodoList.RemoveAt(index);
            List todoList = context.Web.Lists.GetByTitle(currentListName);
            ListItem listItem = todoList.GetItemById(index + 1);
            listItem.DeleteObject();

            context.ExecuteQuery();
        }
    }
}


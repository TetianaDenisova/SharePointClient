using Microsoft.SharePoint.Client;
using System;
using System.Windows;

namespace SharePointClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClientContext context;
        private Web web;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Login();
            var myList = "My Todo list";
            AddItem(myList, "Buy popcorn", "Completed", "High", DateTime.Now, 1);

            context.ExecuteQuery();
        }

        private void AddItem(string todoListName, string title, string status, string priority, DateTime dueDate, int percentComplete)
        {
            List todoList = context.Web.Lists.GetByTitle(todoListName);
            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
            ListItem newItem = todoList.AddItem(itemCreateInfo);
            newItem["Title"] = title;
            newItem["Status"] = status;
            newItem["Priority"] = priority;
            newItem["DueDate"] = dueDate;
            newItem["PercentComplete"] = percentComplete;
            newItem.Update();
        }

        private void CreateList(string listName)
        {
            ListCreationInformation creationInfo = new ListCreationInformation();
            creationInfo.Title = listName;
            creationInfo.TemplateType = (int)ListTemplateType.Tasks;
            List list = web.Lists.Add(creationInfo);

            list.Update();
            context.ExecuteQuery();
        }
        private void DeleteList(string listName)
        {
            List list = web.Lists.GetByTitle(listName);
            list.DeleteObject();

            context.ExecuteQuery();
        }
        private void UpdateTitle()
        {
            web.Title = "HelloTania";
            web.Update();
            context.ExecuteQuery();
        }

        private void GetTitle()
        {
            context.Load(web, w => w.Title, w => w.Description, w => w.Fields);
            context.ExecuteQuery();
            tbFullInfo.Text = web.Title;
        }

        private void Login()
        {
            context = new ClientContext("https://m365x460933.sharepoint.com/sites/TestTodoList");

            context.Credentials = new SharePointOnlineCredentials("admin@m365x460933.onmicrosoft.com", pbox1.SecurePassword.Copy());
            web = context.Web;
            context.Load(web);
            context.ExecuteQuery();
            label1.Content = web.Title;
        }
    }
}

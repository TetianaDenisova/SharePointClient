using Microsoft.SharePoint.Client;
using System;
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
        private Web web;
        public MainWindow()
        {
            InitializeComponent();
            Login();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            var myList = "My Todo list";
            var taskName = "Buy popcorn";
            var task = new Task("Buy dollars", "Completed", "High", 0.2, DateTime.Now);
            UpdateListItem(myList, taskName, task);
            context.ExecuteQuery();
        }

        private void AddItem(string todoListName, Task newTask)
        {
            List todoList = context.Web.Lists.GetByTitle(todoListName);
            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
            ListItem newItem = todoList.AddItem(itemCreateInfo);
            newItem["Title"] = newTask.Title;
            newItem["Status"] = newTask.Status;
            newItem["Priority"] = newTask.Priority;
            newItem["DueDate"] = newTask.DueDate;
            newItem["PercentComplete"] = newTask.PercentComplete;
            newItem.Update();
        }

        private void UpdateListItem(string todoListName, string taskName, Task newTask)
        {
            string xmlQuery = "<View><RowLimit>100</RowLimit></View>";
            List oList = context.Web.Lists.GetByTitle(todoListName);
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
        }

        private void Login()
        {
            context = new ClientContext("https://skybowdev.sharepoint.com/sites/dev_tko");

            context.Credentials = new SharePointOnlineCredentials("tetiana.kozlovska@skybow.com", pbox1.SecurePassword.Copy());
            web = context.Web;
            context.Load(web);
            context.ExecuteQuery();
        }
    }
}

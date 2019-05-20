using Microsoft.SharePoint.Client;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;

namespace SharePointClient
{
    public static class SharePointService
    {
        private static ClientContext context;
        private static string currentListName;
        private static Web web;
        public static ObservableCollection<Task> UploadList(string todoListName)
        {
            currentListName = todoListName;
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
                 task["Description"] != null ? task["Description"].ToString() : string.Empty,
                 DateTime.Parse(task["DueDate"].ToString())
                 ))
                .ToList());
            return new ObservableCollection<Task>(tasks);
        }
        public static void AddItem(Task newTask)
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
            //  TodoList.Add(newTask);
        }
        public static void UpdateListItem(string taskName, Task newTask)
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
        public static void CreateList(string listName)
        {
            currentListName = listName;
            ListCreationInformation creationInfo = new ListCreationInformation();
            creationInfo.Title = listName;
            creationInfo.TemplateType = (int)ListTemplateType.Tasks;
            List list = web.Lists.Add(creationInfo);


            context.ExecuteQuery();
            list.Update();

            Field field = list.Fields.AddFieldAsXml("<Field DisplayName='Description' Type='Text' InternalName='Description' />",
                                                       true,
                                                       AddFieldOptions.DefaultValue);
            context.ExecuteQuery();
        }
        public static void DeleteList(string listName)
        {
            List list = web.Lists.GetByTitle(listName);
            list.DeleteObject();

            context.ExecuteQuery();
        }
        public static void UpdateTitle(string newTitle)
        {
            web.Title = newTitle;
            web.Update();
            context.ExecuteQuery();
        }
        public static void GetTitle()
        {
            context.Load(web, w => w.Title, w => w.Description, w => w.Fields);
            context.ExecuteQuery();
        }
        public static void Login()
        {
            context = new ClientContext("https://skybowdev.sharepoint.com/sites/dev_tko");

            context.Credentials = new SharePointOnlineCredentials("tetiana.kozlovska@skybow.com", new NetworkCredential("", "Semen07081977@").SecurePassword);
            web = context.Web;
            context.Load(web);
            context.ExecuteQuery();
        }
        public static void RemoveItem(int index)
        {
            List todoList = context.Web.Lists.GetByTitle(currentListName);
            ListItem listItem = todoList.GetItemById(index + 1);
            listItem.DeleteObject();

            context.ExecuteQuery();
        }
    }
}

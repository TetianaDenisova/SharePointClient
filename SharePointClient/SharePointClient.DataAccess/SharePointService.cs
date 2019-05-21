using Microsoft.SharePoint.Client;
using SharePointClient.DataAccess;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;

namespace SharePointClient
{
    public class SharePointService : IDataService
    {
        private ClientContext context;
        private string currentListName;
        private Web web;
        ICredentials credentials;
        ClientContext clientContext;
        IOutput output;
        public SharePointService(ICredentials credentials, string siteUrl, IOutput output)
        {
            this.credentials = credentials;
            this.clientContext = new ClientContext(siteUrl);
            this.output = output;
        }

        public ObservableCollection<Task> UploadList(string todoListName)
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

        public void AddItem(Task newTask)
        {
            List todoList = context.Web.Lists.GetByTitle(currentListName);
            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
            ListItem newItem = todoList.AddItem(itemCreateInfo);
            GetListItem(newTask, ref newItem);
            newItem.Update();
            context.ExecuteQuery();
        }

        private void GetListItem(Task newTask, ref ListItem newItem)
        {
            newItem[nameof(newTask.Title)] = newTask.Title;
            newItem[nameof(newTask.Status)] = newTask.Status;
            newItem[nameof(newTask.Priority)] = newTask.Priority;
            newItem[nameof(newTask.DueDate)] = newTask.DueDate;
            newItem[nameof(newTask.PercentComplete)] = newTask.PercentComplete;
            newItem[nameof(newTask.Description)] = newTask.Description;
        }

        public void UpdateListItem(string taskName, Task newTask)
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
                GetListItem(newTask, ref oldItem);
                oldItem.Update();

                context.ExecuteQuery();
            }
        }
        public void CreateList(string listName)
        {
            try
            {
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
                currentListName = listName;
            }
            catch (Exception)
            {
                output.Show("List already exists!");
                return;
            }
        }
        public void DeleteList(string listName)
        {
            List list = web.Lists.GetByTitle(listName);
            list.DeleteObject();

            context.ExecuteQuery();
        }
        public void UpdateTitle(string newTitle)
        {
            web.Title = newTitle;
            web.Update();
            context.ExecuteQuery();
        }
        public void GetTitle()
        {
            context.Load(web, w => w.Title, w => w.Description, w => w.Fields);
            context.ExecuteQuery();
        }
        public void Login()
        {
            context = clientContext;
            context.Credentials = credentials;
            web = context.Web;
            context.Load(web);
            context.ExecuteQuery();
        }
        public void RemoveItem(int index)
        {
            List todoList = context.Web.Lists.GetByTitle(currentListName);
            ListItem listItem = todoList.GetItemById(index + 1);
            listItem.DeleteObject();

            context.ExecuteQuery();
        }
    }
}

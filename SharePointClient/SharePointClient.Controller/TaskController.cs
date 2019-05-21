using SharePointClient.DataAccess;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace SharePointClient.Controller
{
    public class TaskController : ITaskController
    {
        private ObservableCollection<Task> todoList = new ObservableCollection<Task>();
        IDataService service;
        public TaskController(IDataService service)
        {
            this.service = service;
        }
        public ObservableCollection<Task> DownloadList(string currentListName)
        {
            return service.UploadList(currentListName);
        }

        public void Login()
        {
            service.Login();
        }

        public ObservableCollection<Task> CreateList(string listName)
        {
            service.CreateList(listName);
            return todoList;
        }

        public ObservableCollection<Task> AddItem(Task newTask)
        {
            service.AddItem(newTask);
            todoList.Add(newTask);
            return todoList;
        }
        public ObservableCollection<Task> UpdateItem(string Title, Task newTask, Guid selectedTaskId)
        {
            service.UpdateListItem(Title, newTask);
            var oldTask = todoList.Single(x => x.Id == selectedTaskId);
            var index = todoList.IndexOf(oldTask);
            todoList[index] = newTask;
            return todoList;
        }

        public ObservableCollection<Task> RemoveItem(Guid selectedTaskId)
        {
            var oldTask = todoList.Single(x => x.Id == selectedTaskId);
            var index = todoList.IndexOf(oldTask);
            todoList.RemoveAt(index);
            service.RemoveItem(index);
            return todoList;
        }
    }
}

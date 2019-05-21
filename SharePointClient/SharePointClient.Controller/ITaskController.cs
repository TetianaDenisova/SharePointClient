using System;
using System.Collections.ObjectModel;

namespace SharePointClient.Controller
{
    public interface ITaskController
    {
        ObservableCollection<Task> AddItem(Task newTask);
        ObservableCollection<Task> CreateList(string listName);
        ObservableCollection<Task> DownloadList(string currentListName);
        void Login();
        ObservableCollection<Task> RemoveItem(Guid selectedTaskId);
        ObservableCollection<Task> UpdateItem(string Title, Task newTask, Guid selectedTaskId);
    }
}
using System.Collections.ObjectModel;

namespace SharePointClient.DataAccess
{
    public interface IDataService
    {
        ObservableCollection<Task> UploadList(string todoListName);
        void AddItem(Task newTask);
        void UpdateListItem(string taskName, Task newTask);
        void CreateList(string listName);
        void DeleteList(string listName);
        void UpdateTitle(string newTitle);
        void GetTitle();
        void Login();
        void RemoveItem(int index);
    }
}

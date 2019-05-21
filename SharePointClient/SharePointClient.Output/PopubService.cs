using System.Windows;

namespace SharePointClient.DataAccess
{
    public class PopubService : IOutput
    {
        public void Show(string msg)
        {
            MessageBox.Show(msg);
        }
    }
}

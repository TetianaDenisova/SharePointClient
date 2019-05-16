using Microsoft.SharePoint.Client;
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
            GetTitle();
            var myList = "My List";

        }

        private void CreateList(string listName)
        {
            ListCreationInformation creationInfo = new ListCreationInformation();
            creationInfo.Title = listName;
            creationInfo.TemplateType = (int)ListTemplateType.Announcements;
            List list = web.Lists.Add(creationInfo);
            list.Description = "New Description";

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
            context = new ClientContext("https://skybow.sharepoint.com/sites/TestTeamSite");

            context.Credentials = new SharePointOnlineCredentials("tetiana.kozlovska@skybow.com", pbox1.SecurePassword.Copy());
            web = context.Web;
            context.Load(web);
            context.ExecuteQuery();
            label1.Content = web.Title;
        }
    }
}

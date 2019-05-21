using Microsoft.SharePoint.Client;
using Ninject.Modules;
using SharePointClient.DataAccess;
using System.Configuration;
using System.Net;

namespace SharePointClient
{
    class Bindings : NinjectModule
    {
        public override void Load()
        {
            var siteUrl = ConfigurationManager.AppSettings["siteUrl"];
            var usrname = ConfigurationManager.AppSettings["usrname"];
            var pass = ConfigurationManager.AppSettings["pass"];
            Bind<ICredentials>().ToConstant(new SharePointOnlineCredentials(usrname, new NetworkCredential(string.Empty, pass).SecurePassword));
            Bind<ClientContext>().ToConstant(new ClientContext(siteUrl));
            //Bind<IOutput>().To<PopubService>();
            Bind<IDataService>().To<SharePointService>().WithConstructorArgument(typeof(string), siteUrl); ;
        }
    }
}

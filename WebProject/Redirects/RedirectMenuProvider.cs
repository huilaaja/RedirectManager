using System.Collections.Generic;
using EPiServer.Security;
using EPiServer.Shell.Navigation;

namespace WebProject.Redirects
{
    [MenuProvider]
    public class RedirectMenuProvider : IMenuProvider
    {
        public IEnumerable<MenuItem> GetMenuItems()
        {
            return new MenuItem[] 
            {
                new UrlMenuItem("Redirect Manager", "/global/cms/redirects", "/Views/Admin/RedirectManager.cshtml")
                {
                    IsAvailable = context => PrincipalInfo.HasAdminAccess
                }
            };
        }
    }
}
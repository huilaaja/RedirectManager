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
                new UrlMenuItem("Redirects", "/global/cms/redirects", "/Admin/RedirectManager")
                {
                    IsAvailable = context => PrincipalInfo.HasAdminAccess
                }
            };
        }
    }
}
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using System.Linq;

namespace WebProject.Redirects
{
    public static class RedirectKeeper
    {
        public static void Page_Moving(object sender, ContentEventArgs e)
        {
            if (!(e.Content is PageData))
                return;

            if (e.TargetLink == ContentReference.WasteBasket || DataFactory.Instance.GetPage(e.ContentLink.ToPageReference()).IsDeleted)
                return;

            PageDataCollection pages = DataFactory.Instance.GetLanguageBranches(e.ContentLink.ToPageReference());

            foreach (PageData page in pages)
            {
                LogChange(page, true);
            }
        }

        public static void UrlSegment_Changed(object sender, ContentEventArgs e)
        {

            if (!(e.Content is PageData))
                return;

            if (ContentReference.IsNullOrEmpty(e.ContentLink))
                return; //new page

            PageData oldPage = GetLastVersion(e.ContentLink.ToPageReference(), (e.Content as ILocalizable).MasterLanguage.TwoLetterISOLanguageName) as PageData;// DataFactory.Instance.GetPage(e.PageLink, new LanguageSelector(e.Page.LanguageBranch));

            if (oldPage != null && oldPage.URLSegment != (e.Content as PageData).URLSegment)
                LogChange(oldPage, true);
        }

        public static IContent GetLastVersion(PageReference reference, string lang)
        {
            var versionRepository = ServiceLocator.Current.GetInstance<IContentVersionRepository>();
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();

            var versions = versionRepository.List(reference);
            var lastVersion = versions
                .OrderBy(v => v.Saved)
                .Take(versions.Count() - 1)
                .OrderByDescending(v => v.Saved)
                .FirstOrDefault(version => version.LanguageBranch == lang);

            if (lastVersion == null)
            {
                //var msg = string.Format("Unable to find last version for ContentReference '{0}'.", reference.ID);
                //throw new Exception(msg);
                return null;
            }

            return contentRepository.Get<IContent>(lastVersion.ContentLink, LanguageSelector.AutoDetect(true));
        }

        private static void LogChange(PageData changedPage, bool wildcard = false)
        {
            var relativeUrl = ServiceLocator.Current.GetInstance<UrlResolver>().GetUrl(changedPage.ContentLink).ToLower();
            if (relativeUrl.Length > 1 && relativeUrl.Last() == '/')
                relativeUrl = relativeUrl.Remove(relativeUrl.Length - 1);

            var redirectService = ServiceLocator.Current.GetInstance<RedirectService>();
            redirectService.AddRedirect(10000,
                EPiServer.Web.SiteDefinition.Current.Name.ToLower(),
                relativeUrl,
                wildcard,
                null,
                changedPage.PageLink.ID,
                changedPage.Language.TwoLetterISOLanguageName);

        }

        public static void Page_Deleted(object sender, DeleteContentEventArgs e)
        {
            using (var context = ServiceLocator.Current.GetInstance<RedirectDbContext>())
            {
                foreach (ContentReference descendent in e.DeletedDescendents)
                {
                    var redirects = context.RedirectRules.Where(x => x.ToContentId == descendent.ID);
                    foreach (var r in redirects)
                    {
                        context.RedirectRules.Remove(r);
                    }
                }
                context.SaveChanges();
            }
        }
    }
}
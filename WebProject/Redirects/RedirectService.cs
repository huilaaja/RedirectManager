using System;
using System.Data.Entity;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

namespace WebProject.Redirects
{
    public class RedirectService
    {
        public static bool TableExist = false;
        public const string RedirectTableName = "SOLITA_Redirect";
        private readonly UrlResolver _urlResolver;
        private readonly IContentRepository _contentRepository;

        public RedirectService(UrlResolver urlResolver, IContentRepository contentRepository)
        {
            _urlResolver = urlResolver;
            _contentRepository = contentRepository;
            TableExist = RedirectTableExists();
        }

        public RedirectRule GetRedirect(int id)
        {
            if (!TableExist) return null;
            using (var context = ServiceLocator.Current.GetInstance<RedirectDbContext>())
            {
                return context.RedirectRules.FirstOrDefault(x => x.Id == id);
            }
        }

        public RedirectRule[] List()
        {
            if (!TableExist) return new RedirectRule[] { };

            using (var context = ServiceLocator.Current.GetInstance<RedirectDbContext>())
            {
                return context.RedirectRules
                                .OrderBy(x => x.SortOrder)
                                .ThenBy(x => x.FromUrl)
                                .ToArray();
            }
        }

        public int AddRedirect(int? sortOrder, string fromUrl, bool? wildcard, string toUrl, int? toContentId, string toContentLang)
        {
            if (string.IsNullOrEmpty(toUrl) && toContentId.GetValueOrDefault(0) <= 0)
                return 0;
            if (string.IsNullOrEmpty(toContentLang))
                toContentLang = null;
            if (string.IsNullOrEmpty(fromUrl))
                fromUrl = "/";
            if (!fromUrl.StartsWith("/"))
                fromUrl = "/" + fromUrl;
            using (var context = ServiceLocator.Current.GetInstance<RedirectDbContext>())
            {
                var r = new RedirectRule()
                {
                    SortOrder = sortOrder.GetValueOrDefault(0),
                    FromUrl = fromUrl,
                    Wildcard = wildcard.GetValueOrDefault(false),
                    ToUrl = toUrl,
                    ToContentId = toContentId.GetValueOrDefault(0),
                    ToContentLang = toContentLang,
                };
                context.RedirectRules.Add(r);
                return context.SaveChanges();
            }
        }

        public int ModifyRedirect(int id, int? sortOrder, string fromUrl, bool? wildcard, string toUrl, int? toContentId, string toContentLang)
        {
            if (string.IsNullOrEmpty(toUrl) && toContentId.GetValueOrDefault(0) <= 0)
                return 0;
            if (string.IsNullOrEmpty(toContentLang))
                toContentLang = null;
            if (string.IsNullOrEmpty(fromUrl))
                fromUrl = "/";
            if (!fromUrl.StartsWith("/"))
                fromUrl = "/" + fromUrl;
            using (var context = ServiceLocator.Current.GetInstance<RedirectDbContext>())
            {
                var r = context.RedirectRules.First(x => x.Id == id);
                    r.SortOrder = sortOrder.GetValueOrDefault(0);
                    r.FromUrl = fromUrl;
                    r.Wildcard = wildcard.GetValueOrDefault(false);
                    r.ToUrl = toUrl;
                    r.ToContentId = toContentId.GetValueOrDefault(0);
                    r.ToContentLang = toContentLang;
                context.Entry(r).State = EntityState.Modified;
                return context.SaveChanges();
            }
        }

        public int DeleteRedirect(int id)
        {
            using (var context = ServiceLocator.Current.GetInstance<RedirectDbContext>())
            {
                var r = context.RedirectRules.First(x => x.Id == id);
                context.RedirectRules.Remove(r);
                return context.SaveChanges();
            }
        }

        public string GetPrimaryRedirectUrlOrDefault(string relativeUrl)
        {
            if (!TableExist) return null;
            if (string.IsNullOrEmpty(relativeUrl))
                return null;
            if(relativeUrl.Length > 1 && relativeUrl.Last() == '/')
                relativeUrl = relativeUrl.Remove(relativeUrl.Length - 1);
            relativeUrl = relativeUrl.ToLower();
            using (var context = ServiceLocator.Current.GetInstance<RedirectDbContext>())
            {
                var exactMatch = context.RedirectRules
                                .Where(x => x.FromUrl.Equals(relativeUrl, StringComparison.InvariantCultureIgnoreCase))
                                .OrderBy(x => x.SortOrder)
                                .ThenBy(x => x.FromUrl)
                                .FirstOrDefault();

                var wildcards = context.RedirectRules
                                .Where(x => x.Wildcard)
                                .OrderBy(x => x.SortOrder)
                                .ThenBy(x => x.FromUrl);
                var match = wildcards.FirstOrDefault(x => relativeUrl.StartsWith(x.FromUrl));

                RedirectRule theMatch = (exactMatch != null && match != null)
                                    ? exactMatch.SortOrder < match.SortOrder ? exactMatch : match
                                    : exactMatch ?? match;
                if (theMatch == null) return null;

                return theMatch.ToContentId > 0 
                        ? _urlResolver.GetUrl(new ContentReference(theMatch.ToContentId), theMatch.ToContentLang) 
                        : theMatch.ToUrl;
            }
        }

        public string[] GetGlobalLanguageOptions()
        {
            return _contentRepository.GetLanguageBranches<PageData>(ContentReference.StartPage)
                                    .Select(branch => branch.LanguageID)
                                    .ToArray();
        }


        public bool CreateTable()
        {
            using (var context = ServiceLocator.Current.GetInstance<RedirectDbContext>())
            {
                context.Database.ExecuteSqlCommand(
                    @"CREATE TABLE [dbo].[" + RedirectTableName + @"](
                    [Id][int] IDENTITY(1, 1) NOT NULL,
                    [SortOrder][int] NOT NULL,
                    [FromUrl][nvarchar](max) NULL,
                    [ToUrl][nvarchar](max) NULL,
                    [ToContentId][int] NOT NULL,
                    [ToContentLang][nvarchar](10) NULL,
	                [Wildcard] [bit] NOT NULL DEFAULT ((0)),
                 CONSTRAINT[PK_dbo." + RedirectTableName + @"] PRIMARY KEY CLUSTERED
                ( [Id] ASC)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]
                ");
                return TableExist = true;
            }
        }

        protected bool RedirectTableExists()
        {
            using (var context = ServiceLocator.Current.GetInstance<RedirectDbContext>())
            {
                var t = context.Database.SqlQuery<int?>(@"
                         SELECT 1 FROM sys.tables AS T
                         INNER JOIN sys.schemas AS S ON T.schema_id = S.schema_id
                         WHERE S.Name = 'dbo' AND T.Name = '" + RedirectTableName + @"'")
                         .SingleOrDefault() != null;
                return TableExist = t;
            }
        }

    }




}

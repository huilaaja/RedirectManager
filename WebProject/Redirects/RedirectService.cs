using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace WebProject.Redirects
{
    public class RedirectService
    {
        private readonly UrlResolver _urlResolver;
        private readonly IContentRepository _contentRepository;
        private readonly ISiteDefinitionRepository _siteDefinitionRepository;
        private readonly ILanguageBranchRepository _languageBranchRepository;

        public RedirectService(UrlResolver urlResolver, IContentRepository contentRepository, ISiteDefinitionRepository siteDefinitionRepository, ILanguageBranchRepository languageBranchRepository)
        {
            _urlResolver = urlResolver;
            _contentRepository = contentRepository;
            _siteDefinitionRepository = siteDefinitionRepository;
            _languageBranchRepository = languageBranchRepository;
            RedirectRuleStorage.Init();
        }

        public RedirectRule GetRedirect(int id)
        {
            if (!RedirectRuleStorage.IsUpToDate) return null;
            using (var context = ServiceLocator.Current.GetInstance<RedirectDbContext>())
            {
                return context.RedirectRules.FirstOrDefault(x => x.Id == id);
            }
        }

        public RedirectRule[] List()
        {
            if (!RedirectRuleStorage.IsUpToDate) return new RedirectRule[] { };

            using (var context = ServiceLocator.Current.GetInstance<RedirectDbContext>())
            {
                return context.RedirectRules
                                .OrderBy(x => x.SortOrder)
                                .ThenBy(x => x.Host)
                                .ThenBy(x => x.FromUrl)
                                .ToArray();
            }
        }

        public int AddRedirect(int? sortOrder, string host, string fromUrl, bool? wildcard, string toUrl, int? toContentId, string toContentLang)
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
                    Host = host,
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

        public int ModifyRedirect(int id, int? sortOrder, string host, string fromUrl, bool? wildcard, string toUrl, int? toContentId, string toContentLang)
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
                r.Host = host;
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

        public string GetPrimaryRedirectUrlOrDefault(string host, string relativeUrl)
        {
            if (!RedirectRuleStorage.IsUpToDate) return null;
            if (string.IsNullOrEmpty(relativeUrl))
                return null;
            if (relativeUrl.Length > 1 && relativeUrl.Last() == '/')
                relativeUrl = relativeUrl.Remove(relativeUrl.Length - 1);
            relativeUrl = HttpUtility.UrlDecode(relativeUrl.ToLower());
            using (var context = ServiceLocator.Current.GetInstance<RedirectDbContext>())
            {
                var exactMatch = context.RedirectRules
                                .Where(x => x.Host == null || x.Host == "*" || x.Host.Equals(host, StringComparison.InvariantCultureIgnoreCase))
                                .Where(x => x.FromUrl.Equals(relativeUrl, StringComparison.InvariantCultureIgnoreCase))
                                .OrderBy(x => x.SortOrder)
                                .ThenBy(x => x.FromUrl)
                                .FirstOrDefault();

                var wildcards = context.RedirectRules
                                .Where(x => x.Host == null || x.Host == "*" || x.Host.Equals(host, StringComparison.InvariantCultureIgnoreCase))
                                .Where(x => x.Wildcard)
                                .OrderBy(x => x.SortOrder)
                                .ThenBy(x => x.FromUrl);
                var match = wildcards.FirstOrDefault(x => relativeUrl.StartsWith(x.FromUrl)
                                                        || relativeUrl.Equals(x.FromUrl, StringComparison.InvariantCultureIgnoreCase));

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
            return _languageBranchRepository.ListEnabled().Select(x => x.Culture.Name).ToArray();
        }

        public string[] GetGlobalHostOptions()
        {
            return _siteDefinitionRepository.List()
                                            .SelectMany(s => s.Hosts.Select(h => h.Name))
                                            .Where(h => !h.Equals("*", StringComparison.InvariantCultureIgnoreCase))
                                            .OrderBy(h => h)
                                            .ToArray();
        }


    }

    public static class RedirectRuleStorage
    {
        public const string RedirectTableName = "SOLITA_Redirect";
        public static bool IsUpToDate => TableExist && TableV2;
        public static bool TableExist { get; private set; }
        public static bool TableV2 { get; private set; }

        public static void Init()
        {
            if (!IsUpToDate)
            {
                TableExist = RedirectTableExists();
                TableV2 = TableV2Exists();
            }
        }

        public static bool CreateTable()
        {
            using (var context = ServiceLocator.Current.GetInstance<RedirectDbContext>())
            {
                context.Database.ExecuteSqlCommand(
                    @"CREATE TABLE [dbo].[" + RedirectTableName + @"](
                    [Id][int] IDENTITY(1, 1) NOT NULL,
                    [SortOrder][int] NOT NULL,
                    [Host][nvarchar](max) NULL,
                    [FromUrl][nvarchar](max) NULL,
                    [ToUrl][nvarchar](max) NULL,
                    [ToContentId][int] NOT NULL,
                    [ToContentLang][nvarchar](10) NULL,
	                [Wildcard] [bit] NOT NULL DEFAULT ((0)),
                 CONSTRAINT[PK_dbo." + RedirectTableName + @"] PRIMARY KEY CLUSTERED
                ( [Id] ASC)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]
                ");
                return TableExist = TableV2 = true;
            }
        }

        public static bool UpdateTableV2()
        {
            using (var context = ServiceLocator.Current.GetInstance<RedirectDbContext>())
            {
                context.Database.ExecuteSqlCommand(
                    @"ALTER TABLE [dbo].[" + RedirectTableName + @"] ADD [Host][nvarchar](max) NULL"
                );
                return TableV2 = true;
            }
        }

        public static bool RedirectTableExists()
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

        public static bool TableV2Exists()
        {
            using (var context = ServiceLocator.Current.GetInstance<RedirectDbContext>())
            {
                var t = context.Database.SqlQuery<int?>(@"
                        select 1 from INFORMATION_SCHEMA.columns where
                            table_name = '" + RedirectTableName + @"'
                            and column_name = 'Host'")
                         .SingleOrDefault() != null;
                return TableV2 = t;
            }
        }
    }
}

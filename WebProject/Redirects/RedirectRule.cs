using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Redirects
{
    [Table(RedirectRuleStorage.RedirectTableName)]
    public class RedirectRule
    {
        [Key]
        public int Id { get; set; }
        public string Host { get; set; }
        public int SortOrder { get; set; }
        public string FromUrl { get; set; }
        public string ToUrl { get; set; }
        public int ToContentId { get; set; }
        public string ToContentLang { get; set; }
        public bool Wildcard { get; set; }
    }
}

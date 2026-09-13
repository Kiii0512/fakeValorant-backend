using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexusProtocol.API.Models
{
    [Table("site_settings")]
    public class SiteSetting
    {
        [Key]
        [Column("key")]
        public string Key { get; set; } = string.Empty;

        [Column("value")]
        public string Value { get; set; } = string.Empty;
    }
}
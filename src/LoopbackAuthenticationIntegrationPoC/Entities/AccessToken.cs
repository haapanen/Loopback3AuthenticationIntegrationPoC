using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LoopbackAuthenticationIntegrationPoC.Entities
{
    [Table("accesstoken")]
    public class AccessToken
    {
        [Key] 
        [Column("id")] 
        public string Id { get; set; } = Guid.NewGuid().ToString("n");
        [Column("ttl")] 
        public int Ttl { get; set; } = 1209600;
        [Column("scopes")]
        public string[] Scopes { get; set; } = null;
        [Column("created")]
        public DateTime Created { get; set; } = DateTime.UtcNow;
        [Required]
        [Column("userId")]
        public int UserId { get; set; }
        [Required]
        [JsonIgnore]
        public User User { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LoopbackAuthenticationIntegrationPoC.Entities
{
    [Table("user")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("realm")]
        public string Realm { get; set; }
        [Column("username")]
        public string Username { get; set; }
        [Required]
        [JsonIgnore]
        [Column("password")]
        public string PasswordHash { get; set; }
        [Required]
        [Column("email")]
        public string Email { get; set; }
        [Column("emailVerified")]
        public bool? EmailVerified { get; set; }
        [Column("verificationToken")]
        public string VerificationToken { get; set; }
        [JsonIgnore]
        public List<AccessToken> AccessTokens { get; set; } = new List<AccessToken>();
    }
}

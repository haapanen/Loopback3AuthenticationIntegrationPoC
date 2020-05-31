using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LoopbackAuthenticationIntegrationPoC.Entities
{
    [Table("acl")]
    public class Acl
    {
        [Key]
        public int Id { get; set; }
        [Column("model")]
        public string Model { get; set; }
        [Column("property")]
        public string Property { get; set; }
        [Column("accessType")]
        public string AccessType { get; set; }
        [Column("permission")]
        public string Permission { get; set; }
        [Column("principalType")]
        public string PrincipalType { get; set; }
        [Column("principalId")]
        public string PrincipalId { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lykke.Service.Referral.MsSqlRepositories.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }
    }
}

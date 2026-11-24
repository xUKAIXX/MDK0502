using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace RealEstateAgency.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    [Display(Name = "ФИО")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Телефон")]
    [StringLength(30)]
    public string? Phone { get; set; }

    [Display(Name = "Дата регистрации")]
    public DateTime RegisteredAt { get; set; } = DateTime.Now;
}

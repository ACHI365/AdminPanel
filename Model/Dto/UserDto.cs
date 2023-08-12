using System.ComponentModel.DataAnnotations;

namespace AdminPanel.Model.Dto;

public class UserDto
{
    [Required]
    [MaxLength(256)]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}

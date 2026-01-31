using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModels;

public class EditUserViewModel
{
    public EditUserViewModel()
    {
        Claims = [];
        Roles = [];
    }

    public string Id { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    public string City { get; set; }
    public List<string> Claims { get; set; }
    public IList<string> Roles { get; set; }
}

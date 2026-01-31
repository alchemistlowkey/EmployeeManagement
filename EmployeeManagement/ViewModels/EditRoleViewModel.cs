using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModels;

public class EditRoleViewModel
{
    public EditRoleViewModel()
    {
        Users = [];
    }
    public string? Id { get; set; }

    [Required(ErrorMessage = "Role name is required")]
    public string? RoleName { get; set; }

    public List<string>? Users { get; set; }
}

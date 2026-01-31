using System;
using System.ComponentModel.DataAnnotations;
using EmployeeManagement.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.ViewModels;

public class RegisterViewModel
{
    [Required]
    [EmailAddress]
    [Remote(action: "EmailExists", controller: "Account")]
    // [ValidEmailDomain(allowedDomain: "alchemistlowkey.com", ErrorMessage = "Email domain must be alchemistlowkey.com")]
    public string? Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
    public string? ConfirmPassword { get; set; }

    public string? City { get; set; }
}

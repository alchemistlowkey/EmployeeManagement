using System;

namespace EmployeeManagement.ViewModels;

public class UserClaimsViewModel
{
    public UserClaimsViewModel()
    {
        Cliams = [];
    }
    public string UserId { get; set; }
    public List<UserClaim> Cliams { get; set; }
}

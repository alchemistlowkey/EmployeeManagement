using System;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Models;

public static class ModelBuilderExtensions
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    Name = "Maryam",
                    Email = "m@b.com",
                    Department = Dept.HR
                },
                new Employee
                {
                    Id = 2,
                    Name = "John",
                    Email = "j@b.com",
                    Department = Dept.IT
                },
                new Employee
                {
                    Id = 3,
                    Name = "Sam",
                    Email = "s@b.com",
                    Department = Dept.Payroll
                },
                new Employee
                {
                    Id = 4,
                    Name = "Sara",
                    Email = "s@b.com",
                    Department = Dept.Admin
                },
                new Employee
                {
                    Id = 5,
                    Name = "Bob",
                    Email = "b@b.com",
                    Department = Dept.None
                }
            );
    }

}

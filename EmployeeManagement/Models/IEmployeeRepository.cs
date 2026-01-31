using System;

namespace EmployeeManagement.Models;

public interface IEmployeeRepository
{
    // Get employee by ID
    Employee GetEmployee(int id);
    // Get all employees
    IEnumerable<Employee> GetAllEmployees();
    // Add a new employee
    Employee Add(Employee employee);
    // Update an existing employee
    Employee Update(Employee employeeChanges);
    // Delete an employee by ID
    Employee Delete(int id);
}

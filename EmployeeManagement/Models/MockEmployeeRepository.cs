using System;

namespace EmployeeManagement.Models;

public class MockEmployeeRepository : IEmployeeRepository
{
    private List<Employee> _employeeList;

    public MockEmployeeRepository()
    {
        _employeeList =
        [
            new Employee() { Id = 1, Name = "Mary", Department = Dept.HR, Email = "m@b.com" },
            new Employee() { Id = 2, Name = "John", Department = Dept.IT, Email = "j@b.com" },
            new Employee() { Id = 3, Name = "Sam", Department = Dept.Payroll, Email = "s@b.com" },
            new Employee() { Id = 4, Name = "Sara", Department = Dept.Admin, Email = "s@b.com" },
            new Employee() { Id = 5, Name = "Bob", Department = Dept.None, Email = "b@b.com" }
        ];
    }

    public Employee Add(Employee employee)
    {
        employee.Id = _employeeList.Max(e => e.Id) + 1;
        _employeeList.Add(employee);
        return employee;
    }

    public Employee Delete(int id)
    {
        Employee employee = _employeeList.FirstOrDefault(emp => emp.Id == id);
        if (employee != null)
        {
            _employeeList.Remove(employee);
        }
        return employee;
    }

    public IEnumerable<Employee> GetAllEmployees()
    {
        return _employeeList;
    }

    public Employee GetEmployee(int id)
    {
        return _employeeList.FirstOrDefault(e => e.Id == id);
    }

    public Employee Update(Employee employeeChanges)
    {
        Employee employee = _employeeList.FirstOrDefault(emp => emp.Id == employeeChanges.Id);
        if (employee != null)
        {
            employee.Name = employeeChanges.Name;
            employee.Email = employeeChanges.Email;
            employee.Department = employeeChanges.Department;
        }
        return employee;
    }
}

using System;
using EmployeeManagement.Models;
using EmployeeManagement.Security;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers;

// [Route("Home")]
// [Route("[controller]")]
// [Authorize]
public class HomeController : Controller
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IWebHostEnvironment hostingEnvironment;
    private readonly ILogger logger;
    private readonly IDataProtector protector;

    public HomeController(IEmployeeRepository employeeRepository, IWebHostEnvironment hostingEnvironment, ILogger<HomeController> logger, IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings)
    {

        _employeeRepository = employeeRepository;
        this.hostingEnvironment = hostingEnvironment;
        this.logger = logger;
        protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.EmployeeIdRouteValue);
    }

    // Attributes routing
    // [Route("")]
    // [Route("/")]
    // [Route("[Index]")]
    // [Route("[action]")]
    [AllowAnonymous]
    public ViewResult Index()
    {
        var model = _employeeRepository.GetAllEmployees()
                                       .Select(e =>
                                       {
                                           e.EncryptedId = protector.Protect(e.Id.ToString());
                                           return e;
                                       });

        return View(model);
    }

    // [Route("Details/{id?}")]
    // [Route("[action]/{id?}")]
    [AllowAnonymous]
    public ViewResult Details(string id)
    {
        // throw new Exception("Error");

        logger.LogTrace("Trace Log");
        logger.LogDebug("Debug Log");
        logger.LogInformation("Information Log");
        logger.LogWarning("Warning Log");
        logger.LogError("Error Log");
        logger.LogCritical("Critical Log");

        int employeeId = Convert.ToInt32(protector.Unprotect(id));

        Employee employee = _employeeRepository.GetEmployee(employeeId);

        if (employee == null)
        {
            Response.StatusCode = 404;
            return View("EmployeeNotFound", employeeId);
        }

        HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
        {
            Employee = employee,
            PageTitle = "Employee Details Page"
        };

        return View(homeDetailsViewModel);

        // Employee model = _employeeRepository.GetEmployee(2);
        // ViewBag.PageTitle = "Employee Details";
        // return View(model);


        // ViewData["Employee"] = model;
        // ViewData["PageTitle"] = "Employee Details";

        // ViewBag.Employee = model;
        // ViewBag.PageTitle = "Employee Details";
        // return View();

        // return View(model); // Looks for Views/Home/Details.cshtml

        // return View("../../MyViews/Test"); // Looks for Views/MyViews/Test.cshtml
        // return View("../Test/Update"); // Looks for Views/Test/Update.cshtml
        // return View("MyViews/Test.cshtml"); // Works but not recommended
        // return View("Test"); // Looks for Views/Home/Test.cshtml
    }

    [HttpGet]
    public ViewResult Edit(int id)
    {
        Employee employee = _employeeRepository.GetEmployee(id);
        EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
        {
            Id = employee.Id,
            Name = employee.Name,
            Email = employee.Email,
            Department = employee.Department,
            ExistingPhotoPath = employee.PhotoPath
        };
        return View(employeeEditViewModel);
    }

    [HttpPost]
    public IActionResult Edit(EmployeeEditViewModel model)
    {
        if (ModelState.IsValid)
        {
            Employee employee = _employeeRepository.GetEmployee(model.Id);
            employee.Name = model.Name;
            employee.Email = model.Email;
            employee.Department = model.Department;
            if (model.Photo != null)
            {
                if (model.ExistingPhotoPath != null)
                {
                    string filePath = Path.Combine(hostingEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                    System.IO.File.Delete(filePath);
                }
                employee.PhotoPath = ProcessUploadFile(model);
            }

            Employee updatedEmployee = _employeeRepository.Update(employee);
            return RedirectToAction("index");
        }
        return View(model);
    }

    [HttpGet]
    public ViewResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(EmployeeCreateViewModel model)
    {
        if (ModelState.IsValid)
        {
            string uniqueFileName = ProcessUploadFile(model);

            Employee newEmployee = new Employee
            {
                Name = model.Name,
                Email = model.Email,
                Department = model.Department,
                PhotoPath = uniqueFileName
            };
            _employeeRepository.Add(newEmployee);
            return RedirectToAction("details", new { id = newEmployee.Id });
        }
        return View();
    }

    private string ProcessUploadFile(EmployeeCreateViewModel model)
    {
        string uniqueFileName = null;
        if (model.Photo != null)
        {

            string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                model.Photo.CopyTo(fileStream);
            }
        }
        return uniqueFileName;
    }
}

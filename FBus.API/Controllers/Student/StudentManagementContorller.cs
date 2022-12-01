using System.Threading.Tasks;
using FBus.Business.StudentManagement.Interface;
using FBus.Business.StudentManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBus.API.Controllers.Student
{
    [ApiController]
    [Authorize]
    public class StudentManagementContorller : BaseController
    {
        private readonly IStudentService _studentService;

        public StudentManagementContorller(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        [Route(ApiVer1Url.Student.BaseApiUrl + "/{id}")]
        public async Task<IActionResult> GetDetailsByStudent(string id)
        {
            return SendResponse(await _studentService.GetDetails(id));
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.Student + "/{id}")]
        public async Task<IActionResult> GetDetailsByAdmin(string id)
        {
            return SendResponse(await _studentService.GetDetails(id));
        }


        [HttpGet]
        [Route(ApiVer1Url.Student.BaseApiUrl + "/statistics/{id}")]
        public async Task<IActionResult> GetDetailStatistics(string id)
        {
            return SendResponse(await _studentService.Statistics(id));
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.StudentList)]
        public async Task<IActionResult> GetList()
        {
            return SendResponse(await _studentService.GetList());
        }

        [HttpPut]
        [Route(ApiVer1Url.Admin.Student + "/{id}")]
        public async Task<IActionResult> UpdateByAdmin(string id, [FromForm] UpdateStudentModel student)
        {
            return SendResponse(await _studentService.Update(id, student));
        }

        [HttpPut]
        [Route(ApiVer1Url.Student.BaseApiUrl + "/{id}")]
        public async Task<IActionResult> UpdateByStudent(string id, [FromForm] UpdateStudentModel student)
        {
            return SendResponse(await _studentService.Update(id, student));
        }
    }
}
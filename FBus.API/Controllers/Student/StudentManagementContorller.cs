using System.Threading.Tasks;
using FBus.Business.StudentManagement.Interface;
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
        public async Task<IActionResult> GetDetails(string id)
        {
            return SendResponse(await _studentService.GetDetails(id));
        }

        [HttpGet]
        [Route(ApiVer1Url.Student.StudentList)]
        public async Task<IActionResult> GetList()
        {
            return SendResponse(await _studentService.GetList());
        }
    }
}
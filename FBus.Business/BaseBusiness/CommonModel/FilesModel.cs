using Microsoft.AspNetCore.Http;

namespace FBus.Business.BaseBusiness.CommonModel
{
    public class FilesModel
    {
        public string[] DeleteFiles { get; set; }
        public IFormFile[] UploadFiles { get; set; }
    }
}
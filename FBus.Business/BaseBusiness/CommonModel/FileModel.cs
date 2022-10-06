using Microsoft.AspNetCore.Http;

namespace FBus.Business.BaseBusiness.CommonModel
{
    public class FileModel
    {
        public string DeleteFile { get; set; }
        public IFormFile UploadFile { get; set; }
    }
}
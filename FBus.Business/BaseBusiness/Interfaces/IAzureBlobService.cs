using System.Threading.Tasks;
using FBus.Business.BaseBusiness.Configuration;
using Microsoft.AspNetCore.Http;

namespace FBus.Business.BaseBusiness.Interfaces
{
    public interface IAzureBlobService
    {
        Task<string> UploadFile(IFormFile[] files, AzureBlobContainer index);
        Task<string> UploadFile(IFormFile file, AzureBlobContainer index);
        Task<string> DeleteFile(string[] fileNames, AzureBlobContainer index, string photoUrl);
        Task<string> DeleteFile(string fileName, AzureBlobContainer index, string photoUrl);
    }
}
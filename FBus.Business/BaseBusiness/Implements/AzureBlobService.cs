using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using FBus.Business.BaseBusiness.Configuration;
using FBus.Business.BaseBusiness.Interfaces;
using FBus.Data.Interfaces;
using Microsoft.AspNetCore.Http;

namespace FBus.Business.BaseBusiness.Implements
{
    public class AzureBlobService : IAzureBlobService
    {
        protected readonly BlobServiceClient _blobServiceClient;
        private static readonly string[] _container = { "driver", "student" };
        public AzureBlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        // Upload files to azure blob
        public async Task<string> UploadFile(IFormFile[] files, AzureBlobContainer index)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(_container[(int)index]);
            if (blobContainer == null || files == null)
            {
                return null;
            }
            string result = string.Empty;
            foreach (IFormFile file in files)
            {
                string fileName = Guid.NewGuid().ToString() + "." + file.ContentType.Substring(6);
                var blobClient = blobContainer.GetBlobClient(fileName);
                await blobClient.UploadAsync(file.OpenReadStream());
                result += fileName + " ";
            }
            return result;
        }

        // Upload file to azure blob
        public async Task<string> UploadFile(IFormFile file, AzureBlobContainer index)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(_container[(int)index]);
            if (blobContainer == null || file == null)
            {
                return null;
            }
            string fileName = Guid.NewGuid().ToString() + "." + file.ContentType.Substring(6);
            var blobClient = blobContainer.GetBlobClient(fileName);
            await blobClient.UploadAsync(file.OpenReadStream());
            return fileName + " ";
        }

        // Delete files from azure blob
        public async Task<string> DeleteFile(string[] fileNames, AzureBlobContainer index, string photoUrl)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(_container[(int)index]);
            if (blobContainer == null || fileNames == null)
            {
                return photoUrl;
            }

            foreach (string fileName in fileNames)
            {
                if (photoUrl.Contains(fileName))
                {
                    try
                    {
                        var blobClient = blobContainer.GetBlobClient(fileName);
                        await blobClient.DeleteAsync();
                        photoUrl = photoUrl.Replace(fileName + " ", "");
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            return photoUrl;
        }

        // Delete file from azure blob
        public async Task<string> DeleteFile(string fileName, AzureBlobContainer index, string photoUrl)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(_container[(int)index]);
            if (blobContainer == null || string.IsNullOrEmpty(fileName))
            {
                return photoUrl;
            }
            if (photoUrl.Contains(fileName))
            {
                try
                {
                    var blobClient = blobContainer.GetBlobClient(fileName.Trim());
                    await blobClient.DeleteAsync();
                    photoUrl = photoUrl.Replace(fileName.Trim() + " ", "");
                }
                catch
                {
                    return "";
                }
            }
            return photoUrl;
        }
    }
}
using System;
using System.Linq;
using System.Text;
using FBus.Business.BaseBusiness.Interfaces;
using FBus.Data.Interfaces;

namespace FBus.Business.BaseBusiness.Implements
{
    public class BaseService : IBaseService
    {
        protected readonly IUnitOfWork _unitOfWork;

        public BaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public static T UpdateTypeOfNullAbleObject<T>(T oldValue, T newValue)
        {
            return newValue != null ? newValue : oldValue;
        }
        public static T UpdateTypeOfNotNullAbleObject<T>(T oldValue, T? newValue) where T : struct
        {
            return newValue != null ? newValue.Value : oldValue;
        }

        public static T? UpdateTypeOfNotNullAbleObject<T>(T? oldValue, T? newValue) where T : struct
        {
            return newValue != null ? newValue.Value : oldValue != null ? oldValue.Value : null;
        }

        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public static bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != passwordHash[i]) return false;
            }

            return true;
        }

        public static string GeneratePinCodeAuto(int lengthOfString)
        {
            var random = new Random();
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, lengthOfString).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
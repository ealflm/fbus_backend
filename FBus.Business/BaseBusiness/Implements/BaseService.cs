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
    }
}
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
    }
}
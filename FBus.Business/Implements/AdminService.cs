using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FBus.Business.Interfaces;
using FBus.Business.ViewModel;
using FBus.Data.Interfaces;

namespace FBus.Business.Implements
{
    public class AdminService : BaseService, IAdminService
    {
        public AdminService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<List<AdminViewModel>> GetAdmin()
        {
            var admins = await _unitOfWork.AdminRepository.Query()
                .Select(x => new AdminViewModel()
                {
                    Username = x.Username
                }).ToListAsync();
            return admins;
        }
    }
}

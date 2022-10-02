using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FBus.Business.ViewModel;

namespace FBus.Business.Interfaces
{
    public interface IAdminService
    {
        Task<List<AdminViewModel>> GetAdmin();
    }
}

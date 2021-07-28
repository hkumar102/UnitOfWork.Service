using System;
using UnitOfWork.Interface.Providers;

namespace UnitOfWork.Service.Providers
{

    public class DefaultUserInformationProvider : IUserInformationProvider
    {
        public int UserId { get; set; }
    }
}

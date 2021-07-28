using System;
using System.Linq;
using UnitOfWork.Interface.Providers;

namespace UnitOfWork.Service.Providers
{

    /// <summary>
    /// Default Audit Stamp provider to set audit properties
    /// </summary>
    public class DefaultAuditStampProvider : IAuditStampProvider<int>
    {
        private readonly IUserInformationProvider<int> _userInformationProvider;
        public DefaultAuditStampProvider(IUserInformationProvider<int> userInformationProvider)
        {
            _userInformationProvider = userInformationProvider;
        }

        public void SetModifiedEntityAuditStamp(object entity)
        {
            var updatedOnProperty = entity.GetType().GetProperties().FirstOrDefault(x => x.Name == "UpdatedOn");

            if (updatedOnProperty != null)
            {
                updatedOnProperty.SetValue(entity, DateTime.UtcNow);
            }

            var updatedByProperty = entity.GetType().GetProperties().FirstOrDefault(x => x.Name == "UpdatedBy");

            if (updatedByProperty != null)
            {
                updatedByProperty.SetValue(entity, _userInformationProvider.UserId);
            }
        }

        public void SetNewEntityAuditStamp(object entity)
        {

            var createdOnProperty = entity.GetType().GetProperties().FirstOrDefault(x => x.Name == "CreatedOn");

            if (createdOnProperty != null)
            {
                createdOnProperty.SetValue(entity, DateTime.UtcNow);
            }

            var createdByProperty = entity.GetType().GetProperties().FirstOrDefault(x => x.Name == "CreatedBy");

            if (createdByProperty != null)
            {
                createdByProperty.SetValue(entity, _userInformationProvider.UserId);
            }
        }
    }
}

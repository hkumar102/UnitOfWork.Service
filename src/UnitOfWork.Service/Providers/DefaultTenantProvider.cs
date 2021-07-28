using System;
using System.Linq;
using UnitOfWork.Interface.Providers;

namespace UnitOfWork.Service.Providers
{
    /// <summary>
    /// Default TenantId to get and set the tenant property when a new entity created if application is multi-tenant
    /// </summary>
    public class DefaultTenantProvider : ITenantProvider<int>
    {
        public int TenantId { get; set; }

        public void SetTenantProperty(object entity)
        {
            var tenantProperty = entity.GetType().GetProperties().FirstOrDefault(x => x.Name == "TenantId");

            if (tenantProperty != null)
            {
                tenantProperty.SetValue(entity, TenantId);
            }
        }
    }
}

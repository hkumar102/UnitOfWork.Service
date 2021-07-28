using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitOfWork.Interface;
using UnitOfWork.Interface.Providers;

namespace UnitOfWork.Service
{
    public class DefaultEntityAuditHandler : IEntityAuditHandler<int>
    {
        private readonly ITenantProvider<int> _tenantProvider;
        private readonly IAuditStampProvider<int> _auditStampProvider;
        private readonly ISoftDeleteProvider _softDeleteProvider;
        private readonly DbContext _context;

        /// <summary>
        /// ITenantProvider<TKey> passed in constructor and not null meaning its a Multi-Tenant Applications
        /// </summary>
        public readonly bool IsMultiTenantApplication = false;

        /// <summary>
        /// ISoftDeleteProvider passed in constructor and not null meaning SoftDelete Enalbed.
        /// When you delete a application it will automatically look for the ISoftDeleteProvider and Update the "Deleted" field 
        /// Change the EntityState to Modified from Deleted
        /// </summary>
        public readonly bool IsSoftDeleteEnabled = false;

        public DefaultEntityAuditHandler(
            DbContext dbConext,
            ITenantProvider<int> tenantProvider = null,
            IAuditStampProvider<int> auditStampProvider = null,
            ISoftDeleteProvider softDeleteProvider = null)
        {

            _context = dbConext;
            _tenantProvider = tenantProvider;
            _auditStampProvider = auditStampProvider;
            _softDeleteProvider = softDeleteProvider;

            if (_tenantProvider != null)
            {
                IsMultiTenantApplication = true;
            }

            if (_softDeleteProvider != null)
            {
                IsSoftDeleteEnabled = true;
            }
        }

        public void Handle()
        {
            var dbEntities = _context.ChangeTracker.Entries().Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted)).ToList();
            var currentTime = DateTime.UtcNow;
            foreach (var entityEntry in dbEntities)
            {
                var entity = entityEntry.Entity;

                switch (entityEntry.State)
                {
                    case EntityState.Deleted:
                        if (IsSoftDeleteEnabled) _softDeleteProvider.SetDeleteProperty(entityEntry);
                        break;
                    case EntityState.Modified:
                        if (_auditStampProvider != null) _auditStampProvider.SetModifiedEntityAuditStamp(entity);
                        break;
                    case EntityState.Added:
                        if (_auditStampProvider != null) _auditStampProvider.SetNewEntityAuditStamp(entity);
                        if (IsMultiTenantApplication && _tenantProvider != null) _tenantProvider.SetTenantProperty(entity);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

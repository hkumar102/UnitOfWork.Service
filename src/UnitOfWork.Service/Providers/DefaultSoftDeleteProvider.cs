using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;
using UnitOfWork.Interface.Providers;

namespace UnitOfWork.Service.Providers
{

    /// <summary>
    /// Default SoftDelete Provider to mark property field as deleted whenever a new deleted request comes
    /// </summary>
    public class DefaultSoftDeleteProvider : ISoftDeleteProvider
    {
        public void SetDeleteProperty(EntityEntry entityEntry)
        {
            var entity = entityEntry.Entity;
            var deletedProperty = entity.GetType().GetProperties().FirstOrDefault(x => x.Name == "IsDeleted");
            if (deletedProperty != null)
            {
                deletedProperty.SetValue(entity, true);
            }

            entityEntry.State = EntityState.Modified;
        }
    }
}

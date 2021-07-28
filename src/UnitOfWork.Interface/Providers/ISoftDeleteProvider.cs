using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace UnitOfWork.Interface.Providers
{
    public interface ISoftDeleteProvider
    {
        void SetDeleteProperty(EntityEntry entityEntry);
    }
}

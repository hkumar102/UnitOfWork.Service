

namespace UnitOfWork.Interface.Providers
{
    /// <summary>
    /// Interface used to define the Audit Stamp dynamic Properties like CreatedOn, CreatedBy etc
    /// </summary>
    public interface IAuditStampProvider<TKey>
        where TKey : struct
    {
        /// <summary>
        /// This method will update the entiy properties for all audit properties like CreatedBy, CreatedOn
        /// </summary>
        /// <param name="entity">entity to set the value for</param>
        void SetNewEntityAuditStamp(object entity);

        /// <summary>
        /// This method will update the entiy properties for all audit properties like UpdateBy, UpdatedOn
        /// </summary>
        /// <param name="entity">entity to set the value for</param>
        void SetModifiedEntityAuditStamp(object entity);
    }
}

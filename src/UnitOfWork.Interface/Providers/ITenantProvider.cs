namespace UnitOfWork.Interface.Providers
{

    /// <summary>
    /// In a multi-tenant application this interface will provide the tenantid
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface ITenantProvider<TKey>
        where TKey : struct
    {
        TKey TenantId { get; }
        void SetTenantProperty(object entity);
    }
}


namespace UnitOfWork.Interface.Providers
{
    /// <summary>
    /// This interface is used to find out information about the user, right now we only require UserId 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IUserInformationProvider<TKey>
        where TKey : struct
    {
        TKey UserId { get; }
    }

    public interface IUserInformationProvider : IUserInformationProvider<int> { }
}

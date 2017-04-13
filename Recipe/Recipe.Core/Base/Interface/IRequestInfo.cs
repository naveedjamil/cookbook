using System.Data.Entity;

namespace Recipe.Core.Base.Interface
{
    public interface IRequestInfo
    {
        string UserId { get; }
        string UserName { get; }
        string Role { get; }
        DbContext Context { get; }
    }
}

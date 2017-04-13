using Recipe.Core.Base.Abstract;

namespace Test.Data.Model
{
    public class User : EntityBase<int>
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
    }
}

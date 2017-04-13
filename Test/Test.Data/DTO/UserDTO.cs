using Recipe.Core.Base.Abstract;
using Test.Data.Model;

namespace Test.Data.DTO
{
    public class UserDTO : DTO<User, int>
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public override void ConvertFromEntity(User entity)
        {
            base.ConvertFromEntity(entity);

            this.FirstName = entity.FirstName;
            this.MiddleName = entity.MiddleName;
            this.LastName = entity.LastName;
        }

        public override User ConvertToEntity(User entity)
        {
            entity = base.ConvertToEntity(entity);

            entity.FirstName = this.FirstName;
            entity.MiddleName = this.MiddleName;
            entity.LastName = this.LastName;

            return entity;
        }
    }
}

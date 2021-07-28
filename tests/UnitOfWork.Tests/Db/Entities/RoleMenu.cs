
using System.ComponentModel.DataAnnotations.Schema;

namespace UnitOfWork.Tests.Db.Entities
{
    [Table("RoleMenu", Schema = "Security")]
    public class RoleMenu
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int MenuId { get; set; }

        public Menu Menu { get; set; }
    }
}

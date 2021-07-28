using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace UnitOfWork.Tests.Db.Entities
{
    [Table("MenuType", Schema = "Config")]
    public class MenuType
    {
        public MenuType()
        {
            Menus = new List<Menu>();
        }

        public int Id { get; set; }
        public List<Menu> Menus { get; set; }
    }
}

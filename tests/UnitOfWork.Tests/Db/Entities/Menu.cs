using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace UnitOfWork.Tests.Db.Entities
{
    [Table("Menu", Schema = "Config")]
    public class Menu
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public int MenuTypeId { get; set; }
        public MenuType MenuType { get; set; }
    }
}

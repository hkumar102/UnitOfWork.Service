using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UnitOfWork.Tests.Db.Entities;

namespace UnitOfWork.Tests.Db
{
    public class InMemoryContext : DbContext
    {
        public DbSet<RoleMenu> RoleMenus { get; set; }
        public DbSet<Menu> Menus { get; set; }

        public InMemoryContext(DbContextOptions<InMemoryContext> options) : base(options) { }
    }
}

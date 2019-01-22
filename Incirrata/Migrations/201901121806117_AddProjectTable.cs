namespace Incirrata.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProjectTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Project",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    Name = c.String(nullable: false, maxLength: 256),
                    Code = c.String(nullable: false),
                    CreatedAt = c.DateTime(nullable: false),
                    UpdatedAt = c.DateTime(nullable: false)
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "ProjectNameIndex");
        }
        
        public override void Down()
        {
        }
    }
}

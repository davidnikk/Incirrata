namespace Incirrata.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdentityUserModify : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "CreatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "Name", c => c.String());
            AddColumn("dbo.AspNetUsers", "Surname", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Surname");
            DropColumn("dbo.AspNetUsers", "Name");
            DropColumn("dbo.AspNetUsers", "CreatedAt");
        }
    }
}

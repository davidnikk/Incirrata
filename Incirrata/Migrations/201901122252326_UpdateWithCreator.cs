namespace Incirrata.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateWithCreator : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Project", "CreatorId", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Task", "CreatorId", c => c.String(nullable: false, maxLength: 128));
            AddForeignKey("dbo.Project", "CreatorId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Task", "CreatorId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
        }
    }
}

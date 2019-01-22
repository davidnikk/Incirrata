namespace Incirrata.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateProjectTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Project", "ProjectManager", c => c.String(maxLength: 128));
            AddForeignKey("dbo.Project", "ProjectManager", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
        }
    }
}

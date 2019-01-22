namespace Incirrata.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStatusColumn : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Task", "Status");
            AddColumn("dbo.Task", "Status", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
        }
    }
}

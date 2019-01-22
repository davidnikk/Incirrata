namespace Incirrata.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTaskTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Task",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    Status = c.String(nullable: false, maxLength: 10),
                    Deadline = c.DateTime(nullable: false),
                    Description = c.String(nullable: false),
                    Progress = c.Int(nullable: false),
                    UserId = c.String(nullable: false, maxLength: 128),
                    ProjectId = c.String(nullable: false, maxLength: 128),
                    CreatedAt = c.DateTime(nullable: false),
                    UpdatedAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                //.ForeignKey("dbo.AspNetUsers", t => t.UserId)
                //.ForeignKey("dbo.Project", t => t.ProjectId)
                .Index(t => t.Id, unique: true, name: "TaskIndex");
            
            AddForeignKey("dbo.Task", "ProjectId", "dbo.Project", "Id");
            AddForeignKey("dbo.Task", "UserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
        }
    }
}

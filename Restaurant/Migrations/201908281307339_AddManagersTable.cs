namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddManagersTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Managers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Managers", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Managers", new[] { "UserId" });
            DropTable("dbo.Managers");
        }
    }
}

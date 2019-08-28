namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWaitersTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Waiters",
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
            DropForeignKey("dbo.Waiters", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Waiters", new[] { "UserId" });
            DropTable("dbo.Waiters");
        }
    }
}

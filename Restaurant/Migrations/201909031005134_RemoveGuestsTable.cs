namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveGuestsTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Guests", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Guests", new[] { "UserId" });
            DropTable("dbo.Guests");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Guests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.Guests", "UserId");
            AddForeignKey("dbo.Guests", "UserId", "dbo.AspNetUsers", "Id");
        }
    }
}

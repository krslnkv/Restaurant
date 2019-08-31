namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveGuestIdColumnFromOrderssTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Orders", "GuestId", "dbo.Guests");
            DropIndex("dbo.Orders", new[] { "GuestId" });
            DropColumn("dbo.Orders", "GuestId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "GuestId", c => c.Int(nullable: false));
            CreateIndex("dbo.Orders", "GuestId");
            AddForeignKey("dbo.Orders", "GuestId", "dbo.Guests", "Id", cascadeDelete: true);
        }
    }
}

namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGuestIdForOrdersTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderDetails", "GuestId", "dbo.Guests");
            DropIndex("dbo.OrderDetails", new[] { "GuestId" });
            AddColumn("dbo.Orders", "GuestId", c => c.Int(nullable: false));
            CreateIndex("dbo.Orders", "GuestId");
            AddForeignKey("dbo.Orders", "GuestId", "dbo.Guests", "Id", cascadeDelete: true);
            DropColumn("dbo.OrderDetails", "GuestId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderDetails", "GuestId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Orders", "GuestId", "dbo.Guests");
            DropIndex("dbo.Orders", new[] { "GuestId" });
            DropColumn("dbo.Orders", "GuestId");
            CreateIndex("dbo.OrderDetails", "GuestId");
            AddForeignKey("dbo.OrderDetails", "GuestId", "dbo.Guests", "Id", cascadeDelete: true);
        }
    }
}

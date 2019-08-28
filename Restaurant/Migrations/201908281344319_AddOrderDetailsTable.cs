namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrderDetailsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GuestId = c.Int(nullable: false),
                        DishId = c.Int(nullable: false),
                        OrderTime = c.DateTime(nullable: false),
                        OrderId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Dishes", t => t.DishId, cascadeDelete: true)
                .ForeignKey("dbo.Guests", t => t.GuestId, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.GuestId)
                .Index(t => t.DishId)
                .Index(t => t.OrderId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderDetails", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.OrderDetails", "GuestId", "dbo.Guests");
            DropForeignKey("dbo.OrderDetails", "DishId", "dbo.Dishes");
            DropIndex("dbo.OrderDetails", new[] { "OrderId" });
            DropIndex("dbo.OrderDetails", new[] { "DishId" });
            DropIndex("dbo.OrderDetails", new[] { "GuestId" });
            DropTable("dbo.OrderDetails");
        }
    }
}

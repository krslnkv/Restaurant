namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWaiterIdColumnForOrders : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "WaiterId", c => c.Int(nullable: false));
            CreateIndex("dbo.Orders", "WaiterId");
            AddForeignKey("dbo.Orders", "WaiterId", "dbo.Waiters", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "WaiterId", "dbo.Waiters");
            DropIndex("dbo.Orders", new[] { "WaiterId" });
            DropColumn("dbo.Orders", "WaiterId");
        }
    }
}

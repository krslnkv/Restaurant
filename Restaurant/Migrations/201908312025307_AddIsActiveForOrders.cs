namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsActiveForOrders : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "IsActive");
        }
    }
}

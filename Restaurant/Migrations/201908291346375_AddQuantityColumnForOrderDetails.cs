namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddQuantityColumnForOrderDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderDetails", "Quantity", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderDetails", "Quantity");
        }
    }
}

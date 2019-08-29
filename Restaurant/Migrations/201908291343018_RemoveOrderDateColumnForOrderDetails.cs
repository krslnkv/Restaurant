namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveOrderDateColumnForOrderDetails : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.OrderDetails", "OrderTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderDetails", "OrderTime", c => c.DateTime(nullable: false));
        }
    }
}

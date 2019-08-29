namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFinalPriceColumnForOrderssTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "FinalPrice", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "FinalPrice");
        }
    }
}

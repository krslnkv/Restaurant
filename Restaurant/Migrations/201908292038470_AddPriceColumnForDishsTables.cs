namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPriceColumnForDishsTables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Dishes", "Price", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Dishes", "Price");
        }
    }
}

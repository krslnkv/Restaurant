namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixDishNameColumn : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Dishes", "Name", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Dishes", "Name", c => c.Int(nullable: false));
        }
    }
}

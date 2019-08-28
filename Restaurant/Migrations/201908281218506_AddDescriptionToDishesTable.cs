namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDescriptionToDishesTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Dishes", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Dishes", "Description");
        }
    }
}

namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsClosedColumnToShiftsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shifts", "IsClosed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Shifts", "IsClosed");
        }
    }
}

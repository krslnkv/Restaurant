namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLastShiftColumnToWaitersTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Waiters", "LastShift", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Waiters", "LastShift");
        }
    }
}

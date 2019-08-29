namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveLastShiftFromWaiters : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Waiters", "LastShift");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Waiters", "LastShift", c => c.DateTime(nullable: false));
        }
    }
}

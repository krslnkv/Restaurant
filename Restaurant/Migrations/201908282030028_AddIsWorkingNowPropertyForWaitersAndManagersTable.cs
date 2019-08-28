namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsWorkingNowPropertyForWaitersAndManagersTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Managers", "IsWorkingNow", c => c.Boolean(nullable: false));
            AddColumn("dbo.Waiters", "IsWorkingNow", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Waiters", "IsWorkingNow");
            DropColumn("dbo.Managers", "IsWorkingNow");
        }
    }
}

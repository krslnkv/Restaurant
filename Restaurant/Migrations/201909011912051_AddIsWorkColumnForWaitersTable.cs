namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsWorkColumnForWaitersTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Waiters", "IsWork", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Waiters", "IsWork");
        }
    }
}

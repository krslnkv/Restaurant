namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsBookedColumnForTaibles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tables", "IsBooked", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tables", "IsBooked");
        }
    }
}

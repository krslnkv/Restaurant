namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsShowColumnForTablesTable1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tables", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tables", "Name", c => c.String());
        }
    }
}

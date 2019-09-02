namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsShowColumnForTablesTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tables", "IsShow", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tables", "IsShow");
        }
    }
}

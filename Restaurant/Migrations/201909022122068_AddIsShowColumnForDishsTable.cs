namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsShowColumnForDishsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Dishes", "IsShow", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Dishes", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Dishes", "Description", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Dishes", "Description", c => c.String());
            AlterColumn("dbo.Dishes", "Name", c => c.String());
            DropColumn("dbo.Dishes", "IsShow");
        }
    }
}

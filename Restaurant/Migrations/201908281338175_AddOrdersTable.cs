namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrdersTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShiftId = c.Int(nullable: false),
                        TableId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Shifts", t => t.ShiftId, cascadeDelete: true)
                .ForeignKey("dbo.Tables", t => t.TableId, cascadeDelete: true)
                .Index(t => t.ShiftId)
                .Index(t => t.TableId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "TableId", "dbo.Tables");
            DropForeignKey("dbo.Orders", "ShiftId", "dbo.Shifts");
            DropIndex("dbo.Orders", new[] { "TableId" });
            DropIndex("dbo.Orders", new[] { "ShiftId" });
            DropTable("dbo.Orders");
        }
    }
}

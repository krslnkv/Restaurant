namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShiftsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Shifts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ManagerId = c.Int(nullable: false),
                        WaiterId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        ExpDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Managers", t => t.ManagerId, cascadeDelete: true)
                .ForeignKey("dbo.Waiters", t => t.WaiterId, cascadeDelete: true)
                .Index(t => t.ManagerId)
                .Index(t => t.WaiterId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Shifts", "WaiterId", "dbo.Waiters");
            DropForeignKey("dbo.Shifts", "ManagerId", "dbo.Managers");
            DropIndex("dbo.Shifts", new[] { "WaiterId" });
            DropIndex("dbo.Shifts", new[] { "ManagerId" });
            DropTable("dbo.Shifts");
        }
    }
}

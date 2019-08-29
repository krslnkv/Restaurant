namespace Restaurant.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddManyToManyRelationForWaiterAndShift : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Shifts", "WaiterId", "dbo.Waiters");
            DropIndex("dbo.Shifts", new[] { "WaiterId" });
            CreateTable(
                "dbo.WaiterShifts",
                c => new
                    {
                        Waiter_Id = c.Int(nullable: false),
                        Shift_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Waiter_Id, t.Shift_Id })
                .ForeignKey("dbo.Waiters", t => t.Waiter_Id, cascadeDelete: true)
                .ForeignKey("dbo.Shifts", t => t.Shift_Id, cascadeDelete: true)
                .Index(t => t.Waiter_Id)
                .Index(t => t.Shift_Id);
            
            DropColumn("dbo.Shifts", "WaiterId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Shifts", "WaiterId", c => c.Int(nullable: false));
            DropForeignKey("dbo.WaiterShifts", "Shift_Id", "dbo.Shifts");
            DropForeignKey("dbo.WaiterShifts", "Waiter_Id", "dbo.Waiters");
            DropIndex("dbo.WaiterShifts", new[] { "Shift_Id" });
            DropIndex("dbo.WaiterShifts", new[] { "Waiter_Id" });
            DropTable("dbo.WaiterShifts");
            CreateIndex("dbo.Shifts", "WaiterId");
            AddForeignKey("dbo.Shifts", "WaiterId", "dbo.Waiters", "Id", cascadeDelete: true);
        }
    }
}

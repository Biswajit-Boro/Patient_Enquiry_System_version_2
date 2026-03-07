namespace Patient_Enquiry_System_version_2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddTicketsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tickets",
                c => new
                {
                    TicketId = c.Int(nullable: false, identity: true),
                    PatientId = c.String(nullable: false, maxLength: 128),
                    Status = c.String(),
                    CreatedAt = c.DateTime(nullable: false),
                    ClosedAt = c.DateTime(),
                })
                .PrimaryKey(t => t.TicketId);
        }

        public override void Down()
        {
            DropTable("dbo.Tickets");
        }
    }
}
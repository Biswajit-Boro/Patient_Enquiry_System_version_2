namespace Patient_Enquiry_System_version_2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddTicketMessagesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TicketMessages",
                c => new
                {
                    TicketMessageId = c.Int(nullable: false, identity: true),
                    TicketId = c.Int(nullable: false),
                    SenderType = c.String(nullable: false, maxLength: 20),
                    MessageText = c.String(nullable: false),
                    SentAt = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.TicketMessageId);
        }

        public override void Down()
        {
            DropTable("dbo.TicketMessages");
        }
    }
}
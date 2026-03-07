namespace Patient_Enquiry_System_version_2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFileUrlToTicketMessages1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicketMessages", "FileUrl", c => c.String());
            AddColumn("dbo.TicketMessages", "FileType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TicketMessages", "FileType");
            DropColumn("dbo.TicketMessages", "FileUrl");
        }
    }
}

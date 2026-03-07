namespace Patient_Enquiry_System_version_2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPriorityToTicket : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "Priority", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tickets", "Priority");
        }
    }
}

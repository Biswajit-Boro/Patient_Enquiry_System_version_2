namespace Patient_Enquiry_System_version_2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            // Add new structured fields
            AddColumn("dbo.Enquiries", "Name", c => c.String());
            AddColumn("dbo.Enquiries", "Phone", c => c.String());
            AddColumn("dbo.Enquiries", "Issue", c => c.String());
            AddColumn("dbo.Enquiries", "Status", c => c.String());

            // Remove old raw message column
            DropColumn("dbo.Enquiries", "MessageText");
        }

        public override void Down()
        {
            // Restore old column if rollback happens
            AddColumn("dbo.Enquiries", "MessageText", c => c.String());

            // Remove structured fields
            DropColumn("dbo.Enquiries", "Status");
            DropColumn("dbo.Enquiries", "Issue");
            DropColumn("dbo.Enquiries", "Phone");
            DropColumn("dbo.Enquiries", "Name");
        }
    }
}

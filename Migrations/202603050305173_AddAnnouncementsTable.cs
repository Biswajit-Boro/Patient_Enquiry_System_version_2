namespace Patient_Enquiry_System_version_2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAnnouncementsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Announcements",
                c => new
                    {
                        AnnouncementId = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        SentAt = c.DateTime(nullable: false),
                        TotalSent = c.Int(nullable: false),
                        TotalFailed = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AnnouncementId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Announcements");
        }
    }
}

namespace Patient_Enquiry_System_version_2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPatientsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Patients",
                c => new
                    {
                        PatientId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        TelegramUserId = c.Long(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PatientId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Patients");
        }
    }
}

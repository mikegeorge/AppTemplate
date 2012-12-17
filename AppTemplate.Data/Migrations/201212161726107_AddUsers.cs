namespace AppTemplate.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUsers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        PasswordSalt = c.String(),
                        PasswordHash = c.String(),
                        UserRole = c.Int(nullable: false),
                        Disabled = c.Boolean(nullable: false),
                        Comment = c.String(),
                        Agreement = c.Boolean(nullable: false),
                        PasswordNeedsUpdating = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateLastLogin = c.DateTime(),
                        DateLastPasswordChange = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
        }
    }
}

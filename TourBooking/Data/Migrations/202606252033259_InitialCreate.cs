namespace TourBooking.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BookingDetails",
                c => new
                    {
                        DetailId = c.Int(nullable: false, identity: true),
                        BookingId = c.Int(nullable: false),
                        PassengerType = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.DetailId)
                .ForeignKey("dbo.Bookings", t => t.BookingId, cascadeDelete: true)
                .Index(t => t.BookingId);
            
            CreateTable(
                "dbo.Bookings",
                c => new
                    {
                        BookingId = c.Int(nullable: false, identity: true),
                        BookingCode = c.String(nullable: false, maxLength: 50),
                        TourId = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        StaffId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BookingDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.BookingId)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .ForeignKey("dbo.Staffs", t => t.StaffId, cascadeDelete: true)
                .ForeignKey("dbo.Tours", t => t.TourId, cascadeDelete: true)
                .Index(t => t.TourId)
                .Index(t => t.CustomerId)
                .Index(t => t.StaffId);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerId = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false, maxLength: 100),
                        Phone = c.String(nullable: false, maxLength: 20),
                        Email = c.String(maxLength: 100),
                        NationalId = c.String(maxLength: 20),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerId);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        PaymentId = c.Int(nullable: false, identity: true),
                        BookingId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Method = c.Int(nullable: false),
                        PaidAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PaymentId)
                .ForeignKey("dbo.Bookings", t => t.BookingId, cascadeDelete: true)
                .Index(t => t.BookingId);
            
            CreateTable(
                "dbo.Staffs",
                c => new
                    {
                        StaffId = c.Int(nullable: false, identity: true),
                        StaffCode = c.String(nullable: false, maxLength: 50),
                        FullName = c.String(nullable: false, maxLength: 100),
                        Username = c.String(nullable: false, maxLength: 50),
                        PasswordHash = c.String(nullable: false),
                        Role = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.StaffId);
            
            CreateTable(
                "dbo.Tours",
                c => new
                    {
                        TourId = c.Int(nullable: false, identity: true),
                        TourCode = c.String(nullable: false, maxLength: 50),
                        TourName = c.String(nullable: false, maxLength: 200),
                        Destination = c.String(nullable: false, maxLength: 100),
                        DepartureDate = c.DateTime(nullable: false),
                        ReturnDate = c.DateTime(nullable: false),
                        PriceAdult = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PriceChild = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalSlots = c.Int(nullable: false),
                        AvailableSlots = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TourId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bookings", "TourId", "dbo.Tours");
            DropForeignKey("dbo.Bookings", "StaffId", "dbo.Staffs");
            DropForeignKey("dbo.Payments", "BookingId", "dbo.Bookings");
            DropForeignKey("dbo.Bookings", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.BookingDetails", "BookingId", "dbo.Bookings");
            DropIndex("dbo.Payments", new[] { "BookingId" });
            DropIndex("dbo.Bookings", new[] { "StaffId" });
            DropIndex("dbo.Bookings", new[] { "CustomerId" });
            DropIndex("dbo.Bookings", new[] { "TourId" });
            DropIndex("dbo.BookingDetails", new[] { "BookingId" });
            DropTable("dbo.Tours");
            DropTable("dbo.Staffs");
            DropTable("dbo.Payments");
            DropTable("dbo.Customers");
            DropTable("dbo.Bookings");
            DropTable("dbo.BookingDetails");
        }
    }
}

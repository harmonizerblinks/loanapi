using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApi.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Sequence> Sequence { get; set; }
        public DbSet<Alert> Alert { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<AccountType> AccountType { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<Cot> Cot { get; set; }
        public DbSet<Charge> Charge { get; set; }
        public DbSet<Cheque> Cheque { get; set; }
        public DbSet<Session> Session { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Garantor> Garantor { get; set; }
        public DbSet<SmsApi> SmsApi { get; set; }
        public DbSet<Sms> Sms { get; set; }
        public DbSet<SmsBoardcast> SmsBoardcast { get; set; }
        public DbSet<MainNominal> MainNominal { get; set; }
        public DbSet<Nominal> Nominal { get; set; }
        public DbSet<Teller> Teller { get; set; }
        public DbSet<Transit> Transit { get; set; }
        public DbSet<Transaction> Transaction { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity<Sequence>().HasData(
            //    new Sequence { SequenceId = 1, Name = "Employee", Prefix = "EMP", Counter = 1, Length = 4, Date = DateTime.Now, UserId = "807ba6c0-e845-4695-847e-92edca9d66db" },
            //    new Sequence { SequenceId = 2, Name = "Customer", Prefix = "CUST", Counter = 1, Length = 4, Date = DateTime.Now, UserId = "807ba6c0-e845-4695-847e-92edca9d66db" },
            //    new Sequence { SequenceId = 3, Name = "Loan", Prefix = "100", Counter = 1, Length = 6, Date = DateTime.Now, UserId = "807ba6c0-e845-4695-847e-92edca9d66db" },
            //    new Sequence { SequenceId = 4, Name = "Savings", Prefix = "100", Counter = 1, Length = 6, Date = DateTime.Now, UserId = "807ba6c0-e845-4695-847e-92edca9d66db" },
            //    new Sequence { SequenceId = 5, Name = "Location", Prefix = "L", Counter = 1, Length = 4, Date = DateTime.Now, UserId = "807ba6c0-e845-4695-847e-92edca9d66db" },
            //    new Sequence { SequenceId = 6, Name = "Nominal", Prefix = "1", Counter = 1, Length = 3, Date = DateTime.Now, UserId = "807ba6c0-e845-4695-847e-92edca9d66db" },
            //    new Sequence { SequenceId = 7, Name = "AccountType", Prefix = "1", Counter = 1, Length = 34, Date = DateTime.Now, UserId = "807ba6c0-e845-4695-847e-92edca9d66db" },
            //    new Sequence { SequenceId = 8, Name = "Joint", Prefix = "JNT", Counter = 1, Length = 6, Date = DateTime.Now, UserId = "807ba6c0-e845-4695-847e-92edca9d66db" });
            //builder.Entity<Company>().HasData(
            //    new Company { CompanyId = 1, Code="Acyst001", Name="Dedawa Company Ltd", Mobile="08053476754",
            //        SessionDate = DateTime.Now, Expiry= DateTime.Now.AddYears(1),
            //        Postal ="102 Sapele Road", Address="Enter A Valid Address",
            //        Date = DateTime.Now, UserId = "807ba6c0-e845-4695-847e-92edca9d66db" });
            //builder.Entity<Session>().HasData(
            //    new Session { SessionId = 1, SessionDate = DateTime.Now.Date, Status = "Active", Date = DateTime.Now, UserId = "807ba6c0-e845-4695-847e-92edca9d66db" });
            //builder.Entity<Location>().HasData(
            //    new Location { LocationId = 1, Name = "Sapele Road", Code = "001", Date = DateTime.Now, UserId = "807ba6c0-e845-4695-847e-92edca9d66db" });

            //builder.Entity<AppUser>().HasData(
            //    new AppUser {
            //        UserName = "Acyst", NormalizedUserName = "Acyst", PhoneNumber = "0238288675",
            //        Email = "info@acyst.tech", NormalizedEmail = "INFO@ACYST.TECH",
            //        EmailConfirmed = true, Login = DateTime.UtcNow, LogOut = DateTime.UtcNow,
            //        MUserId = "807ba6c0-e845-4695-847e-92edca9d66db", UserType = "Admin", 
            //        MDate = DateTime.UtcNow, IsLoggedIn = false, AccessFailedCount = 0,
            //        SecurityStamp = "CJ2EVAYDU6HKQPMFCY7A3ROLDIQNWNRM",
            //        ConcurrencyStamp = "073d68a5-ad7d-4312-9b25-d86d1a604329",
            //        PasswordHash = "AQAAAAEAACcQAAAAEOrzhtqT+9ZuBKNmqNY/4xVf8ruHsdTHMfE8KCVsipUBA9CjcZMVjYRaR0Nzl8jgKQ=="
            //    }/*, new AppUser {
            //        UserName = "Harmony", NormalizedUserName = "HARMONY", PhoneNumber = "0238288675",
            //        Email = "harmonizerblinks@gmail.com", NormalizedEmail = "HARMONIZERBLINKS@GMAIL.COM",
            //        EmailConfirmed = true, Login = DateTime.UtcNow, LogOut = DateTime.UtcNow,
            //        UserType = "Admin", MUserId = "807ba6c0-e845-4695-847e-92edca9d66db",
            //        MDate = DateTime.UtcNow, IsLoggedIn = false, AccessFailedCount = 0,
            //        SecurityStamp = "CJ2EVAYDU6HKQPMFCY7A3ROLDIQNWNRM",
            //        ConcurrencyStamp = "073d68a5-ad7d-4312-9b25-d86d1a604329",
            //        PasswordHash = "AQAAAAEAACcQAAAAEOrzhtqT+9ZuBKNmqNY/4xVf8ruHsdTHMfE8KCVsipUBA9CjcZMVjYRaR0Nzl8jgKQ=="
            //   }*/);
            //builder.Entity<MainNominal>().HasData(
            //    new MainNominal { MainNominalId=1, Code="100", Name="Income", Status = "Active", Date = DateTime.Now, UserId = "807ba6c0-e845-4695-847e-92edca9d66db" },
            //    new MainNominal { MainNominalId = 2, Code = "101", Name = "Expense", Status = "Active", Date = DateTime.Now, UserId = "807ba6c0-e845-4695-847e-92edca9d66db" },
            //    new MainNominal { MainNominalId = 3, Code = "102", Name = "Liability", Status = "Active", Date = DateTime.Now, UserId = "807ba6c0-e845-4695-847e-92edca9d66db" });

            base.OnModelCreating(builder);
        }
    }
}

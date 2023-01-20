Create Database:  
dotnet ef migrations add InitialCreate --project DAL.Database --startup-project ConsoleApp  
dotnet ef database update --project DAL.Database --startup-project ConsoleApp
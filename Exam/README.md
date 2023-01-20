Create Database:  
dotnet ef migrations add InitialCreate --project DAL --startup-project WebApp  
dotnet ef database update --project DAL --startup-project WebApp
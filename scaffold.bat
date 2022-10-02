@echo off
cd FBus.Data
dotnet ef dbcontext scaffold "Server=fbus.database.windows.net;Database=FBus;TrustServerCertificate=true;User Id=fbus;Password=Capstone@123" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context-dir Context --force --no-onconfiguring
if %errorlevel%==0 (echo Done!) else (echo Failed!)
pause
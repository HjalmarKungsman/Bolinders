# Bolinders

Installation:

##### 1. Create appsettings.json in Bolinders.Web/
```cs
{
  "ConnectionStrings": {
    "BolindersVehicles": "Server=.\\SQL2016;Database=Bolinders;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

##### 2. Create ApplicationDbContextFactory.cs in Bolinders.Web/DataAccess/
```cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bolinders.Web.DataAccess
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Server=.\\SQL2016;Database=Bolinders;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
```

##### 3. Change path to the SQL-server
##### 4. Create new database in SMSS named "Bolinders"
##### 5. Delete the Migrations-folder
##### 6. Run "Add-Migration Initial" in the NPM konsol
##### 7. Run "Update-Database" in the NPM konsol
##### 8. Build and Run project

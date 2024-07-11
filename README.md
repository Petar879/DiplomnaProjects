# DiplomnaProjects

The project files used for my Bachelor's Degree POS system.

### Technologies 
- .NET MAUI(.NET 8)
- Entity Framework(.NET 8)
- MySQL
- ASP .NET Core API(.NET 8)

### Running the project
1. In a MySQL server add these [databases](DatabaseFiles);
2. Update the connection string for [POS Project](EfLibrary/Data/DiplomnaContext.cs#L41) and [ASP Authentication Server](DotNet8Authentication/Program.cs#L17);
3. Run the Authention server;
4. Run the DiplomnaPOS App;

### Additional notes
- The project is primary maid on Windows;
- All NFC functionality has been tested with [ACR122U](https://www.acs.com.hk/en/products/3/acr122u-usb-nfc-reader/);
- The project can compile and run on Android, but NFC and other functionalities don't work;

// Coding Challenge - Backend Engineer - Blockchain event stream processor
// Author: Andy Lower
// Date:   06/10/2023

// ** References **   
// Basic setup - https://learn.microsoft.com/en-us/visualstudio/get-started/csharp/tutorial-console?view=vs-2022
// JSON Library - https://www.nuget.org/packages/newtonsoft.json/
// SQLite for DB - https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/?tabs=netcore-cli

// ** Notes **
// - Followed some suggestions from VS (stripping out declaration of the Program class and one-line using statements)
// - I went with SQLite as storing in a file just didn't seem very robust when dealing with concurrent read/writes
// - Database class needs a tidy up. Needs separation of DB and eventhandler code.
// - Would add tests with more time to learn a framework
// - Would be simple enough to store the transaction detail, too, but didn't as it wasn't required for the task
// - When an NFT is burned, I set the address to an empty string in the DB. 
//   Seemed better to have a record that it existed (as it's technically not destroyed) and saved me having to check the transaction type
//   A good argument for checking the transaction type in production, given that there may be a mistake in the data provided

using EventStreamProcessor;

if (args.Length == 0)
    return;

string arg = string.Join(" ", args);

if (arg.StartsWith("--"))
{
    var value = args.Length > 1 ? args[1] : "";
    Switch(args[0], value);
}

static void Switch(string _arg, string _value)
{
    switch (_arg)
    {
        case "--read-inline":
            EventHandlers.ReadInline(_value);
            break;
        case "--read-file":
            EventHandlers.ReadFile(_value);
            break;
        case "--wallet":
            EventHandlers.GetOwnerTokens(_value);
            break;
        case "--nft":
            EventHandlers.GetOwner(_value);
            break;
        case "--reset":
            EventHandlers.ResetData();
            break;
    }
}
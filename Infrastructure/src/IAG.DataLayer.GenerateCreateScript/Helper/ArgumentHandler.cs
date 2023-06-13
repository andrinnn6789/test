using System;
using System.Linq;

using IAG.Infrastructure.DataLayer.Context;

namespace IAG.DataLayer.GenerateCreateScript.Helper;

public class ArgumentHandler
{
    public const DatabaseType DefaultDatabaseType = DatabaseType.Postgres;

    public string DbContextAssemblyPath { get; }
    public string DbContextType { get; }
    public DatabaseType Database { get; }

    public bool ShowHelp { get; }

    public ArgumentHandler(string[] args)
    {
        DbContextAssemblyPath = null;
        DbContextType = null;
        Database = DefaultDatabaseType;

        var firstArg = args.FirstOrDefault();
        if (firstArg == null)
        {
            return;
        }

        if (firstArg.Equals("help", StringComparison.InvariantCultureIgnoreCase))
        {
            ShowHelp = true;
            return;
        }

        if (Enum.TryParse(firstArg, true, out DatabaseType dbType))
        {
            Database = dbType;
            return;
        }

        var atPos = firstArg.IndexOf('@');
        if (atPos >= 0)
        {
            DbContextType = firstArg.Substring(0, atPos);
            DbContextAssemblyPath = firstArg.Substring(atPos+1);
        }
        else
        {
            DbContextAssemblyPath = firstArg;
        }

        if ((args.Length > 1) && Enum.TryParse(args[1], true, out dbType))
        {
            Database = dbType;
        }
    }
}
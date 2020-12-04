using System;
using System.Data.SqlClient;

namespace BCCompressionCli
{
    class Program
    {
        private const int CommandTimeout = 60 * 10;
       
        static void Main(string[] args)
        {
            var currentConsoleBackground = Console.BackgroundColor;
            var currentConolseForeground = Console.ForegroundColor;
            try
            {
                if (args.Length < 1 || args.Length > 3 || (args.Length > 0 && args[0] == "-?"))
                {
                    PrintUsage();
                    return;
                }

                Console.BackgroundColor = ConsoleColor.Black;

                string dbName = args[0];
                string serverName = Environment.MachineName;
                string nbOfTables = "10";

                if (args.Length > 1)
                {
                    nbOfTables = args[1];
                }

                if (args.Length > 2)
                {
                    serverName = args[2];
                }

                string sqlConnectionString = $@"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog={dbName};Data Source={serverName}";
                using (SqlConnection connection = new SqlConnection(sqlConnectionString))
                {
                    connection.Open();

                    Console.WriteLine();
                    GetTopNBigTables(connection, nbOfTables);
                    Console.WriteLine();
                    CompressionReport(connection, nbOfTables);
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An error as occurred:");
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Console.BackgroundColor = currentConsoleBackground;
                Console.ForegroundColor = currentConolseForeground;
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("BCCompressionCli <databaseName> [<nbOfTables>] [<serverName>]");
            Console.WriteLine("<databaseName>: The name of the database to perform compression estimation on.");
            Console.WriteLine("<nbOfTables>: Optional parameter. By default information for the top 10 tables are shown.");
            Console.WriteLine("<serverName>: Optional parameter. The name of the SQL server on which the database is mounted.");
            Console.WriteLine("              If not provided, the server is assumed to be running on the local machine.");
        }

        private static void GetTopNBigTables(SqlConnection connection, string nbOfTables)
        {
            string script = string.Format(Resource1.SelectTopNBigTables, nbOfTables);
            SqlCommand command = new SqlCommand(script, connection);
            command.CommandTimeout = CommandTimeout;
            SqlDataReader reader = command.ExecuteReader();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Top {nbOfTables} biggest tables");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0,-30} {1,-30} {2,15:N1} {3,15:N1}", "Company", "Table", "Used(mb)", "Allocated(mb)");
            Console.ForegroundColor = ConsoleColor.White;
            while (reader.Read())
            {
                var tableInfo = SplitTableName(reader[0].ToString());
                Console.WriteLine("{0,-30} {1,-30} {2,15:N1} {3,15:N1}", tableInfo[0], tableInfo[1], reader[1], reader[2]);
            }

            reader.Close();
        }

        private static void CompressionReport(SqlConnection connection, string nbOfTables)
        {
            string script = string.Format(Resource1.CompressionReport, nbOfTables);
            SqlCommand command = new SqlCommand(script, connection);
            command.CommandTimeout = CommandTimeout;

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Estimating compression");
            Console.WriteLine("This can take several minutes, please wait....");
            Console.WriteLine();

            SqlDataReader reader = command.ExecuteReader();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Compression estimates for the top {nbOfTables} biggest tables");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            string format = "{0,-30} {1,-30} {2,20:N0} {3,25:N0} {4,20:N0}";
            Console.WriteLine(format, "Company", "Table", "Average", "Average size with", "Average saving(mb)");
            Console.WriteLine(format, "", "", "current size(mb)", "compression enabled(mb)", "");
            Console.ForegroundColor = ConsoleColor.White;
            while (reader.Read())
            {
                var tableInfo = SplitTableName(reader[1].ToString());
                Console.WriteLine(format, tableInfo[0], tableInfo[1], reader[2], reader[3], reader[4]);
            }

            reader.Close();
        }

        private static string[] SplitTableName(string tableFullName)
        {
            int idx1 = tableFullName.IndexOf('$');
            string company = string.Empty;
            string tableName = tableFullName;
            if (idx1 > -1)
            {
                company = tableFullName.Substring(0, idx1);
                int idx2 = tableFullName.IndexOf('$', idx1 + 1);

                if (idx2 > -1)
                {
                    tableName = tableFullName.Substring(idx1 + 1, idx2 - idx1 - 1);
                }
                else
                {
                    tableName = tableFullName.Substring(idx1 + 1);
                }
            }

            string[] result = new string[2];
            result[0] = company;
            result[1] = tableName;
            return result;
        }
    }
}

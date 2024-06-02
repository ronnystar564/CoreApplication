using System;
using System.IO;
using System.Net;
using System.IO.Compression;
using Npgsql;

namespace HOnkHonk3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string desiredLocation = @"C:\Users\rstar\AppData\Local\Temp"; // Specify your desired location here
            string tempFolderName = "ONSPD"; // Generate a unique folder name
            string tempFolderPath = Path.Combine(desiredLocation, tempFolderName); ; // Temporary folder to store downloaded zip file and extracted CSVs
            string zipFilePath = Path.Combine(tempFolderPath, "2022-11.zip");
            string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=test;Database=honkhonk"; // Replace this string with your database connection string
            Console.WriteLine("Please wait, downloading data...");
            try
            {
                Directory.CreateDirectory(tempFolderPath);
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile("https://parlvid.mysociety.org/os/ONSPD/2022-11.zip", zipFilePath);
                }

                // Extract the contents of the zip file
                ZipFile.ExtractToDirectory(zipFilePath, tempFolderPath);

                string multiCsvFolderPath = Path.Combine(tempFolderPath, "Data\\multi_csv");
                string[] csvFiles = Directory.GetFiles(multiCsvFolderPath, "*.csv");

                Console.WriteLine("Processing CSV files...");
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    foreach (string csvFile in csvFiles)
                    {
                        InsertCSVDataIntoPostgreSQL(csvFile, connection);
                    }
                }

                Console.WriteLine("Data inserted into PostgreSQL successfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            finally
            {
                // Clean up - delete temporary folder and zip file
                 Directory.Delete(tempFolderPath, true);
            }

            Console.ReadLine();
        }

        static void InsertCSVDataIntoPostgreSQL(string filePath, NpgsqlConnection connection)
        {
            long NumberofData = 0;
            using (var reader = new StreamReader(filePath))
            {
                reader.ReadLine(); // Skip header line

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var fields = line.Split(',');

                    InsertDataIntoPostgreSQL(fields, connection);
                    NumberofData++;
                }
                Console.WriteLine(NumberofData + " Data added from " + Path.GetFileName(filePath));
            }
        }

        static void InsertDataIntoPostgreSQL(string[] fields, NpgsqlConnection connection)
        {
            var insertQuery = "INSERT INTO HONKHONK (pcd, pcd2, pcds, dointr, doterm, oscty," +
               "ced, oslaua, osward, parish, usertype, oseast1m," +
               "osnrth1m, osgrdind, oshlthau, nhser, ctry, rgn, streg, pcon, eer, teclec," +
               "ttwa, pct, itl, statsward, oa01, casward, park, lsoa01, msoa01, ur01ind," +
               "oac01, oa11, lsoa11, msoa11, wz11, ccg, bua11, uasd11, ru11ind, oac11," +
               "lat, long, lep1, lep2, pfa, imd, calncv, stp, oa21, lsoa21, msoa21) " +
               "VALUES (@param1, @param2, @param3, @param4, @param5, @param6, @param7, @param8, @param9," +
               "@param10, @param11, @param12, @param13, @param14, @param15, @param16, @param17, @param18, @param19, @param20, @param21, @param22, @param23, @param24," +
               "@param25, @param26, @param27, @param28, @param29, @param30, @param31, @param32, @param33, @param34, @param35, @param36, @param37, @param38, @param39, @param40, @param41," +
               "@param42, @param43, @param44, @param45, @param46, @param47, @param48, @param49, @param50, @param51, @param52, @param53)";

            using (var cmd = new NpgsqlCommand(insertQuery, connection))
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    cmd.Parameters.AddWithValue($"@param{i + 1}", fields[i].Replace("\"", ""));
                }

                cmd.ExecuteNonQuery();
            }
        }
    }
}

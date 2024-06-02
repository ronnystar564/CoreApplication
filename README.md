Project Overview
This project involves creating a .NET console application to fetch postcode data from a CSV file provided by My Society, import the data into a PostgreSQL database, and expose it through an API for easy access and interaction.

Step-by-Step Description

The project begins with setting up the console application. A new .NET Core 6 console application project is created, and necessary packages are installed, including those for PostgreSQL database interaction and handling ZIP files.

Next, file paths and the database connection string are specified. The desired download location and a temporary folder name are set. The path for the downloaded ZIP file is defined, and the connection string for the PostgreSQL database is established.

The project then proceeds to download the ZIP file. Using `WebClient`, the ZIP file is downloaded from the specified URL. Once the ZIP file is downloaded, its contents are extracted to a temporary folder using `ZipFile.ExtractToDirectory`.

With the ZIP file extracted, the folder containing the CSV files is located. Each CSV file is processed iteratively.

A connection to the PostgreSQL database is opened using `NpgsqlConnection`. For each CSV file, its content is read line by line, skipping the header line. Each line is split into fields, and the data is inserted into the PostgreSQL database using an `INSERT` query.

To insert the data, an `INSERT` SQL query is constructed with appropriate parameters. `NpgsqlCommand` is used to execute the query and insert the data into the database. After processing all the CSV files, the database connection is properly closed.

Finally, clean-up operations are performed. The temporary folder and the downloaded ZIP file are deleted to free up space.

This process ensures that postcode data is efficiently fetched from the CSV file, stored in a PostgreSQL database, and prepared for further interaction through an API.

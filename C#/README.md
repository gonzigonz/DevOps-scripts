# Microsoft SQL Blob Downloader (MSBD)
*Requires .NET 3.5*

Simple Windows app that allows you to pass a query and download the files in a given field.

### HELP
* *Connection String* - Connection string to MS SQL Server.
* *SQL Query String* - A SQL query that returns at minimun a column containing the "Blob/s" to download and a column providing the "Filename" to use when saving the blob.
* *Download Path* - A path to save all the blobs from the query.
* *Blob Column Name* - The name of the column form the query containing the Blob object (Binary Array).
* *Filename Column Name* - The name of the column providing the File Name (String) to use for saving each blob.

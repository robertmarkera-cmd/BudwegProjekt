using Budweg.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;

namespace Budweg.View_Models
{
    public class CaliberRepository
    {
        private readonly string ConnectionString;

        private List<Caliper> _caliper = new List<Caliper>();

        public CaliberRepository()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("jsconfig1.json")
            .Build();

            ConnectionString = config.GetConnectionString("MyDBConnection");
        }
        public void AddCaliber(Caliper caliperToBeCreated)
        {
            // Use the stored procedure defined in BudwegDB: dbo.Caliber_Add_Select
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                // Ensure we have a valid BatchID. The DB requires Caliber.BatchID NOT NULL and FK to Batch.
                int batchId = caliperToBeCreated.FrameID; // caller may provide as BatchID

                if (batchId <= 0)
                {
                    // create a minimal Workstation to satisfy Batch.WorkstationID FK, then create a Batch
                    int workstationId;
                    using (SqlCommand cmdWs = new SqlCommand("INSERT INTO Workstation (WorkstationName) VALUES (@Name); SELECT CAST(SCOPE_IDENTITY() AS INT);", con))
                    {
                        cmdWs.Parameters.Add("@Name", SqlDbType.NVarChar, 80).Value = "AutoCreated";
                        workstationId = Convert.ToInt32(cmdWs.ExecuteScalar());
                    }

                    int newBatchId;
                    using (SqlCommand cmdBatch = new SqlCommand("INSERT INTO Batch ([Comment], [Picture], [Amount], [WorkstationID]) VALUES (@Comment, @Picture, @Amount, @WorkstationID); SELECT CAST(SCOPE_IDENTITY() AS INT);", con))
                    {
                        cmdBatch.Parameters.Add("@Comment", SqlDbType.NVarChar, 255).Value = (object?)caliperToBeCreated.Comment ?? DBNull.Value;

                        if (!string.IsNullOrEmpty(caliperToBeCreated.Picture))
                        {
                            try
                            {
                                var bytes = System.IO.File.ReadAllBytes(caliperToBeCreated.Picture);
                                cmdBatch.Parameters.Add("@Picture", SqlDbType.VarBinary, -1).Value = bytes;
                            }
                            catch
                            {
                                cmdBatch.Parameters.Add("@Picture", SqlDbType.VarBinary, -1).Value = DBNull.Value;
                            }
                        }
                        else
                        {
                            cmdBatch.Parameters.Add("@Picture", SqlDbType.VarBinary, -1).Value = DBNull.Value;
                        }

                        cmdBatch.Parameters.Add("@Amount", SqlDbType.Int).Value = DBNull.Value;
                        cmdBatch.Parameters.Add("@WorkstationID", SqlDbType.Int).Value = workstationId;

                        newBatchId = Convert.ToInt32(cmdBatch.ExecuteScalar());
                    }

                    batchId = newBatchId;
                }

                using (SqlCommand cmd = new SqlCommand("dbo.Caliber_Add_Select", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Map model -> stored procedure parameters
                    cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 80).Value = (object?)caliperToBeCreated.Type ?? DBNull.Value;
                    cmd.Parameters.Add("@Brand", SqlDbType.NVarChar, 80).Value = (object?)caliperToBeCreated.Manufacturer ?? DBNull.Value;
                    cmd.Parameters.Add("@Comment", SqlDbType.NVarChar, 255).Value = (object?)caliperToBeCreated.Comment ?? DBNull.Value;

                    // Picture in DB is varbinary(max). If model.Picture is a file path, read bytes; if null/empty send DBNull.
                    if (!string.IsNullOrEmpty(caliperToBeCreated.Picture))
                    {
                        try
                        {
                            var bytes = System.IO.File.ReadAllBytes(caliperToBeCreated.Picture);
                            cmd.Parameters.Add("@Picture", SqlDbType.VarBinary, -1).Value = bytes;
                        }
                        catch
                        {
                            // If reading fails, send NULL to DB
                            cmd.Parameters.Add("@Picture", SqlDbType.VarBinary, -1).Value = DBNull.Value;
                        }
                    }
                    else
                    {
                        cmd.Parameters.Add("@Picture", SqlDbType.VarBinary, -1).Value = DBNull.Value;
                    }

                    // ItemNumber maps to Stelnummer (ItemNumber may be null)
                    if (caliperToBeCreated.FrameID > 0)
                        cmd.Parameters.Add("@ItemNumber", SqlDbType.Int).Value = caliperToBeCreated.FrameID;
                    else
                        cmd.Parameters.Add("@ItemNumber", SqlDbType.Int).Value = DBNull.Value;

                    cmd.Parameters.Add("@BatchID", SqlDbType.Int).Value = batchId;

                    // Execute and get new CaliberID
                    var result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out var newId))
                    {
                        caliperToBeCreated.FrameID = newId;
                    }
                }
            }
        }
    }
}

using Budweg.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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

        public void AddCaliper(Caliper caliper, int batchId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("spAddCaliber", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Standard parametre (med DBNull tjek, hvis værdien er null)
                    cmd.Parameters.AddWithValue("@Type", caliper.Type ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Manufacturer", caliper.Manufacturer ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Comment", caliper.Comment ?? (object)DBNull.Value);

                   
                    cmd.Parameters.AddWithValue("@BatchID", batchId);

                    // Håndtering af Billede (Konverter Bitmap til byte-array)
                    byte[] imageBytes = null;
                    if (caliper.Picture != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            // Gem som PNG i hukommelsen
                            caliper.Picture.Save(ms, ImageFormat.Png);
                            imageBytes = ms.ToArray();
                        }
                    }

                    // Tilføj VARBINARY parameter
                    var picParam = cmd.Parameters.Add("@Picture", SqlDbType.VarBinary, -1);
                    picParam.Value = imageBytes != null ? (object)imageBytes : DBNull.Value;

                    
                    conn.Open();

                   
                    int newFrameId = (int)cmd.ExecuteScalar();

                    caliper.FrameID = newFrameId;
                }
            }
        }
    }
}



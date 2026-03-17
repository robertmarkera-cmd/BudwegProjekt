using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing; 
using System.IO;
using System.Text.Json;
using Budweg.Models;
using Microsoft.Data.SqlClient;

namespace Budweg.View_Models
{
    public class CaliberRepository
    {
        private readonly string ConnectionString;

        private readonly List<Caliper> _caliper = new List<Caliper>();

        public CaliberRepository()
        {
            var baseDir = AppContext.BaseDirectory;
            var cfgPath = Path.Combine(baseDir, "jsconfig1.json");

            if (!File.Exists(cfgPath))
            {
                cfgPath = "jsconfig1.json";
            }

            if (File.Exists(cfgPath))
            {
                var json = File.ReadAllText(cfgPath);
                using var doc = JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty("ConnectionStrings", out var cs)
                    && cs.TryGetProperty("MyDBConnection", out var conn))
                {
                    ConnectionString = conn.GetString() ?? string.Empty;
                }
                else
                {
                    ConnectionString = string.Empty;
                }
            }
            else
            {
                ConnectionString = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'MyDBConnection' blev ikke fundet i jsconfig1.json.");
            }
        }

        public void AddCaliber(Caliper caliperToBeCreated)
        {
            if (caliperToBeCreated is null)
                throw new ArgumentNullException(nameof(caliperToBeCreated));

            byte[]? pictureBytes = null;
            if (caliperToBeCreated.Picture != null)
            {
                using (var ms = new MemoryStream())
                {
                    caliperToBeCreated.Picture.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    pictureBytes = ms.ToArray();
                }
            }

            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();

                using (var cmd = new SqlCommand("dbo.CreateCaliber", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 80)
                       .Value = (object?)caliperToBeCreated.Type ?? DBNull.Value;

                    cmd.Parameters.Add("@Comment", SqlDbType.NVarChar, 255)
                       .Value = (object?)caliperToBeCreated.Comment ?? DBNull.Value;

                    var picParam = cmd.Parameters.Add("@Picture", SqlDbType.VarBinary, -1);
                    picParam.Value = (object?)pictureBytes ?? DBNull.Value;

                    cmd.Parameters.Add("@ItemNumber", SqlDbType.Int)
                       .Value = caliperToBeCreated.FrameID;

                    cmd.Parameters.Add("@Brand", SqlDbType.NVarChar, 80)
                       .Value = (object?)caliperToBeCreated.Manufacturer ?? DBNull.Value;

                    cmd.Parameters.Add("@BatchID", SqlDbType.Int)
                       .Value = DBNull.Value;

                    var result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out var newId))
                    {
                        caliperToBeCreated.FrameID = newId;
                    }

                    _caliper.Add(caliperToBeCreated);
                }
            }
        }
    }
}

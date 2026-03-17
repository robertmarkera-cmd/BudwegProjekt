using Budweg.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.IO;
using System.Text.Json;

namespace Budweg.View_Models
{
    public class CaliberRepository
    {
        private readonly string ConnectionString;

        public CaliberRepository()
        {
            var json = File.ReadAllText("jsconfig1.json");
            var doc = JsonDocument.Parse(json);
            ConnectionString =
                doc.RootElement
                   .GetProperty("ConnectionStrings")
                   .GetProperty("MyDBConnection")
                   .GetString()
                   ?? "";
        }

        public void AddCaliber(Caliper cal)
        {
            byte[]? pictureBytes = null;

            if (cal.Picture != null)
            {
                using var ms = new MemoryStream();
                cal.Picture.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                pictureBytes = ms.ToArray();
            }

            using var con = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("dbo.CreateCaliber", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 80).Value = (object?)cal.Type ?? DBNull.Value;
            cmd.Parameters.Add("@Comment", SqlDbType.NVarChar, 255).Value = (object?)cal.Comment ?? DBNull.Value;
            cmd.Parameters.Add("@Picture", SqlDbType.VarBinary, -1).Value = (object?)pictureBytes ?? DBNull.Value;
            cmd.Parameters.Add("@ItemNumber", SqlDbType.Int).Value = cal.FrameID;
            cmd.Parameters.Add("@Brand", SqlDbType.NVarChar, 80).Value = (object?)cal.Manufacturer ?? DBNull.Value;
            cmd.Parameters.Add("@BatchID", SqlDbType.Int).Value = cal.BatchID; // REQUIRED

            con.Open();
            var result = cmd.ExecuteScalar();

            if (result != null && int.TryParse(result.ToString(), out int newId))
                cal.FrameID = newId;
        }
    }
}
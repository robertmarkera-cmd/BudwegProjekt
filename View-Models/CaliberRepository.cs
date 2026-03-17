using Budweg.Models;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Text.Json;
using System.Data;

namespace Budweg.View_Models
{
    public class CaliberRepository
    {
        private readonly string ConnectionString;

        private List<Caliper> _caliper = new List<Caliper>();

        public CaliberRepository()
        {

            var json = File.ReadAllText("jsconfig1.json");
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
        public void AddCaliber(Caliper caliperToBeCreated)
        {

            byte[]? pictureBytes = null;
            if (caliperToBeCreated.Picture != null)
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    caliperToBeCreated.Picture.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    pictureBytes = ms.ToArray();
                }
            }

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("dbo.CreateCaliber", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 80).Value = (object?)caliperToBeCreated.Type ?? DBNull.Value;
                    cmd.Parameters.Add("@Comment", SqlDbType.NVarChar, 255).Value = (object?)caliperToBeCreated.Comment ?? DBNull.Value;
                    cmd.Parameters.Add("@Picture", SqlDbType.VarBinary, -1).Value = (object?)pictureBytes ?? DBNull.Value;
                    cmd.Parameters.Add("@ItemNumber", SqlDbType.Int).Value = caliperToBeCreated.FrameID;
                    cmd.Parameters.Add("@Brand", SqlDbType.NVarChar, 80).Value = (object?)caliperToBeCreated.Manufacturer ?? DBNull.Value;
                    cmd.Parameters.Add("@BatchID", SqlDbType.Int).Value = DBNull.Value;

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

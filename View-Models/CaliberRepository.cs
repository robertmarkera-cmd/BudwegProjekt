using Budweg.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Budweg.View_Models
{
    public class CaliberRepository
    {
        private readonly string ConnectionString;

        public CaliberRepository()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("jsconfig1.json")
                .Build();

            ConnectionString = config.GetConnectionString("MyDBConnection") ?? "";
        }

        public void AddCaliber(Caliper cal)
        {
            using var con = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("dbo.CreateCaliber", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 80).Value =
                (object?)cal.Type ?? DBNull.Value;

            cmd.Parameters.Add("@Comment", SqlDbType.NVarChar, 255).Value =
                (object?)cal.Comment ?? DBNull.Value;

            cmd.Parameters.Add("@Picture", SqlDbType.VarBinary, -1).Value = DBNull.Value;

            cmd.Parameters.Add("@ItemNumber", SqlDbType.Int).Value = cal.FrameID;

            cmd.Parameters.Add("@Brand", SqlDbType.NVarChar, 80).Value =
                (object?)cal.Manufacturer ?? DBNull.Value;

            cmd.Parameters.Add("@BatchID", SqlDbType.Int).Value = cal.BatchID;

            con.Open();
            var result = cmd.ExecuteScalar();

            if (result != null && int.TryParse(result.ToString(), out int newId))
                cal.FrameID = newId;
        }
    }
}
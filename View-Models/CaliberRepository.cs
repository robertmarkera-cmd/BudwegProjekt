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

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("AddCaliper", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = caliperToBeCreated.Type;
                    cmd.Parameters.Add("@Manufacturer", SqlDbType.NVarChar).Value = caliperToBeCreated.Manufacturer;
                    cmd.Parameters.Add("@Comment", SqlDbType.NVarChar).Value = caliperToBeCreated.Comment;
                    cmd.Parameters.Add("@Picture", SqlDbType.NVarChar).Value = caliperToBeCreated.Picture;
                    cmd.Parameters.Add("@FrameID", SqlDbType.Int).Value = caliperToBeCreated.FrameID;

                    caliperToBeCreated.FrameID = Convert.ToInt32(cmd.ExecuteScalar());

                }
            }
        }
    }
}

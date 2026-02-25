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
        public void Create(Caliper caliperToBeCreated)
        {

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Caliper (Type, Manufacturer, Comment, Picture, FrameID)"
                    + "VALUES(@Type, @Manufacturer, @Comment, @Picture, @FrameID)" + "SELECT @@IDENTITY", con))
                {
                    cmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = caliperToBeCreated.Type;
                    cmd.Parameters.Add("@Manufacturer", SqlDbType.DateTime2).Value = caliperToBeCreated.Manufacturer;
                    cmd.Parameters.Add("@Comment", SqlDbType.DateTime2).Value = caliperToBeCreated.Comment;
                    cmd.Parameters.Add("@Picture", SqlDbType.NVarChar).Value = caliperToBeCreated.Picture;
                    cmd.Parameters.Add("@FrameID", SqlDbType.Int).Value = caliperToBeCreated.FrameID;
                    caliperToBeCreated.FrameID = Convert.ToInt32(cmd.ExecuteScalar());
                    _caliper.Add(caliperToBeCreated);

                }
            }
        }
    }
}
//public string Type { get; set; }
//public string Manufacturer { get; set; }
//public string Comment { get; set; }
//public Bitmap Picture { get; set; }
//public int FrameID { get; set; }
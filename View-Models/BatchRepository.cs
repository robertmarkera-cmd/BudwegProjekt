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
    public class BatchRepository
    {
        private readonly string ConnectionString;

        private List<Batch> _batch = new List<Batch>();

        public BatchRepository()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("jsconfig1.json")
            .Build();

            ConnectionString = config.GetConnectionString("MyDBConnection");
        }
        public void Create(Batch batchToBeCreated)
        {

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Caliper (Comment, BatchNumber, BatchAmount, Picture)"
                    + "VALUES(@Comment, @BatchNumber, @BatchAmount, @Picture)" + "SELECT @@IDENTITY", con))
                {
                    cmd.Parameters.Add("@Comment", SqlDbType.NVarChar).Value = batchToBeCreated.Comment;
                    // Ret typerne så de matcher dine DB-kolonner:
                    cmd.Parameters.Add("@BatchNumber", SqlDbType.Int).Value = batchToBeCreated.BatchNumber;   // fx hvis int
                    cmd.Parameters.Add("@BatchAmount", SqlDbType.Int).Value = batchToBeCreated.BatchAmount;   // fx hvis int
                    cmd.Parameters.Add("@Picture", SqlDbType.NVarChar).Value = batchToBeCreated.Picture;
                    batchToBeCreated.BatchID = (int)cmd.ExecuteScalar();   // <-- Id, ikke Comment
                    _batch.Add(batchToBeCreated);
                }

            }
            }
        }
    }


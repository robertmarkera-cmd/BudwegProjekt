using Budweg.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Budweg.View_Models
{
    public class WorkStationRepository
    {
        private readonly string ConnectionString;

        private List<WorkStation> _workStation = new List<WorkStation>();

        public WorkStationRepository()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("jsconfig1.json")
            .Build();

            ConnectionString = config.GetConnectionString("MyDBConnection");
        }
        public void Create(WorkStation workStationToBeCreated)
        {

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO WorkStation (OperationalNumber, Arival, Departure, ArrivalNumber, DepartureNumber,WasteNumber,WorkStationName)"
                    + "VALUES(@OperationalNumber, @Arival, @Departure, @ArrivalNumber, @DepartureNumber, @WasteNumber, @WorkStationName)" + "SELECT @@IDENTITY", con))
                {
                    cmd.Parameters.Add("@OperationalNumber", SqlDbType.Int).Value = workStationToBeCreated.OperationalNumber;
                    cmd.Parameters.Add("@Arival", SqlDbType.DateTime2).Value = workStationToBeCreated.Arival;
                    cmd.Parameters.Add("@Departure", SqlDbType.DateTime2).Value = workStationToBeCreated.Departure;
                    cmd.Parameters.Add("@ArrivalNumber", SqlDbType.Int).Value = workStationToBeCreated.ArrivalNumber;
                    cmd.Parameters.Add("@DepartureNumber", SqlDbType.Int).Value = workStationToBeCreated.DepartureNumber;
                    cmd.Parameters.Add("@WasteNumber", SqlDbType.Int).Value = workStationToBeCreated.WasteNumber;
                    cmd.Parameters.Add("@WorkStationName", SqlDbType.NVarChar).Value = workStationToBeCreated.WorkStationName;
                    workStationToBeCreated.WorkstationID = Convert.ToInt32(cmd.ExecuteScalar());
                    _workStation.Add(workStationToBeCreated);

                }
            }
        }
    }
}

    
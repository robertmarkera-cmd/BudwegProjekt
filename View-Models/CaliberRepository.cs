using Budweg.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Windows.Input;

namespace Budweg.View_Models
{
    public class CaliberRepository
    {
        private readonly string ConnectionString;
        public ICommand Save { get; }

        public CaliberRepository()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("jsconfig1.json")
                .Build();

            ConnectionString = config.GetConnectionString("MyDBConnection") ?? "";

            Save = new SaveCommand(ConnectionString);
        }

        public void AddCaliber(Caliper cal)
        {
            Save.Execute(cal);
        }
    }

    public class SaveCommand : ICommand
    {
        private readonly string _connectionString;

        public SaveCommand(string connectionString)
        {
            _connectionString = connectionString;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            if (parameter is not Caliper cal)
                return;

            using var con = new SqlConnection(_connectionString);
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
using Checkers.Game;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Checkers.DataAccess.Dapper
{
    public class StateRepository
    {
        private const string connectionString = @"Server=localhost\SQLEXPRESS01;Database=Checkers;Trusted_Connection=True;";

        public List<StateValue> GetStates()
        {
            const string sql = @"SELECT * FROM States";

            using (var connection = new SqlConnection(connectionString))
            {
                return connection.Query<StateValue>(sql).AsList() ;
            }
        }

        public void SaveState(string state, double value)
        {
            const string sql = "INSERT INTO States (State, Value) Values (@State, @Value)";

            using (var connection =  new SqlConnection(connectionString))
            {
                connection.Open();
                connection.Execute(sql, new
                {
                    State = state,
                    Value = value
                });
            }
        }
    }
}

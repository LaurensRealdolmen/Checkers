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
        private const string connectionString = @"Server=(localdb)\cherckers;Integrated Security=true;";

        public IEnumerable<string> GetStates()
        {
            var sql = @"SELECT Value FROM States";

            using (var connection = new SqlConnection(connectionString))
            {
                return connection.Query<string>(sql).AsList() ;
            }
        }

        public void SaveState( int id, string state)
        {
            var sql = "INSERT INTO States (Value) Values (@Value)";
            
            using (var connection =  new SqlConnection(connectionString))
            {
                connection.Open();
                connection.Execute(sql, state);
            }
        }
    }
}

// To deal with Database interaction only

using Microsoft.Data.Sqlite;
using System.Xml.Linq;

namespace EventStreamProcessor

{
    internal class Database
    {
        private static string dbpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tokens.db");
        private string source = $"Data Source={dbpath};Mode=ReadWrite;Pooling=False;";

        // Constructor: Create Tokens table if not exist
        public Database() 
        {
            string tableCommand = @"
                CREATE TABLE IF NOT EXISTS Tokens (
                    TokenId NVARCHAR(50) PRIMARY KEY,
                    Owner NVARCHAR(50) NOT NULL
                )";
            ExecQuery(tableCommand);
        }

        public void ExecQuery(string query)
        {
            var connection = NewConnection();
            connection.Open();

            var createTable = new SqliteCommand(query, connection);

            createTable.ExecuteNonQuery();

            connection.Close();
            SqliteConnection.ClearAllPools();
        }

        public void InsertBulk(List<Token> _data)
        {
            var connection = NewConnection();
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            {
                var command = connection.CreateCommand();
                command.CommandText =
                @"
                    REPLACE INTO Tokens (TokenId, Owner) 
                    VALUES ($id, $owner)
                ";

                // use parameters to prevent SQL injection
                var idParam = command.CreateParameter();
                idParam.ParameterName = "$id";
                command.Parameters.Add(idParam);

                var ownerParam = command.CreateParameter();
                ownerParam.ParameterName = "$owner";
                command.Parameters.Add(ownerParam);

                for (var i = 0; i < _data.Count; i++)
                {
                    idParam.Value = _data[i].Id;
                    ownerParam.Value = _data[i].Owner;
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }

            connection.Close();
            SqliteConnection.ClearAllPools();
        }

        public string GetTokenOwner(string _tokenId)
        {
            var connection = NewConnection();
            connection.Open();

            var entries = new List<Token>();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT Owner 
                FROM Tokens
                WHERE TokenId = $id
            ";

            // use parameters to prevent SQL injection
            var pk = command.CreateParameter();
            pk.ParameterName = "$id";
            command.Parameters.Add(pk);

            pk.Value = _tokenId;

            using (var reader = command.ExecuteReader())
            {
                //SqliteDataReader query = command.ExecuteReader();
                while (reader.Read())
                {
                    string owner = reader["Owner"].ToString()!;
                    if (owner != "")
                        entries.Add(new Token
                        {
                            Id = _tokenId,
                            Owner = owner
                        });
                }
            }

            connection.Close();
            SqliteConnection.ClearAllPools();

            return entries.Count > 0
                ? $"Token {_tokenId} is owned by {entries[0].Owner}"
                : $"Token {_tokenId} is not owned by any wallet";
            
        }

        public List<Token> GetOwnerTokens(string _owner)
        {
            var connection = NewConnection();
            connection.Open();

            var entries = new List<Token>();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT TokenId 
                FROM Tokens
                WHERE Owner = $owner
            ";

            // use parameters to prevent SQL injection
            var sk = command.CreateParameter();
            sk.ParameterName = "$owner";
            command.Parameters.Add(sk);

            sk.Value = _owner;

            using (var reader = command.ExecuteReader())
            {
                //SqliteDataReader query = command.ExecuteReader();
                while (reader.Read())
                {
                    string tokenId = reader["TokenId"].ToString()!;
                    entries.Add(new Token
                    {
                        Id = tokenId,
                        Owner = _owner
                    });
                }
            }

            connection.Close();
            SqliteConnection.ClearAllPools();

            return entries;
        }

        public SqliteConnection NewConnection()
        {
            using var connection = new SqliteConnection(source);
            return connection;
        }
    }
}
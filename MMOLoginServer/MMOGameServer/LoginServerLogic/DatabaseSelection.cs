using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using MMOLoginServer.ServerData;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace MMOLoginServer.LoginServerLogic
{
    public class DatabaseSelection
    {
        private string connectionString;
        public DatabaseSelection(string connString)
        {
            connectionString = connString;
        }
        public int DeleteCharacter(int accountId, string characterName)
        {
            int result = DeleteSqlData("DELETE FROM Character WHERE Name = @characterName AND accountID = @accountId", new SqlParameter("characterName", characterName), new SqlParameter("accountId", accountId));
            return result;
        }
        public int CreateCharacter(string characterName, ClientData account)
        {
            int result = InsertSqlData("INSERT INTO Character(AccountID,Name) VALUES(@accountId,@name)", new SqlParameter("accountId", account.id), new SqlParameter("name", characterName));
            return result;
        }
        public int UserAuthentication(string username, byte[] password)
        {
            List<ClientData> accounts = GetSqlClientData("SELECT Username, Password, Salt, Id FROM Account WHERE Username = @0", new SqlParameter("0", username));

            if (accounts.Count == 0)
                return -1;

            byte[] saltByte = accounts[0].salt;
            byte[] passwordSalted = new byte[password.Length + saltByte.Length];

            Buffer.BlockCopy(password, 0, passwordSalted, 0, password.Length);
            Buffer.BlockCopy(saltByte, 0, passwordSalted, password.Length, saltByte.Length);

            passwordSalted = new System.Security.Cryptography.SHA256Managed().ComputeHash(passwordSalted);

            Debug.Log("Got: {0}\nStoredPassword: {1}\nMessagedPassword: {2}", username, BitConverter.ToString(passwordSalted), BitConverter.ToString(accounts[0].password));

            Console.WriteLine(BitConverter.ToString(passwordSalted));
            Console.WriteLine(BitConverter.ToString(accounts[0].password));

            if (username.ToLower() == accounts[0].name.ToLower() && BitConverter.ToString(passwordSalted) == BitConverter.ToString(accounts[0].password))
            {
                return accounts[0].id;
            }
            return -1;
        }
        public List<CharacterData> GetCharactersData(int accountId)
        {
            List<CharacterData> characters = new List<CharacterData>();
            List<string[]> rows = GetSqlData("SELECT * FROM Character WHERE AccountID = @0", new SqlParameter("0", accountId)); ;
            foreach (var row in rows)
            {
                characters.Add(new CharacterData(row));
            }
            return characters;
        }
        public int DeleteSqlData(string selection, params SqlParameter[] sqlParam)
        {
            int result;
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connectionString;
                SqlCommand command = new SqlCommand(selection, conn);
                foreach (var param in sqlParam)
                {
                    command.Parameters.Add(param);
                }
                conn.Open();
                result = command.ExecuteNonQuery();

            }
            if (result < 0)
                Debug.Log("Error deleting from database");
            return result;
        }
        public int InsertSqlData(string selection, params SqlParameter[] sqlParam)
        {
            int result;
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connectionString;
                SqlCommand command = new SqlCommand(selection, conn);
                foreach (var param in sqlParam)
                {
                    command.Parameters.Add(param);
                }

                conn.Open();
                result = command.ExecuteNonQuery();

            }
            if (result < 0)
                Debug.Log("Error inserting into database");
            return result;
        }
        public List<string[]> GetSqlData(string selection, params SqlParameter[] sqlParam)
        {
            List<string[]> data = new List<string[]>();
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connectionString;
                SqlCommand command = new SqlCommand(selection, conn);
                foreach (var param in sqlParam)
                {
                    command.Parameters.Add(param);
                }

                conn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    int rowNum = 0;
                    while (reader.Read())
                    {
                        data.Add(new string[reader.FieldCount]);
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            data[rowNum][i] = reader[i].ToString();
                        }
                        rowNum++;
                    }
                }
            }
            return data;
        }
        public List<ClientData> GetSqlClientData(string selection, params SqlParameter[] sqlParam)
        {
            List<ClientData> data = new List<ClientData>();
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connectionString;
                SqlCommand command = new SqlCommand(selection, conn);
                foreach (var param in sqlParam)
                {
                    command.Parameters.Add(param);
                }

                conn.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ClientData acc = new ClientData();
                        acc.name = (string)reader[0];
                        acc.password = (byte[])reader[1];
                        acc.salt = (byte[])reader[2];
                        acc.id = (int)reader[3];
                        data.Add(acc);
                    }
                }
            }
            return data;
        }
    }
}

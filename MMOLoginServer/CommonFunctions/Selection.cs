using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Lidgren.Network.ServerFiles;
using MMOLoginServer.ServerData;

namespace CommonFunctions
{
    public class Selection
    {
        private string connectionString;
        public static Selection instance;
        public Selection(string connString)
        {
            connectionString = connString;
            instance = this;
        }
        public int DeleteCharacter(int accountId, string characterName)
        {
            int result = DeleteSqlData("DELETE FROM Character WHERE Name = @characterName AND accountID = @accountId", new SqlParameter("characterName", characterName), new SqlParameter("accountId", accountId));
            return result;
        }
        public int CreateCharacter(int accountID, string characterName, int characterType = 0)
        {
            int result = InsertSqlData("INSERT INTO Character(AccountID,Type,Name) VALUES(@accountId,@type,@name)", new SqlParameter("accountId", accountID), new SqlParameter("type", characterType), new SqlParameter("name", characterName));
            return result;
        }
        public int UserAuthentication(string username, byte[] password)
        {
            List<ConnectionData> accounts = GetSqlClientData("SELECT Username, Password, Salt, Id FROM Account WHERE Username = @0", new SqlParameter("0", username));

            if (accounts.Count == 0)
                return -1;

            byte[] saltByte = accounts[0].salt;
            byte[] passwordSalted = new byte[password.Length + saltByte.Length];

            Buffer.BlockCopy(password, 0, passwordSalted, 0, password.Length);
            Buffer.BlockCopy(saltByte, 0, passwordSalted, password.Length, saltByte.Length);

            passwordSalted = new System.Security.Cryptography.SHA256Managed().ComputeHash(passwordSalted);

            Console.WriteLine("Got: {0}\nStoredPassword:    {1}\nMessagedPassword: {2}", username, BitConverter.ToString(passwordSalted), BitConverter.ToString(accounts[0].password));


            if (username.ToLower() == accounts[0].name.ToLower() && Util.CompareByteArrays(passwordSalted, accounts[0].password))
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
        public CharacterData GetCharacterData(int accountId, int characterId, string characterName)
        {
            CharacterData character = null;
            List<string[]> rows = GetSqlData("SELECT * FROM Character WHERE AccountID = @0 AND Name = @characterName AND Id = @characterId", new SqlParameter("0", accountId), new SqlParameter("characterId", characterId), new SqlParameter("characterName", characterName));
            if (rows.Count == 1)
                foreach (var row in rows)
                {
                    character = new CharacterData(row);
                }
            if (rows.Count == 0)
            {
                throw new Exception("No such character");
            }
            if (rows.Count > 1)
            {
                throw new Exception("More then 1 character in the list! IMPOSSIBLE");
            }
            return character;
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
                Console.WriteLine("Error deleting from database");
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
                Console.WriteLine("Error inserting into database");
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
        public List<ConnectionData> GetSqlClientData(string selection, params SqlParameter[] sqlParam)
        {
            List<ConnectionData> data = new List<ConnectionData>();
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
                        ConnectionData acc = new ConnectionData();
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
        public int CountSqlData(string selection, params SqlParameter[] sqlParam)
        {
            int count = 0;
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connectionString;
                SqlCommand command = new SqlCommand(selection, conn);

                foreach (var param in sqlParam)
                {
                    command.Parameters.Add(param);
                }
                conn.Open();
                count = (int)command.ExecuteScalar();
            }
            return count;
        }
    }
}


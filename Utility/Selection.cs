using System;
using System.Collections.Generic;
using Utility.Models;
using System.Linq;

namespace Utility
{
    public class Selection
    {
        private string connectionString;
        public static Selection instance;
        //ServerContext database;
        int result;
        public Selection(string connString)
        {
            connectionString = connString;

            instance = this;
        }
        public int DeleteCharacter(int accountId, string characterName)
        {
            ServerContext database = new ServerContext();
            Character remove = database.Character.Where(x => x.AccountId == accountId).Where(x => x.Name == characterName).First();
            if (remove != null)
            {
                database.Character.Remove(remove);
            }
            result = database.SaveChanges();
            return result;
        }
        public int CreateCharacter(int accountID, string characterName, int characterType = 0)
        {
            using (ServerContext database = new ServerContext())
            {
                Character character = new Character();
                character.AccountId = accountID;
                character.Name = characterName;
                character.CharType = characterType;

                database.Add(character);
                result = database.SaveChanges();

            }
            return result;
        }
        public int CreateAccount(string username, string email, byte[] password, byte[] salt)
        {
            using (ServerContext database = new ServerContext())
            {
                Account account = new Account();
                account.Username = username;
                account.Email = email;
                account.Password = password;
                account.Salt = salt;
                database.Account.Add(account);

                database.SaveChanges();
            }
            return result;
        }
        public int UserAuthentication(string username, byte[] password)
        {
            using (ServerContext database = new ServerContext())
            {
                Account account = database.Account.Where(x => x.Username == username).First();
                if (account == null)
                    return -1;

                byte[] saltByte = account.Salt;
                byte[] passwordSalted = new byte[password.Length + saltByte.Length];

                Buffer.BlockCopy(password, 0, passwordSalted, 0, password.Length);
                Buffer.BlockCopy(saltByte, 0, passwordSalted, password.Length, saltByte.Length);
                passwordSalted = new System.Security.Cryptography.SHA256Managed().ComputeHash(passwordSalted);

                //Console.WriteLine("Got: {0}\nStoredPassword:    {1}\nMessagedPassword: {2}", username, BitConverter.ToString(passwordSalted), BitConverter.ToString(accounts[0].password));

                if (username.ToLower() == account.Username.ToLower() && Util.CompareByteArrays(passwordSalted, account.Password))
                {
                    return account.Id;
                }
            }
            return -1;
        }
        public List<Character> GetCharactersData(int accountId)
        {
            var characters = new List<Character>();
            using (ServerContext database = new ServerContext())
            {
                characters = (database.Character.Where(x => x.AccountId.Equals(accountId)).ToList());
            }
            return characters;
        }
        public Character GetCharacterData(int accountId, int characterId, string characterName)
        {
            Character character = null;
            using (ServerContext database = new ServerContext())
            {
                character = database.Character.Where(x => x.AccountId == accountId).Where(x => x.Id == characterId).Where(x => x.Name == characterName).First();
                if (character == null)
                {
                    throw new Exception("No such character");
                }
            }
            return character;
        }
    }
}
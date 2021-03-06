﻿using MySql.Data.MySqlClient;
using System;
using System.Text;
using System.Collections.Generic;
using Utility.Models;

namespace Utility
{

    public class Selection
    {
        public string connectionString;
        public static Selection instance;
        int result;
        public Selection(string host, string database, int port, string user, string password)
        {

            connectionString = "Server=192.168.0.24" + ";Database=" + database + ";Port=" + port + ";User=" + user + ";Password=" + password + ";CharSet=utf8;";

            instance = this;
        }
        public Selection()
        {
        }
        public int CreateCharacter(int accountID, string characterName, int characterType = 0)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM mmo.character WHERE name=@characterName", conn))
                {
                    cmd.Parameters.AddWithValue("characterName", characterName);
                    result = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            if (result > 0)
                return -1;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("INSERT INTO mmo.character (id_account, name, type) VALUES (@id_account, @name, @type)", conn))
                {
                    cmd.Parameters.AddWithValue("id_account", accountID);
                    cmd.Parameters.AddWithValue("name", characterName);
                    cmd.Parameters.AddWithValue("type", characterType);

                    result = cmd.ExecuteNonQuery();
                }
            }
            return result;
        }

        public int DeleteCharacter(int accountID, string characterName)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("DELETE FROM mmo.character WHERE id_account=@id_account AND name=@name", conn))
                {
                    cmd.Parameters.AddWithValue("id_account", accountID);
                    cmd.Parameters.AddWithValue("name", characterName);

                    result = cmd.ExecuteNonQuery();
                }
            }
            return result;
        }
        public int CreateAccount(string username, string email, string password, string salt)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM `mmo`.`account` WHERE username=@username OR email=@email", conn))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.Parameters.AddWithValue("email", email);

                    result = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            if (result > 0)
                return -1;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("INSERT INTO `mmo`.`account` ( `username`, `email`, `password`, `salt`) VALUES (@username, @email, @password, @salt);", conn))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.Parameters.AddWithValue("email", email);
                    cmd.Parameters.AddWithValue("password", password);
                    cmd.Parameters.AddWithValue("salt", salt);

                    result = cmd.ExecuteNonQuery();
                }
            }
            return result;
        }
        public int UserAuthentication(string username, string hexPassword)
        {
            
            Account account = SQLGetAccount(username);
            if (account == null)
            {
                Console.WriteLine("No account with username: " + username);
                return -1;
            }

            string hexSalt = account.Salt;
            string hexPasswordSalted = hexPassword + hexSalt;
            byte[] bytePasswordSalted = Encoding.UTF8.GetBytes(hexPasswordSalted);


            bytePasswordSalted = new System.Security.Cryptography.SHA256Managed().ComputeHash(bytePasswordSalted);
            string hexHashedPasswordSalted = BitConverter.ToString(bytePasswordSalted);
            Console.WriteLine("UsernameDB:\t!{0}!", account.Username);
            Console.WriteLine("UsernameUs:\t!{0}!", username);
            Console.WriteLine("PasswordDB:\t!{0}!\nPasswordUs:\t!{1}!", account.Password, hexHashedPasswordSalted);
            if (account != null)
                if (username.ToLower().Equals(account.Username.ToLower()) && hexHashedPasswordSalted.Equals(account.Password))
                {
                    Console.WriteLine("Password OK");
                    return account.Id;
                }
            Console.WriteLine("Password NOT OK");
            return -1;
        }

        private Account SQLGetAccount(string username)
        {
            Account account = null;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                Console.WriteLine(username);
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM mmo.account WHERE username=@username", conn))
                    {
                        cmd.Parameters.AddWithValue("username", username);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                account = new Account();
                                account.Id = reader.GetInt32("id_account");
                                account.Username = reader.GetString("username");
                                account.Email = reader.GetString("email");
                                account.Password = reader.GetString("password");
                                account.Salt = reader.GetString("salt");
                            }
                        }
                    }
                }
            }
            return account;

        }
        public List<Character> GetCharactersData(int accountID)
        {
            var characters = new List<Character>();
            characters = SQLGetCharacters(accountID);
            return characters;
        }

        private List<Character> SQLGetCharacters(int accountID)
        {
            List<Character> characters = new List<Character>();
            Character character = null;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM mmo.character WHERE id_account=@accountID", conn))
                    {
                        cmd.Parameters.AddWithValue("accountID", accountID);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                character = LoadCharacter(reader);
                                characters.Add(character);
                            }
                        }
                    }
                }
            }

            return characters;
        }

        public Character GetCharacterData(int accountID, int characterID, string characterName)
        {
            Character character = null;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM mmo.character WHERE id_account=@accountID AND id_character=@characterID AND name=@characterName", conn))
                    {
                        cmd.Parameters.AddWithValue("accountID", accountID);
                        cmd.Parameters.AddWithValue("characterID", characterID);
                        cmd.Parameters.AddWithValue("characterName", characterName);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            character = LoadCharacter(reader);
                        }
                    }
                }
            }
            return character;
        }
        private Character LoadCharacter(MySqlDataReader reader)
        {
            Character character = new Character();
            character.CharacterID = reader.GetInt32("id_character");
            character.AccountID = reader.GetInt32("id_account");
            character.AccountID = reader.GetInt32("id_account");
            character.Name = reader.GetString("name");
            character.CharType = reader.GetInt32("type");
            character.Level = reader.GetInt32("level");
            character.Exp = reader.GetInt32("exp");
            character.Gold = reader.GetInt32("gold");
            character.Health = reader.GetInt32("current_health");
            character.MaxHealth = reader.GetInt32("max_health");
            character.Mana = reader.GetInt32("current_mana");
            character.MaxMana = reader.GetInt32("max_mana");
            character.PosX = reader.GetFloat("x_position");
            character.PosY = reader.GetFloat("y_position");
            character.PosZ = reader.GetFloat("z_position");

            character.Skills = GetCharacterSkills(character.CharacterID);
            character.Inventory = GetCharacterItems(character.CharacterID);

            return character;
        }
        public List<Skill> GetSkillsData()
        {
            List<Skill> skills = new List<Skill>();
            Skill skill;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM mmo.skill", conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            skill = new Skill();
                            skill.ID = reader.GetInt32("id");
                            skill.Name = reader.GetString("name");
                            skill.SkillType = reader.GetInt32("type");
                            skill.ManaCost = reader.GetInt32("use_cost");
                            skill.ManaCostMultiplier = reader.GetFloat("use_cost_multiplier");
                            skill.GoldCost = reader.GetInt32("leveling_cost");
                            skill.GoldCostMultiplier = reader.GetFloat("leveling_cost_multiplier");
                            skill.Range = reader.GetFloat("range");
                            skill.RangeMultiplier = reader.GetFloat("range_multiplier");

                            skill.RequiredSkillID = reader.GetInt32("rq_id_skill");

                            int id;
                            int value;
                            int minLevel;
                            float multiplier;
                            for (int i = 0; i < 7; i++)
                            {
                                id = reader.GetInt32("stat_" + i);
                                if (id != 0)
                                {
                                    //name = reader.GetString("stat_name");
                                    value = reader.GetInt32("stat_" + i + "_value");
                                    minLevel = reader.GetInt32("stat_" + i + "_min_level");
                                    multiplier = reader.GetFloat("stat_" + i + "_multiplier");

                                    skill.Effects.Add(id, new Effect(id, value, minLevel, multiplier));
                                }
                            }
                            skills.Add(skill);
                        }
                    }
                }
            }
            return skills;
        }
        public Dictionary<int, CharacterItem> GetCharacterItems(int characterID)
        {
            Dictionary<int, CharacterItem> Inventory = new Dictionary<int, CharacterItem>();
            CharacterItem item = null;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM mmo.character_slot WHERE character_id = @characterID", conn))
                {
                    cmd.Parameters.AddWithValue("characterID", characterID);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            item = new CharacterItem();
                            item.ID = reader.GetInt32("id");
                            item.ItemID = reader.GetInt32("item_id");
                            item.SlotID = reader.GetInt32("slot_id");
                            item.SlotType = (ESlotType)reader.GetInt32("slot_type");
                            item.Amount = reader.GetInt32("amount");
                            item.Level = reader.GetInt32("level");
                            item.Durability = reader.GetInt32("durability");
                            item.MaxDurability = reader.GetInt32("max_durability");

                            Inventory.Add(item.ID, item);
                        }
                    }
                }
            }
            return Inventory;
        }
        public List<InventoryItem> GetItems()
        {
            List<InventoryItem> items = new List<InventoryItem>();
            InventoryItem item;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM mmo.item", conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            item = new InventoryItem();
                            item.Name = reader.GetString("name");
                            item.ID = reader.GetInt32("id");
                            item.RequiredLevel = reader.GetInt32("rq_level");
                            item.ItemType = (EItemType)reader.GetInt32("type");

                            int id;
                            int value;
                            int minLevel;
                            float multiplier;
                            for (int i = 0; i < 7; i++)
                            {
                                id = reader.GetInt32("stat_" + i);
                                if (id != 0)
                                {
                                    value = reader.GetInt32("stat_" + i + "_value");
                                    minLevel = reader.GetInt32("stat_" + i + "_min_level");
                                    multiplier = reader.GetFloat("stat_" + i + "_multiplier");

                                    item.Effects.Add(id, new Effect(id, value, minLevel, multiplier));
                                }
                            }
                            items.Add(item);
                        }
                    }
                }
            }
            return items;
        }
        private Dictionary<int, int> GetCharacterSkills(int characterID)
        {
            Dictionary<int, int> skills = new Dictionary<int, int>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM mmo.character_skill WHERE id_character = @characterID", conn))
                {
                    cmd.Parameters.AddWithValue("characterID", characterID);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt16("id");
                            int level = reader.GetInt16("level");
                            skills.Add(id, level);
                        }
                    }
                }
            }
            return skills;
        }
    }
}
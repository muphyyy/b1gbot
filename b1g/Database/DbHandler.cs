using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace b1g.Database
{
    public class DbHandler
    {
        public static string GetConnectionString()
        {
            string host = "localhost";
            string uid = "root";
            string password = "";
            string db = "b1gbot";
            string ssl = "none";

            return "SERVER=" + host + "; DATABASE=" + db + "; UID=" + uid + "; PASSWORD=" + password + "; SSLMODE=" + ssl + ";";
        }

        public async static Task InsertDbBan(string user, string banned_by, string reason, string date)
        {
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO `bans` (`user`, `banned_by`, `reason`, `date`) VALUES (@user, @banned_by, @reason, @date)";
                command.Parameters.AddWithValue("@user", user);
                command.Parameters.AddWithValue("@banned_by", banned_by);
                command.Parameters.AddWithValue("@reason", reason);
                command.Parameters.AddWithValue("@date", date);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public async static Task InsertDbKick(string user, string kicked_by, string reason, string date)
        {
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO `kicks` (`user`, `kicked_by`, `reason`, `date`) VALUES (@user, @kicked_by, @reason, @date)";
                command.Parameters.AddWithValue("@user", user);
                command.Parameters.AddWithValue("@kicked_by", kicked_by);
                command.Parameters.AddWithValue("@reason", reason);
                command.Parameters.AddWithValue("@date", date);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public async static Task InsertDbMuted(string user, string muted_by, string reason, string date)
        {
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO `muteds` (`user`, `muted_by`, `reason`, `date`) VALUES (@user, @muted_by, @reason, @date)";
                command.Parameters.AddWithValue("@user", user);
                command.Parameters.AddWithValue("@muted_by", muted_by);
                command.Parameters.AddWithValue("@reason", reason);
                command.Parameters.AddWithValue("@date", date);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public async static Task InsertDbStrike(string user, string striked_by, string reason, string date)
        {
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO `strikes` (`user`, `striked_by`, `reason`, `date`) VALUES (@user, @striked_by, @reason, @date)";
                command.Parameters.AddWithValue("@user", user);
                command.Parameters.AddWithValue("@striked_by", striked_by);
                command.Parameters.AddWithValue("@reason", reason);
                command.Parameters.AddWithValue("@date", date);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public static async Task<bool> CheckIfUserIsMuted(string userId)
        {
            bool status = false;
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM muteds WHERE user = @user LIMIT 1";
                command.Parameters.AddWithValue("@user", userId);

                DbDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                status = reader.HasRows;
            }
            return status;
        }

        public async static Task DeleteDbMuted(string userId)
        {
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM muteds WHERE user = @user";
                command.Parameters.AddWithValue("@user", userId);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public static async Task<int> CheckUserStrikes(string userId)
        {
            int numberOfStrikes = 0;
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM strikes WHERE user = @user";
                command.Parameters.AddWithValue("@user", userId);

                DbDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                DataTable dt = new DataTable();
                dt.Load(reader);
                numberOfStrikes = dt.Rows.Count;
            }
            return numberOfStrikes;
        }

        public async static Task DeleteUserStrike(string userId)
        {
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM strikes WHERE user = @user ORDER BY id ASC LIMIT 1";
                command.Parameters.AddWithValue("@user", userId);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public async static Task InsertDbUserRole(string userId, int roleId)
        {
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO `roles` (`userid`, `role`) VALUES (@userid, @role)";
                command.Parameters.AddWithValue("@userid", userId);
                command.Parameters.AddWithValue("@role", roleId);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public async static Task DeleteDbUserRole(string userId, int roleId)
        {
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM roles WHERE userid = @user AND role = @role";
                command.Parameters.AddWithValue("@user", userId);
                command.Parameters.AddWithValue("@role", roleId);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public static async Task<bool> CheckIfUserHasRole(string userId, int role)
        {
            bool status = false;
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM roles WHERE userid = @user AND role = @role LIMIT 1";
                command.Parameters.AddWithValue("@user", userId);
                command.Parameters.AddWithValue("@role", role);

                DbDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                status = reader.HasRows;
            }
            return status;
        }

        public async static Task InsertDbUserJoin(string userId)
        {
            string date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO `joins` (`userid`, `date`) VALUES (@userid, @date)";
                command.Parameters.AddWithValue("@userid", userId);
                command.Parameters.AddWithValue("@date", date);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public static async Task<bool> CheckIfUserIsJoined(string userId)
        {
            bool status = false;
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM joins WHERE userid = @user";
                command.Parameters.AddWithValue("@user", userId);

                DbDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                status = reader.HasRows;
            }
            return status;
        }

        public static async Task<string> GetUserJoinedDate(string userId)
        {
            string date = "0";
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM joins WHERE userid = @user";
                command.Parameters.AddWithValue("@user", userId);

                DbDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

                if (reader.HasRows)
                {
                    await reader.ReadAsync().ConfigureAwait(false);
                    {
                        date = reader.GetString(reader.GetOrdinal("date"));
                    }
                }
                return date;
            }
        }

        public static async Task<string> GetUserLevel(string userId)
        {
            string date = "0";
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM levels WHERE userid = @user";
                command.Parameters.AddWithValue("@user", userId);

                DbDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

                if (reader.HasRows)
                {
                    await reader.ReadAsync().ConfigureAwait(false);
                    {
                        date = reader.GetString(reader.GetOrdinal("lastmessage"));
                    }
                }
                return date;
            }
        }

        public async static Task InsertDbUserLevel(string userId)
        {
            string date = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO `levels` (`userid`, `lastmessage`) VALUES (@userid, @date)";
                command.Parameters.AddWithValue("@userid", userId);
                command.Parameters.AddWithValue("@date", date);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public async static Task UpdateDbUserLevel(string userId, int messages)
        {
            string lastmessage = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE levels SET messages = @messages, lastmessage = @lastmessage WHERE userid = @userid";
                command.Parameters.AddWithValue("@userid", userId);
                command.Parameters.AddWithValue("@messages", messages + 1);
                command.Parameters.AddWithValue("@lastmessage", lastmessage);

                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public static async Task<int> GetUserMessages(string userId)
        {
            int date = 0;
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM levels WHERE userid = @user";
                command.Parameters.AddWithValue("@user", userId);

                DbDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

                if (reader.HasRows)
                {
                    await reader.ReadAsync().ConfigureAwait(false);
                    {
                        date = reader.GetInt32(reader.GetOrdinal("messages"));
                    }
                }
                return date;
            }
        }
    }
}

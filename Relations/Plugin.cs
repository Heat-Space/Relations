using TerrariaApi.Server;
using Terraria;
using TShockAPI;
using Microsoft.Xna.Framework;
using NuGet.Protocol.Plugins;
using System.IO.Streams;
using IL.Terraria.DataStructures;
using MySql.Data.MySqlClient;
using System.Data;
using Microsoft.Data.Sqlite;
using TShockAPI.DB;

namespace Relations
{
    [ApiVersion(2, 1)]
    public class RelationsPlugin : TerrariaPlugin
    {
        #region Data
        public override string Author => "HeatSpace";
        public override string Name => "Relations";
        public RelationsPlugin(Main game) : base(game) { }

        public override Version Version => new Version(1, 3, 0);

        #endregion

        #region Variables
        public static string?[] requests = new string?[Main.maxPlayers];

        public static string?[] daterequests = new string?[Main.maxPlayers];
        public static string?[] datewarps = new string?[Main.maxPlayers];

        public static IDbConnection DB;

        #endregion

        #region Initialize
        public override void Initialize()
        {

            Commands.ChatCommands.AddRange(PluginCommands.commands);

            ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
            InitailizeBD();
        }
        #endregion

        #region TShock
        #region Hooks
        void OnLeave(LeaveEventArgs args)
        {
            requests[args.Who] = null;
            daterequests[args.Who] = null;
            datewarps[args.Who] = null;
        }
        #endregion
        #endregion

        #region BD
        internal static void InitailizeBD()
        {
            IQueryBuilder builder = null;
            switch (TShock.Config.Settings.StorageType)
            {
                default:
                    return;

                case "mysql":
                    var hostport = TShock.Config.Settings.MySqlHost.Split(':');
                    DB = new MySqlConnection();
                    DB.ConnectionString = string.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4};",
                        hostport[0],
                        hostport.Length > 1 ? hostport[1] : "3306",
                        TShock.Config.Settings.MySqlDbName,
                        TShock.Config.Settings.MySqlUsername,
                        TShock.Config.Settings.MySqlPassword);
                    builder = new MysqlQueryCreator();
                    break;
                case "sqlite":
                    DB = new SqliteConnection(string.Format("Data Source=@0", Path.Combine(TShock.SavePath, "MyData.sqlite")));
                    builder = new SqliteQueryCreator();
                    break;
            }

            SqlTable table = new SqlTable("Marriages",
                new SqlColumn("Nickname", MySqlDbType.Text),
                new SqlColumn("Nickname2", MySqlDbType.Text));

            new SqlTableCreator(DB, builder).EnsureTableStructure(table);
        }

        public static string GetMarried(string playerName)
        {
            string nickname = string.Empty;

            using (var reader = DB.QueryReader("SELECT * FROM Marriages WHERE Nickname = @0", playerName))
            {
                if (reader.Read())
                {
                    nickname = reader.Get<string>("Nickname2");
                }
                else
                {
                    nickname = string.Empty;
                }
            }
            return nickname;
        }

        public static string GetMarried2(string playerName2)
        {
            string nickname = string.Empty;

            using (var reader = DB.QueryReader("SELECT * FROM Marriages WHERE Nickname2 = @0", playerName2))
            {
                if (reader.Read())
                {
                    nickname = reader.Get<string>("Nickname");
                }
                else
                {
                    nickname = string.Empty;
                }
            }
            return nickname;
        }

        public static bool Exists(string nickname)
        {
            bool exists = false;

            using (var reader = DB.QueryReader("SELECT * FROM Marriages WHERE Nickname = @0", nickname))
            {
                if (reader.Read())
                {
                    exists = true;
                }
                else
                {
                    exists = false;
                }
            }
            return exists;
        }

        public static bool Exists2(string nickname)
        {
            bool exists = false;

            using (var reader = DB.QueryReader("SELECT * FROM Marriages WHERE Nickname2 = @0", nickname))
            {
                if (reader.Read())
                {
                    exists = true;
                }
                else
                {
                    exists = false;
                }
            }
            return exists;
        }

        public static void CreateMarriage(string playerName, string playerName2)
        {
            DB.Query("INSERT INTO Marriages VALUES (@0, @1); ", playerName, playerName2);
        }

        public static void SaveMarriage(string playerName, string playerName2)
        {
            DB.Query("UPDATE Marriages SET Nickname2 = @1 WHERE Nickname = @0", playerName, playerName2);
        }

        public static string GetAllMarriages()
        {
            string nickname2 = string.Empty;
            string nickname = string.Empty;

            using (var reader = DB.QueryReader("SELECT * FROM Marriages"))
            {
                if (reader.Read())
                {
                    nickname2 = reader.Get<string>("Nickname2");
                    nickname = reader.Get<string>("Nickname");
                    if(nickname2 == string.Empty)
                    {
                        nickname2 = "NONE";
                    }
                }
                else
                {
                    nickname2 = string.Empty;
                    nickname = string.Empty;
                }
            }
            return nickname + " || " + nickname2 + "\n";
        }

        public static void DeleteMarriage(string nickname)
        {
            DB.Query("DELETE FROM Marriages WHERE Nickname=@0 OR Nickname2=@0", nickname);
        }
        #endregion
    }
}

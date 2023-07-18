using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using Terraria;
using TShockAPI.DB;
using static Relations.RelationsPlugin;
using Microsoft.Xna.Framework;

namespace Relations
{
    public class PluginCommands
    {
        #region Commands List
        public static Command[] commands = new Command[]
        {
            // Would be glad to get some translations from u guys :D

            new Command(PluginPermissions.kiss, KissCommand, "love")
            {
                HelpText = "Поцелуй кое-кого особенного ~ \nСинтаксис: /love <игрок>"
            },
            new Command(PluginPermissions.plunk, SlapCommand, "plunk")
            {
                HelpText = "Шлёпни кое-кого особенного ~ \nСинтаксис: /plunk <игрок>"
            },
            new Command(PluginPermissions.marry, MarryCommand, "marry")
            {
                HelpText = "Свадьба! \n/marry <игрок> - запрос на свадьбу с игроком. \nПодкоманды:\n/marry -confirm - принятие заявки на свадьбу. \n/marry -reject - отклонение заявки на свадьбу.",
                AllowServer = false
            },
            new Command(PluginPermissions.divorce, DivorceCommand, "divorce")
            {
                HelpText = "Разведись, если твой супруг тебе больше не нравится...  \nСинтаксис: /divorce",
                AllowServer = false
            },
            new Command(PluginPermissions.pat, PatCommand, "pat")
            {
                HelpText = "Гладь кого хочешь ~ \nСинтаксис: /pat <игрок>"
            },
            new Command(PluginPermissions.beat, BeatCommand, "beat")
            {
                HelpText = "Домашнее насилие - норма! \nСинтаксис: /beat <игрок>"
            },
            new Command(PluginPermissions.date, DateCommand, "date")
            {
                HelpText = "Пригласите любого игрока на свидание :D \n/date <игрок> <варп> - запрос на свидание с игроком в определённом месте\nПодкоманды:\n/date -confirm - принятие заявки на свидание\n/date -reject - отклонение заявки на свидание. ",
                AllowServer = false
            },
            new Command(PluginPermissions.sex, NSFWCommand_1, "sex", "fuck")
            {
                HelpText = "Мне серьёзно придётся объяснять это?... \nСинтаксис: /sex <игрок>",
                AllowServer = false
            },
            new Command(PluginPermissions.marriage, MarriageCommand, "marriage", "managemarriages")
            {
                HelpText = "Женись на зуме без его согласия.\nПодкоманды: \n/marriage -del <имя> - удаление свадьбы в базе данных\n/marriage -ins <имя> <имя2> - создание свадьбы двух игроков\n/marriage -list - вывод всех свадьб в базе данных"
            },
            new Command(PluginPermissions.rallow, RelationsAllowCommand, "rallow", "relationsallow")
            {
                HelpText = "Разрешает или запрещает использовать команды плагина на отношения на вас.",
                AllowServer = false
            },
            new Command(PluginPermissions.hug, HugCommand, "hug")
            {
                HelpText = "Обнимаффффки! \nСинтаксис: /hug <игрок>"
            }
        };
        #endregion

        public static void HugCommand(CommandArgs args)
        {
            var parameter = args.Parameters;

            if (parameter.Count > 0)
            {
                var plrs = TSPlayer.FindByNameOrID(args.Parameters[0]);

                if (plrs.Count > 1)
                {
                    args.Player.SendMultipleMatchError(plrs.Select(p => p.Name));
                }
                else if (plrs.Count == 0)
                {
                    args.Player.SendErrorMessage("Такого игрока нет на сервере.");
                }
                else
                {
                    var plr = plrs[0];
                    if (useable[plr.Index])
                    {
                        args.Player.SendSuccessMessage("Вы обняли " + plr.Name + ".");
                        TSPlayer.All.SendInfoMessage(args.Player.Name + " обнял игрока " + plr.Name);

                        plr.SetBuff(2);
                        plr.SetBuff(124);
                        args.Player.SetBuff(124);
                    }
                    else
                    {
                        
                            args.Player.SendErrorMessage("Данный игрок защищён от команд.");
                        
                    }
                }
            }
        }

        public static void RelationsAllowCommand(CommandArgs args)
        {
            useable[args.Player.Index] = !useable[args.Player.Index];
            if (useable[args.Player.Index])
            {
                TSPlayer.All.SendInfoMessage(args.Player.Name + " снова беззащитен!");
            }
            else
            {
                TSPlayer.All.SendInfoMessage(args.Player.Name + " защищён от злых команд!");
            }
        }

        public static void MarriageCommand(CommandArgs args)
        {
            var par = args.Parameters;

            if(par.Count > 0)
            {
                switch(par[0]) 
                {
                    case "-del":

                        if(par.Count > 1)
                        {
                            if (Exists(par[1]))
                            {
                                DeleteMarriage(par[1]);
                                args.Player.SendSuccessMessage("Вы удалили свадьбы игрока " + par[1]);
                            }
                        }
                        else
                        {
                            args.Player.SendErrorMessage("Неверный синтаксис.");
                        }
                        break;

                    case "-ins":
                        if(par.Count > 2)
                        {
                            if (!Exists(par[2]) && !Exists(par[1]))
                            {
                                CreateMarriage(par[1], par[2]);
                                CreateMarriage(par[2], par[1]);
                                args.Player.SendSuccessMessage($"Вы создали свадьбы между игроками ({par[1]}, {par[2]}");
                            }
                            else
                            {
                                args.Player.SendErrorMessage("Один из игроков уже был женат.");
                            }
                        }
                        else
                        {
                            args.Player.SendErrorMessage("Неверный синтаксис.");
                        }
                        break;

                    case "-list":
                        args.Player.SendInfoMessage(GetAllMarriages());
                        break;
                }
            }
            else
            {
                args.Player.SendErrorMessage("Неверный синтаксис.");
            }
        }

        public static void KissCommand(CommandArgs args)
        {
            var parameter = args.Parameters;

            if (parameter.Count > 0)
            {
                var plrs = TSPlayer.FindByNameOrID(args.Parameters[0]);

                if (plrs.Count > 1)
                {
                    args.Player.SendMultipleMatchError(plrs.Select(p => p.Name));
                }
                else if (plrs.Count == 0)
                {
                    args.Player.SendErrorMessage("Такого игрока нет на сервере.");
                }
                else
                {
                    var plr = plrs[0];
                    if (useable[plr.Index])
                    {



                        args.Player.SendSuccessMessage("Вы поцеловали " + plr.Name + ".");

                        TSPlayer.All.SendInfoMessage(args.Player.Name + " поцеловал игрока " + plr.Name);
                        if (Main.LocalPlayer.Male)
                        {
                            plr.SendInfoMessage("Кажется, " + args.Player.Name + " в вас влюблён!");
                        }
                        else
                        {
                            plr.SendInfoMessage("Кажется, " + args.Player.Name + " в вас влюблена!");
                        }
                        plr.SetBuff(119, 500);
                    }
                    else
                    {
                        args.Player.SendErrorMessage("Данный игрок защищён от команд.");
                    }
                }
            }
            else
            {
                args.Player.SendErrorMessage("Неверный синтаксис. Синтаксис: /love <игрок>");
            }
        }

        public static void DateCommand(CommandArgs args)
        {
            var parameter = args.Parameters;

            if (parameter.Count > 0)
            {
                switch (parameter[0])
                {
                    case "-cancel":

                        var plrR3 = RelationsPlugin.daterequests[args.Player.Index];

                        var plrs4 = TSPlayer.FindByNameOrID(plrR3.Replace(" ⌇", ""));

                        if (plrs4.Count > 0)
                        {
                            foreach (TSPlayer plr in plrs4)
                            {
                                if (RelationsPlugin.daterequests[plr.Index] == args.Player.Name && RelationsPlugin.daterequests[args.Player.Index] == plr.Name + " ⌇")
                                {
                                    RelationsPlugin.daterequests[args.Player.Index] = null;
                                    RelationsPlugin.daterequests[plr.Index] = null;

                                    TSPlayer.All.SendInfoMessage("Заявка на свидание игрока " + args.Player.Name + " была отменена.");
                                    args.Player.SendSuccessMessage("Вы отменили свидание.");

                                }
                            }
                        }
                        break;

                    case "-confirm":


                        var plrR = RelationsPlugin.daterequests[args.Player.Index];

                        var plrs = TSPlayer.FindByNameOrID(plrR);

                        if (plrs.Count > 0)
                        {
                            foreach (TSPlayer plr in plrs)
                            {
                                if (datewarps[args.Player.Index] != null && datewarps[plr.Index] != null && RelationsPlugin.daterequests[args.Player.Index] == plr.Name && RelationsPlugin.daterequests[plr.Index] == args.Player.Name + " ⌇")
                                {

                                    if (Main.LocalPlayer.Male)
                                    {
                                        plr.SendInfoMessage(args.Player.Name + " пришел на ваше свидание!");
                                        TSPlayer.All.SendInfoMessage(args.Player.Name + " принял заявку " + plr.Name + " на свидание!");
                                    }
                                    else
                                    {
                                        plr.SendInfoMessage(args.Player.Name + " пришла на ваше свидание!");
                                        TSPlayer.All.SendInfoMessage(args.Player.Name + " приняла заявку " + plr.Name + " на свидание!");
                                    }

                                    Warp warpB = TShock.Warps.Find(datewarps[args.Player.Index]);
                                    args.Player.Teleport(warpB.Position.X, warpB.Position.Y);
                                    plr.Teleport(warpB.Position.X, warpB.Position.Y);

                                    RelationsPlugin.daterequests[plr.Index] = null;
                                    RelationsPlugin.daterequests[args.Player.Index] = null;
                                    datewarps[plr.Index] = null;
                                    datewarps[args.Player.Index] = null;

                                }
                            }


                        }
                        else
                        {
                            args.Player.SendInfoMessage("У вас нет запросов.");
                        }

                        break;

                    case "-reject":

                        var plrR2 = RelationsPlugin.daterequests[args.Player.Index];

                        var plrs2 = TSPlayer.FindByNameOrID(plrR2);

                        if (plrs2.Count > 0)
                        {
                            foreach (TSPlayer plr in plrs2)
                            {

                                if (RelationsPlugin.daterequests[args.Player.Index] == plr.Name && RelationsPlugin.daterequests[plr.Index] == args.Player.Name + " ⌇")
                                {
                                    if (Main.LocalPlayer.Male)
                                    {
                                        TSPlayer.All.SendInfoMessage(args.Player.Name + " отклонил заявку " + plr.Name + " на свидание :(");
                                    }
                                    else
                                    {
                                        TSPlayer.All.SendInfoMessage(args.Player.Name + " отклонила заявку " + plr.Name + " на свидание :(");
                                    }
                                    RelationsPlugin.daterequests[plr.Index] = null;
                                    RelationsPlugin.daterequests[args.Player.Index] = null;
                                    datewarps[plr.Index] = null;
                                    datewarps[args.Player.Index] = null;
                                }
                            }
                        }
                        else
                        {
                            args.Player.SendInfoMessage("У вас нет запросов.");
                        }

                        break;


                    default:
                        if (parameter.Count > 1)
                        {


                            var plrs3 = TSPlayer.FindByNameOrID(parameter[0]);

                            if (plrs3.Count > 1)
                            {
                                args.Player.SendMultipleMatchError(plrs3.Select(p => p.Name));
                            }
                            else if (plrs3.Count == 0)
                            {
                                args.Player.SendErrorMessage("Такого игрока нет на сервере.");
                            }
                            else
                            {
                                var plr2 = plrs3[0];

                                if (plr2.HasPermission("relations.date") && useable[plr2.Index])
                                {
                                    if (plr2.Name != args.Player.Name)
                                    {

                                        try
                                        {
                                            Warp warp = TShock.Warps.Find(parameter[1]);
                                            datewarps[args.Player.Index] = warp.Name;
                                            datewarps[plr2.Index] = warp.Name;
                                        }
                                        catch
                                        {
                                            args.Player.SendErrorMessage("Варп не найден. \nДля полного листа варпов пропишите /warp list");
                                            return;
                                        }

                                        RelationsPlugin.daterequests[plr2.Index] = args.Player.Name;
                                        RelationsPlugin.daterequests[args.Player.Index] = plr2.Name + " ⌇";


                                        args.Player.SendInfoMessage(plr2.Name + " был приглашён на свидание.\nЧтобы отменить запрос пропишите /date -cancel");

                                        if (Main.LocalPlayer.Male)
                                        {
                                            TSPlayer.All.SendInfoMessage(args.Player.Name + " пригласил " + plr2.Name + " на свидание!");

                                        }
                                        else
                                        {
                                            TSPlayer.All.SendInfoMessage(args.Player.Name + " пригласила " + plr2.Name + " на свидание!");
                                        }
                                        plr2.SendInfoMessage("Чтобы принять запрос на свидание пропишите /date -confirm");
                                        plr2.SendInfoMessage("Чтобы отклонить запрос на свидание пропишите /date -reject");
                                    }
                                    else
                                    {
                                        args.Player.SendErrorMessage("Ты любишь целовать мальчиков, не так ли?");
                                    }
                                }
                                else
                                {
                                    args.Player.SendErrorMessage("Данный игрок не может ходить на свидания.");
                                }
                            }
                        }
                        else
                        {
                            args.Player.SendErrorMessage("Неверный синтаксис. Синтаксис: /date <игрок> <варп>");
                        }

                        break;
                }
            }
            else
            {
                args.Player.SendErrorMessage("Неверный синтаксис.");
            }
        }

        public static void DivorceCommand(CommandArgs args)
        {

            if (Exists(args.Player.Name))
            {
                var plrN = GetMarried(args.Player.Name);
                if (plrN != string.Empty)
                {
                    var plrs = TSPlayer.FindByNameOrID(plrN);

                    args.Player.SendSuccessMessage("Вы ушли от " + plrN + ".");

                    SaveMarriage(args.Player.Name, string.Empty);
                    SaveMarriage(plrN, string.Empty);
                    

                    if (plrs.Count > 0)
                    {
                        var plr = plrs[0];
                        if (Main.LocalPlayer.Male)
                        {
                            plr.SendInfoMessage(string.Format("{0} ушёл от вас :(", args.Player.Name));
                        }
                        else
                        {
                            plr.SendInfoMessage(string.Format("{0} ушла от вас :(", args.Player.Name));
                        }
                    }

                }
                else
                {
                    args.Player.SendErrorMessage("Вы не женаты.");
                }
            }
            else
            {
                args.Player.SendErrorMessage("Вы не женаты.");
            }


        }




        public static void NSFWCommand_1(CommandArgs args)
        {
            var parameter = args.Parameters;

            if (parameter.Count > 0)
            {
                var plrs = TSPlayer.FindByNameOrID(args.Parameters[0]);

                if (plrs.Count > 1)
                {
                    args.Player.SendMultipleMatchError(plrs.Select(p => p.Name));
                }
                else if (plrs.Count == 0)
                {
                    args.Player.SendErrorMessage("Такого игрока нет на сервере.");
                }
                else
                {
                    var plr = plrs[0];
                    
                    if (Exists(plr.Name) && useable[plr.Index])
                    {
                        var nickname = GetMarried2(plr.Name);

                        if (nickname == args.Player.Name)
                        {
                            args.Player.SendSuccessMessage("Вы.. аэм.. занялись половым размножением с... " + plr.Name + "...");

                            plr.SetBuff(119, 500);
                            plr.SetBuff(169, 500);
                            args.Player.SetBuff(169, 500);
                            args.Player.SetBuff(119, 500);

                            if (Main.LocalPlayer.Male)
                            {
                                plr.SendInfoMessage(string.Format("{0} занялся с вами половым размножением...?", args.Player.Name));
                            }
                            else
                            {
                                plr.SendInfoMessage(string.Format("{0} занялась с вами половым размножением...?", args.Player.Name));
                            }
                        }
                        else
                        {
                            args.Player.SendErrorMessage("Я знаю где ты живешь...");

                        }



                    }
                    else
                    {
                        args.Player.SendErrorMessage("Я знаю где ты живешь...");
                    }

                }
            }
            else
            {
                args.Player.SendErrorMessage("Неверный синтаксис. Синтаксис: /sex <игрок>");
            }
        }
        public static void SlapCommand(CommandArgs args)
        {
            if (args.Parameters.Count > 0)
            {
                var plrs = TSPlayer.FindByNameOrID(args.Parameters[0]);

                if (plrs.Count > 1)
                {
                    args.Player.SendMultipleMatchError(plrs.Select(p => p.Name));
                }
                else if (plrs.Count == 0)
                {
                    args.Player.SendErrorMessage("Такого игрока нет на сервере.");
                }
                else
                {
                    // Зум крутой
                    var rand = new Random();
                    if (rand.NextDouble() > 0.8)
                    {
                        TSPlayer.All.SendMessage("Sticker крутой!", Color.Crimson);
                    }
                    var plr = plrs[0];
                    if (useable[plr.Index])
                    {
                        plr.SetBuff(103, 240);
                        plr.DamagePlayer(5);

                        args.Player.SendSuccessMessage("Вы шлёпнули " + plr.Name + ".");

                        if (Main.LocalPlayer.Male)
                        {
                            TSPlayer.All.SendInfoMessage(args.Player.Name + " шлёпнул игрока " + plr.Name);
                            plr.SendMessage(args.Player.Name + " шлёпнул вас ~", Color.Pink);
                        }
                        else
                        {
                            plr.SendMessage(args.Player.Name + " шлёпнула вас ~", Color.Pink);
                            TSPlayer.All.SendInfoMessage(args.Player.Name + " шлёпнула игрока " + plr.Name);
                        }
                    }
                    else
                    {
                        args.Player.SendErrorMessage("Данный игрок защищён от команд.");
                    }
                }
            }
            else
            {
                args.Player.SendErrorMessage("Неверный синтаксис. Синтаксис: /plunk <игрок>");
            }

        }
        public static void PatCommand(CommandArgs args)
        {
            var parameter = args.Parameters;

            if (parameter.Count > 0)
            {

                var plrs = TSPlayer.FindByNameOrID(args.Parameters[0]);

                if (plrs.Count > 1)
                {
                    args.Player.SendMultipleMatchError(plrs.Select(p => p.Name));
                }
                else if (plrs.Count == 0)
                {
                    args.Player.SendErrorMessage("Такого игрока нет на сервере.");
                }
                else
                {
                    var plr = plrs[0];
                    if (useable[plr.Index])
                    {
                        args.Player.SendSuccessMessage("Вы погладили " + plr.Name + ".");

                        if (Main.LocalPlayer.Male)
                        {
                            TSPlayer.All.SendInfoMessage(args.Player.Name + " погладил игрока " + plr.Name);
                            plr.SendMessage(args.Player.Name + " погладил вас по голове ~", Color.HotPink);
                        }
                        else
                        {
                            plr.SendMessage(args.Player.Name + " погладила вас по голове ~", Color.HotPink);
                            TSPlayer.All.SendInfoMessage(args.Player.Name + " погладила игрока " + plr.Name);
                        }
                    }
                    else
                    {
                        args.Player.SendErrorMessage("Данный игрок защищён от команд.");
                    }
                }
            }
            else
            {
                args.Player.SendErrorMessage("Неверный синтаксис. Синтаксис: /pat <игрок>");
            }
        }
        public static void BeatCommand(CommandArgs args)
        {
            var parameter = args.Parameters;

            if (parameter.Count > 0)
            {
                var plrs = TSPlayer.FindByNameOrID(args.Parameters[0]);

                if (plrs.Count > 1)
                {
                    args.Player.SendMultipleMatchError(plrs.Select(p => p.Name));
                }
                else if (plrs.Count == 0)
                {
                    args.Player.SendErrorMessage("Такого игрока нет на сервере.");
                }
                else
                {
                    var plr = plrs[0];
                    if (useable[plr.Index])
                    {
                        plr.SetBuff(31, 240);
                        plr.DamagePlayer(plr.TPlayer.statLifeMax / 4);

                        args.Player.SendSuccessMessage("Вы ударили " + plr.Name + "!");

                        if (Main.LocalPlayer.Male)
                        {
                            plr.SendMessage(args.Player.Name + " ударил вас! Об отношениях не может быть и речи...", Color.Red);
                            TSPlayer.All.SendInfoMessage(args.Player.Name + " ударил " + plr.Name);
                        }
                        else
                        {
                            plr.SendMessage(args.Player.Name + " ударила вас! Об отношениях не может быть и речи...", Color.Red);
                            TSPlayer.All.SendInfoMessage(args.Player.Name + " ударила " + plr.Name);
                        }
                    }
                    else
                    {
                        args.Player.SendErrorMessage("Данный игрок защищён от команд.");
                    } 
                }
            }
            else
            {
                args.Player.SendErrorMessage("Неверный синтаксис. Синтаксис: /beat <игрок>");
            }
        }
        public static void MarryCommand(CommandArgs args)
        {
            var parameter = args.Parameters;
            if (parameter.Count > 0)
            {
                switch (parameter[0])
                {
                    case "-cancel":

                        var plrR4 = RelationsPlugin.requests[args.Player.Index];

                        var plrs4 = TSPlayer.FindByNameOrID(plrR4.Replace(" ⌇", ""));

                        if (plrs4.Count > 0)
                        {
                            foreach (TSPlayer plr in plrs4)
                            {
                                if (RelationsPlugin.requests[plr.Index] == args.Player.Name && RelationsPlugin.requests[args.Player.Index] == plr.Name + " ⌇")
                                {
                                    RelationsPlugin.requests[args.Player.Index] = null;
                                    RelationsPlugin.requests[plr.Index] = null;

                                    TSPlayer.All.SendInfoMessage("Заявка на свадьбу игрока " + args.Player.Name + " была отменена.");
                                    args.Player.SendSuccessMessage("Вы отменили свадьбу.");

                                }
                            }
                        }


                        break;

                    case "-reject":


                        var plrR2 = RelationsPlugin.requests[args.Player.Index];

                        var plrs = TSPlayer.FindByNameOrID(plrR2);

                        if (plrs.Count > 0)
                        {
                            foreach (TSPlayer plr in plrs)
                            {
                                if (RelationsPlugin.requests[args.Player.Index] == plr.Name && RelationsPlugin.requests[plr.Index] == args.Player.Name + " ⌇")
                                {
                                    RelationsPlugin.requests[args.Player.Index] = null;
                                    RelationsPlugin.requests[plr.Index] = null;

                                    args.Player.SendSuccessMessage("Вы отклонили заявку игрока " + plr.Name + " на свадьбу.");

                                    if (Main.LocalPlayer.Male)
                                    {
                                        TSPlayer.All.SendInfoMessage(args.Player.Name + " отклонил заявку " + plr.Name + " на свадьбу :(");
                                    }
                                    else
                                    {
                                        TSPlayer.All.SendInfoMessage(args.Player.Name + " отклонила заявку " + plr.Name + " на свадьбу :(");
                                    }
                                }
                            }

                        }
                        else
                        {
                            args.Player.SendInfoMessage("У вас нет запросов.");
                        }
                        break;

                    case "-confirm":

                        var plrR = RelationsPlugin.requests[args.Player.Index];

                        var plrs2 = TSPlayer.FindByNameOrID(plrR);

                        if (plrs2.Count > 0)
                        {
                            foreach (TSPlayer plr in plrs2)
                            {

                                if (RelationsPlugin.requests[args.Player.Index] == plr.Name && RelationsPlugin.requests[plr.Index] == args.Player.Name + " ⌇")
                                {

                                    if (Exists(args.Player.Name))
                                    {
                                        SaveMarriage(args.Player.Name, plr.Name);
                                    }
                                    else
                                    {
                                        CreateMarriage(args.Player.Name, plr.Name);
                                    }

                                    if (Exists(plr.Name))
                                    {
                                        SaveMarriage(plr.Name, args.Player.Name);
                                    }
                                    else
                                    {
                                        CreateMarriage(plr.Name, args.Player.Name);
                                    }



                                    RelationsPlugin.requests[args.Player.Index] = null;
                                    RelationsPlugin.requests[plr.Index] = null;

                                    args.Player.SendInfoMessage("Теперь вы женаты на " + plr.Name + ".");

                                    if (Main.LocalPlayer.Male)
                                    {
                                        TSPlayer.All.SendInfoMessage(args.Player.Name + " теперь женат на " + plr.Name + "!");
                                    }
                                    else
                                    {
                                        TSPlayer.All.SendInfoMessage(args.Player.Name + " теперь жената на " + plr.Name + "!");
                                    }
                                }
                            }

                        }
                        else
                        {
                            args.Player.SendInfoMessage("У вас нет запросов.");
                        }
                        break;

                    case "-info":

                        if (parameter.Count < 2)
                        {

                            if (Exists(args.Player.Name))
                            {
                                if (GetMarried(args.Player.Name) != string.Empty)
                                {

                                    var nickname = GetMarried(args.Player.Name);

                                    args.Player.SendInfoMessage("Вы женаты с " + nickname);
                                }
                                else
                                {
                                    args.Player.SendInfoMessage("Вы не женаты.");
                                }
                            }
                            else
                            {
                                args.Player.SendInfoMessage("Вы не женаты.");
                            }
                        }
                        else
                        {
                            if (Exists(parameter[1]))
                            {
                                if (GetMarried(parameter[1]) != string.Empty)
                                {

                                    var nickname = GetMarried(parameter[1]);

                                    args.Player.SendInfoMessage(parameter[1] + " женат с " + nickname);
                                }
                                else
                                {
                                    args.Player.SendInfoMessage(parameter[1] + " не женат.");
                                }
                            }
                            else
                            {
                                args.Player.SendInfoMessage(parameter[1] + " не женат.");
                            }

                        }

                        break;

                    default:


                        var plrs3 = TSPlayer.FindByNameOrID(parameter[0]);
                        if (plrs3.Count > 1)
                        {
                            args.Player.SendMultipleMatchError(plrs3.Select(p => p.Name));
                        }
                        else if (plrs3.Count == 0)
                        {
                            args.Player.SendErrorMessage("Такого игрока нет на сервере.");
                        }
                        else
                        {
                            if (RelationsPlugin.requests[args.Player.Index] == null)
                            {
                                var plr = plrs3[0];
                                if (plr.IsLoggedIn && plr.HasPermission("relations.marry") && useable[plr.Index])
                                {
                                    if (plr.Name != args.Player.Name)
                                    {
                                        TSPlayer.All.SendInfoMessage(args.Player.Name + " хочет жениться на " + plr.Name);
                                        plr.SendInfoMessage("Чтобы принять предложение, пропишите /marry -confirm");
                                        plr.SendInfoMessage("Чтобы отклонить предложение, пропишите /marry -reject");

                                        args.Player.SendInfoMessage("Чтобы отменить запрос пропишите /marry -cancel");


                                        RelationsPlugin.requests[plr.Index] = args.Player.Name;
                                        RelationsPlugin.requests[args.Player.Index] = plr.Name + " ⌇";
                                    }
                                    else
                                    {
                                        args.Player.SendErrorMessage("Ты чо на себе хочешь жениться нарциссист хренов??");
                                    }
                                }
                                else
                                {
                                    args.Player.SendErrorMessage("Данный игрок не может жениться.");
                                }

                            }
                            else
                            {
                                args.Player.SendErrorMessage("Вы уже подали заявку на свадьбу или не приняли существующую.");
                            }
                        }
                        break;
                }

            }
            else
            {
                args.Player.SendErrorMessage("Неверный синтаксис.");
            }
        }
    }
}



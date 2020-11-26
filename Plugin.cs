using Rocket.API;
using Rocket.Core.Commands;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Linq;
using UnityEngine;

namespace WhiteListSystem
{
    public class Plugin : RocketPlugin<Configuration>
    {
        [RocketCommand("whitelist", "", "", AllowedCaller.Both)]
        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = caller as UnturnedPlayer;
            string ID = args[1].ToString();
            UnturnedPlayer targetPlayer = UnturnedPlayer.FromName(args[1]);
            Configuration c = Configuration.Instance;

            if (player.HasPermission(c.AddRemovePerm))
            {
                if (args.Count() > 1)
                {
                    if (args[0] == "add")
                    {
                        c.PlayerIdList.Add(ID);
                        UnturnedChat.Say(player, $"You have whitelisted {ID}.", Color.cyan);
                    }
                    else if (args[0] == "remove")
                    {
                        if (args[1] == targetPlayer.ToString())
                        {
                            c.PlayerIdList.Remove(targetPlayer.CSteamID.ToString());
                            UnturnedChat.Say(player, $"You have unwhitelisted {targetPlayer.DisplayName} - {player.CSteamID}.", Color.yellow);
                            targetPlayer.Kick("You have unwhitelisted.");
                        }
                        else if (args[1] == ID)
                        {
                            c.PlayerIdList.Remove(ID);
                            UnturnedChat.Say(player, $"You have unwhitelisted {ID}.", Color.yellow);
                        }
                    }
                    else
                    {
                        UnturnedChat.Say(player, "Unknow command.", Color.red);
                    }
                }
                else
                {
                    UnturnedChat.Say(player, "Usage : /whiteist <add or remove> <playername>", Color.red);
                }
            }
            else
            {
                UnturnedChat.Say(player, "You don't have a permission.", Color.red);
            }
        }
        protected override void Load()
        {
            U.Events.OnPlayerConnected += Connected;
        }
        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= Connected;
        }
        private void Connected(UnturnedPlayer player)
        {
            Configuration c = Configuration.Instance;

            if (c.PlayerIdList.Contains(player.CSteamID.ToString()))
            {
                return;
            }
            else
            {
                player.Kick("You are not whitelisted.");
                Console.WriteLine($"{player.CSteamID} - {player.SteamName} kicked from the server but it was not whitelisted.", Console.ForegroundColor = ConsoleColor.Yellow);
                Console.ResetColor();
            }
        }
    }
}

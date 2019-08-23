using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using b1g.Services;

namespace b1g.Modules
{
    public class Common : ModuleBase<SocketCommandContext>
    {
        [Command("userinfo")]
        public async Task UserInfo()
        {
            SocketUser user = Context.User;
            if (Context.Channel.Id == (ulong)609542348287770624)
            {
                string joined = await Database.DbHandler.GetUserJoinedDate(user.Id.ToString());
                int mensajes = await Database.DbHandler.GetUserMessages(user.Id.ToString());
                string joined_msg;
                if (joined == "0")
                    joined_msg = "Sin registrar";
                else
                    joined_msg = joined;


                EmbedBuilder builder = new EmbedBuilder();

                builder.WithTitle($"Estadísticas de {user.ToString()}");
                builder.AddField("Fecha de creación", $"{user.CreatedAt}", true);
                builder.AddField("ID del usuario", $"{user.Id}", true);
                builder.AddField("Fecha de entrada al servidor", $"{joined_msg}", true);
                builder.AddField("Mensajes totales en ODCode", $"{mensajes}", true);
                builder.WithThumbnailUrl(user.GetAvatarUrl());

                builder.WithColor(Color.Red);
                await Context.Channel.SendMessageAsync("", false, builder.Build());
            }
            else await ReplyAsync("⚠️ Dirígete a #comandos-bot para usar mis comandos.");
        }

        [Command("avatar")]
        public async Task Avatar(SocketGuildUser target)
        {
            if (Context.Channel.Id == (ulong)609542348287770624)
            {
                await ReplyAsync(target.GetAvatarUrl().ToString());
            }
            else await ReplyAsync("⚠️ Dirígete a #comandos-bot para usar mis comandos.");
        }

        [Command("sampstatus")]
        public async Task SampStatus(string ip, ushort port)
        {
            if (Context.Channel.Id == (ulong)609542348287770624)
            {
                SampQuery api = new SampQuery(ip, port, 'i');
                EmbedBuilder builder = new EmbedBuilder();

                string nombrecat = "";
                string valorcat = "";

                foreach (KeyValuePair<string, string> kvp in api.read(true))
                {

                    switch (kvp.Key)
                    {
                        case "password":
                            nombrecat = "Contraseña";
                            break;

                        case "players":
                            nombrecat = "Jugadores en linea";
                            break;

                        case "maxplayers":
                            nombrecat = "Slots";
                            break;

                        case "hostname":
                            nombrecat = "Nombre del servidor";
                            break;

                        case "gamemode":
                            nombrecat = "Gamemode";
                            break;

                        case "mapname":
                            nombrecat = "Mapa";
                            break;

                        default:
                            nombrecat = kvp.Key;
                            break;
                    }

                    if (kvp.Value == "0")
                        valorcat = "No";
                    else if (kvp.Value == "1")
                        valorcat = "Si";
                    else
                        valorcat = kvp.Value;

                    builder.WithTitle($"Estadísticas de {ip}:{port}");
                    builder.AddField($"{nombrecat}", $"{valorcat}", true);    // true - for inline
                    builder.WithThumbnailUrl("http://i.imgur.com/QnqZoTC.png");
                }

                builder.WithColor(Color.Red);

                if (valorcat == nombrecat) await ReplyAsync("⚠️ Los datos proporcionados no muestran ningún servidor en linea.");
                else await Context.Channel.SendMessageAsync("", false, builder.Build());
            }
            else await ReplyAsync("⚠️ Dirígete a #comandos-bot para usar mis comandos.");
        }
    }
}

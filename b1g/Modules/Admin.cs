using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace b1g.Modules
{
    public class Admin : ModuleBase<SocketCommandContext>
    {
        [Command("ban")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanUserAsync(SocketGuildUser target, [Remainder] string reason = null)
        {
            SocketUser user = Context.User;
            var guild = target.Guild;

            await target.SendMessageAsync($"⚠️ Has sido baneado de ODCode || Razón: {reason}. Si crees que ha sido un error puedes apelar en el foro (https://odcode.net)");
            await target.Guild.AddBanAsync(target, reason: reason);
            await ReplyAsync($"👺 {user.Username.ToString()} ha baneado a {target.Username.ToString()} — Razón: **{reason}**");
            await Database.DbHandler.InsertDbBan(target.Id.ToString(), user.Id.ToString(), reason, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

            var chnl = guild.GetChannel(Config.logsMod) as IMessageChannel;
            await chnl.SendMessageAsync($"⚠️ {user.Username.ToString()} ha baneado a {target.Username.ToString()} — Razón: **{reason}** || `{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}`");
        }

        [Command("kick")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task KickUserAsync(SocketGuildUser target, [Remainder] string reason = null)
        {
            SocketUser user = Context.User;
            var guild = target.Guild;

            await target.SendMessageAsync($"⚠️ Has sido kickeado de ODCode || Razón: {reason}.");
            await target.KickAsync(reason: reason);
            await ReplyAsync($"👺 {user.Username.ToString()} ha kickeado a {target.Username.ToString()} — Razón: **{reason}**");
            await Database.DbHandler.InsertDbKick(target.Id.ToString(), user.Id.ToString(), reason, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

            var chnl = guild.GetChannel(Config.logsMod) as IMessageChannel;
            await chnl.SendMessageAsync($"⚠️ {user.Username.ToString()} ha kickeado a {target.Username.ToString()} — Razón: **{reason}** || `{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}`");
        }

        [Command("mute")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task MuteUserAsync(SocketGuildUser target, [Remainder] string reason = null)
        {
            SocketUser user = Context.User;
            var guild = target.Guild;

            await target.SendMessageAsync($"⚠️ Has sido muteado de ODCode || Razón: {reason}.");
            var role = Context.Guild.GetRole(Config.mutedRol);
            await ((SocketGuildUser)target).AddRoleAsync(role);

            await ReplyAsync($"👺 {user.Username.ToString()} ha muteado a {target.Username.ToString()} — Razón: **{reason}**");
            await Database.DbHandler.InsertDbMuted(target.Id.ToString(), user.Id.ToString(), reason, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

            var chnl = guild.GetChannel(Config.logsMod) as IMessageChannel;
            await chnl.SendMessageAsync($"⚠️ {user.Username.ToString()} ha muteado a {target.Username.ToString()} — Razón: **{reason}** || `{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}`");
        }

        [Command("unmute")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task UnmuteUserAsync(SocketGuildUser target)
        {
            SocketUser user = Context.User;
            var guild = target.Guild;

            await target.SendMessageAsync($"⚠️ Has sido desmuteado de ODCode");
            var role = Context.Guild.GetRole(Config.mutedRol);
            await ((SocketGuildUser)target).RemoveRoleAsync(role);

            await ReplyAsync($"👺 {user.Username.ToString()} ha desmuteado a {target.Username.ToString()}");
            await Database.DbHandler.DeleteDbMuted(target.Id.ToString());

            var chnl = guild.GetChannel(Config.logsMod) as IMessageChannel;
            await chnl.SendMessageAsync($"⚠️ {user.Username.ToString()} ha desmuteado a {target.Username.ToString()} || `{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}`");
        }

        [Command("strike")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task StrikeUserAsync(SocketGuildUser target, [Remainder] string reason = null)
        {
            SocketUser user = Context.User;
            var guild = target.Guild;

            await target.SendMessageAsync($"⚠️ Has recibido un strike en ODCode. Razón: `{reason}`");

            await ReplyAsync($"👺 {user.Username.ToString()} ha strikeado a {target.Username.ToString()} | Razón: `{reason}`");
            await Database.DbHandler.InsertDbStrike(target.Id.ToString(), user.Id.ToString(), reason, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

            var chnl = guild.GetChannel(Config.logsMod) as IMessageChannel;
            await chnl.SendMessageAsync($"⚠️ {user.Username.ToString()} ha strikeado a {target.Username.ToString()}. Razón: {reason} || `{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}`");

            int numberOfStrikes = await Database.DbHandler.CheckUserStrikes(target.Id.ToString());
            if(numberOfStrikes >= 3)
            {
                await target.SendMessageAsync($"⚠️ Has sido baneado de ODCode || Razón: cúmulo de strikes (3). Si crees que ha sido un error puedes apelar en el foro (https://odcode.net)");
                await target.Guild.AddBanAsync(target, reason: reason);
                await ReplyAsync($"👺 {target.Username.ToString()} ha sido baneado por límite de strikes (3)");
                await Database.DbHandler.InsertDbBan(target.Id.ToString(), "strikes", reason, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                await chnl.SendMessageAsync($"⚠️ {target.Username.ToString()} ha sido baneado por cúmulo de strikes (3) || `{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}`");
            }
        }

        [Command("unstrike")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task UnstrikeUserAsync(SocketGuildUser target)
        {
            SocketUser user = Context.User;
            var guild = target.Guild;

            await target.SendMessageAsync($"⚠️ {user.Username.ToString()} te ha quitado un strike en ODCode.");

            await ReplyAsync($"👺 {user.Username.ToString()} le ha quitado un strike a {target.Username.ToString()}");
            await Database.DbHandler.DeleteUserStrike(target.Id.ToString());

            var chnl = guild.GetChannel(Config.logsMod) as IMessageChannel;
            await chnl.SendMessageAsync($"⚠️ {user.Username.ToString()} le ha quitado un strike a {target.Username.ToString()} || `{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}`");
        }

        [Command("infouser")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task UserInfo(SocketGuildUser user)
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

        [Command("say")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task SayBot([Remainder] string mensaje)
        {
            await ReplyAsync(mensaje);
            var message = Context.Message;
            await message.DeleteAsync();
        }

        [Command("resetxp")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task ResetXp(SocketGuildUser target, [Remainder] string mensaje)
        {
            var guild = target.Guild;
            SocketUser user = Context.User;
            await Database.DbHandler.UpdateDbUserLevel(target.Id.ToString(), 0);

            await target.SendMessageAsync($"⚠️ {user.Username.ToString()} te ha reseteado el XP a 0. | Razón: {mensaje}");
            await ReplyAsync($"👺 {user.Username.ToString()} ha resetado el XP de {target.Username.ToString()} | Razón: {mensaje}");

            var chnl = guild.GetChannel(Config.logsMod) as IMessageChannel;
            await chnl.SendMessageAsync($"⚠️ {user.Username.ToString()} ha reseteado el XP de {target.Username.ToString()}. Razón: {mensaje} || `{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}`");
        }

        [Command("bigaso")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task bigasote(SocketGuildUser target, [Remainder] string reason = null)
        {
            var role = Context.Guild.GetRole(Config.levelRole4);
            await ((SocketGuildUser)target).AddRoleAsync(role);
        }

        [Command("pleb")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task pleb(SocketGuildUser target, [Remainder] string reason = null)
        {
            var role = Context.Guild.GetRole(Config.levelRole1);
            await ((SocketGuildUser)target).AddRoleAsync(role);
        }

        [Command("nomorbig")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task nomorbig(SocketGuildUser target, [Remainder] string reason = null)
        {
            var role = Context.Guild.GetRole(Config.levelRole4);
            await ((SocketGuildUser)target).RemoveRoleAsync(role);
        }

    }
}

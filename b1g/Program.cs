using b1g.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Victoria;

namespace b1g
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task RunBotAsync()
        {
            Console.WriteLine("b1g, un bot de Discord pendejo | developed by Muphy");

            var _config = new DiscordSocketConfig { MessageCacheSize = 1000 };
            _client = new DiscordSocketClient(_config);
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton<LavaRestClient>()
                .AddSingleton<LavaSocketClient>()
                .AddSingleton<MusicService>()
                .BuildServiceProvider();

            string botToken = "NjEwNTc4MDQwNjc3NzI4MjY2.XVHTNw.NcJpvOTzcvUAzW97juxU0LJdC-A";

            _client.Log += Log;
            _client.UserJoined += WooUserJoined;
            _client.MessageUpdated += WooMessageUpdated;
            _client.MessageDeleted += WooMessageDeleted;
            _client.ReactionAdded += WooReactionAdded;
            _client.ReactionRemoved += WooReactionRemoved;
            _client.MessageReceived += WooMessageReceived;

            await _services.GetRequiredService<MusicService>().InitializeAsync();

            await RegisterCommandAsync();

            await _client.LoginAsync(TokenType.Bot, botToken);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task WooMessageReceived(SocketMessage arg)
        {
            if (arg.Author.IsBot)
                return;

            string date = await Database.DbHandler.GetUserLevel(arg.Author.Id.ToString());

            if(date == "0")
            {
                await Database.DbHandler.InsertDbUserLevel(arg.Author.Id.ToString());
                return;
            }

            if(date != DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
            {
                int message = await Database.DbHandler.GetUserMessages(arg.Author.Id.ToString());
                int messages = message + 1;
                await Database.DbHandler.UpdateDbUserLevel(arg.Author.Id.ToString(), message);

                if(messages == 200)
                {
                    var guild = _client.GetGuild(Config.mainGuild);
                    var role = guild.GetRole(Config.levelRole1);
                    await ((SocketGuildUser)arg.Author).AddRoleAsync(role);

                    await arg.Author.SendMessageAsync("🔔 ¡Felicidades! Has obtenido el rol 'fdsdsf' por tu actividad en el Discord de ODCode.");
                }

                if (messages == 1000)
                {
                    var guild = _client.GetGuild(Config.mainGuild);
                    var role = guild.GetRole(Config.levelRole2);
                    await ((SocketGuildUser)arg.Author).AddRoleAsync(role);

                    var roleno = guild.GetRole(Config.levelRole1);
                    await ((SocketGuildUser)arg.Author).RemoveRoleAsync(roleno);

                    await arg.Author.SendMessageAsync("🔔 ¡Felicidades! Has obtenido el rol 'fdsdsf' por tu actividad en el Discord de ODCode.");
                }

                if (messages == 3000)
                {
                    var guild = _client.GetGuild(Config.mainGuild);
                    var role = guild.GetRole(Config.levelRole3);
                    await ((SocketGuildUser)arg.Author).AddRoleAsync(role);

                    var roleno = guild.GetRole(Config.levelRole2);
                    await ((SocketGuildUser)arg.Author).RemoveRoleAsync(roleno);

                    await arg.Author.SendMessageAsync("🔔 ¡Felicidades! Has obtenido el rol 'fdsdsf' por tu actividad en el Discord de ODCode.");
                }

                if (messages == 7000)
                {
                    var guild = _client.GetGuild(Config.mainGuild);
                    var role = guild.GetRole(Config.levelRole4);
                    await ((SocketGuildUser)arg.Author).AddRoleAsync(role);

                    var roleno = guild.GetRole(Config.levelRole3);
                    await ((SocketGuildUser)arg.Author).RemoveRoleAsync(roleno);

                    await arg.Author.SendMessageAsync("🔔 ¡Felicidades! Has obtenido el rol 'fdsdsf' por tu actividad en el Discord de ODCode.");
                }
            }

        }

        private async Task WooReactionRemoved(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            Emoji rol1 = new Emoji("😘");
            Emoji rol2 = new Emoji("😃");
            Emoji rol3 = new Emoji("😴");
            Emoji rol4 = new Emoji("😛");
            Emoji rol5 = new Emoji("😇");

            var guild = _client.GetGuild(Config.mainGuild);
            var chnl = _client.GetChannel(Config.logsChannel) as IMessageChannel;

            if (arg1.Id == Config.messageRoles)
            {
                if (arg3.Emote.Name == rol1.Name)
                {
                    var role = guild.GetRole(Config.reactionRole1);
                    await ((SocketGuildUser)arg3.User).RemoveRoleAsync(role);

                    await Database.DbHandler.DeleteDbUserRole(arg3.UserId.ToString(), 1);
                    await chnl.SendMessageAsync($"🔔 {arg3.User} se ha quitado el rol {rol1}");
                }

                if (arg3.Emote.Name == rol2.Name)
                {
                    var role = guild.GetRole(Config.reactionRole2);
                    await ((SocketGuildUser)arg3.User).RemoveRoleAsync(role);

                    await Database.DbHandler.DeleteDbUserRole(arg3.UserId.ToString(), 2);
                    await chnl.SendMessageAsync($"🔔 {arg3.User} se ha quitado el rol {rol2}");
                }

                if (arg3.Emote.Name == rol3.Name)
                {
                    var role = guild.GetRole(Config.reactionRole3);
                    await ((SocketGuildUser)arg3.User).RemoveRoleAsync(role);

                    await Database.DbHandler.DeleteDbUserRole(arg3.UserId.ToString(), 3);
                    await chnl.SendMessageAsync($"🔔 {arg3.User} se ha quitado el rol {rol3}");
                }

                if (arg3.Emote.Name == rol4.Name)
                {
                    var role = guild.GetRole(Config.reactionRole4);
                    await ((SocketGuildUser)arg3.User).RemoveRoleAsync(role);

                    await Database.DbHandler.DeleteDbUserRole(arg3.UserId.ToString(), 4);
                    await chnl.SendMessageAsync($"🔔 {arg3.User} se ha quitado el rol {rol4}");
                }

                if (arg3.Emote.Name == rol5.Name)
                {
                    var role = guild.GetRole(Config.reactionRole5);
                    await ((SocketGuildUser)arg3.User).RemoveRoleAsync(role);

                    await Database.DbHandler.DeleteDbUserRole(arg3.UserId.ToString(), 5);
                    await chnl.SendMessageAsync($"🔔 {arg3.User} se ha quitado el rol {rol5}");
                }
            }
        }

        private async Task WooReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            Emoji rol1 = new Emoji("😘");
            Emoji rol2 = new Emoji("😃");
            Emoji rol3 = new Emoji("😴");
            Emoji rol4 = new Emoji("😛");
            Emoji rol5 = new Emoji("😇");

            var guild = _client.GetGuild(Config.mainGuild);
            var chnl = _client.GetChannel(Config.logsChannel) as IMessageChannel;

            if (arg1.Id == Config.messageRoles)
            {
                if(arg3.Emote.Name == rol1.Name)
                {
                    var role = guild.GetRole(Config.reactionRole1);
                    await ((SocketGuildUser)arg3.User).AddRoleAsync(role);

                    await Database.DbHandler.InsertDbUserRole(arg3.UserId.ToString(), 1);
                    await chnl.SendMessageAsync($"🔔 {arg3.User} ha obtenido el rol {rol1}");
                }

                if (arg3.Emote.Name == rol2.Name)
                {
                    var role = guild.GetRole(Config.reactionRole2);
                    await ((SocketGuildUser)arg3.User).AddRoleAsync(role);

                    await Database.DbHandler.InsertDbUserRole(arg3.UserId.ToString(), 2);
                    await chnl.SendMessageAsync($"🔔 {arg3.User} ha obtenido el rol {rol2}");
                }

                if (arg3.Emote.Name == rol3.Name)
                {
                    var role = guild.GetRole(Config.reactionRole3);
                    await ((SocketGuildUser)arg3.User).AddRoleAsync(role);

                    await Database.DbHandler.InsertDbUserRole(arg3.UserId.ToString(), 3);
                    await chnl.SendMessageAsync($"🔔 {arg3.User} ha obtenido el rol {rol3}");
                }

                if (arg3.Emote.Name == rol4.Name)
                {
                    var role = guild.GetRole(Config.reactionRole4);
                    await ((SocketGuildUser)arg3.User).AddRoleAsync(role);

                    await Database.DbHandler.InsertDbUserRole(arg3.UserId.ToString(), 4);
                    await chnl.SendMessageAsync($"🔔 {arg3.User} ha obtenido el rol {rol4}");
                }

                if (arg3.Emote.Name == rol5.Name)
                {
                    var role = guild.GetRole(Config.reactionRole5);
                    await ((SocketGuildUser)arg3.User).AddRoleAsync(role);

                    await Database.DbHandler.InsertDbUserRole(arg3.UserId.ToString(), 5);
                    await chnl.SendMessageAsync($"🔔 {arg3.User} ha obtenido el rol {rol5}");
                }
            }
        }

        private async Task WooMessageDeleted(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            var oldmsj = await arg1.GetOrDownloadAsync();
            IUser user = oldmsj.Author;
            var chnl = _client.GetChannel(Config.logsChannel) as IMessageChannel;

            await chnl.SendMessageAsync($"🔔 {user.Mention} ha borrado el siguiente mensaje: \n" +
                $"`{oldmsj}` \n" +
                $"\n" +
                $"`{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}`");
        }

        private async Task WooMessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
        {
            IUser user = arg2.Author;
            var chnl = _client.GetChannel(Config.logsChannel) as IMessageChannel;
            var oldmsj = await arg1.GetOrDownloadAsync();

            if (oldmsj.ToString() == arg2.ToString())
                return;

            await chnl.SendMessageAsync($"🔔 {user.Mention} ha editado el siguiente mensaje: \n" +
                $"`{oldmsj}` \n" +
                $"\n" +
                $"`{arg2}` \n" +
                $"\n" +
                $"`{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}`");
        }

        private async Task WooUserJoined(SocketGuildUser user)
        {
            var guild = user.Guild;
            var chnl = _client.GetChannel(Config.logsChannel) as IMessageChannel;

            if(await Database.DbHandler.CheckIfUserIsMuted(user.Id.ToString()))
            {
                var role = user.Guild.GetRole(Config.mutedRol);
                await ((SocketGuildUser)user).AddRoleAsync(role);
            }

            if (await Database.DbHandler.CheckIfUserIsJoined(user.Id.ToString()) == false)
                await Database.DbHandler.InsertDbUserJoin(user.Id.ToString());


            await chnl.SendMessageAsync($"🔔 {user.Mention} se ha unido al Discord. || `{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}`");
            await user.SendMessageAsync("🔊 ¡Bienvenido al Discord de ODCode! (https://odcode.net)");

            var role1 = guild.GetRole(Config.reactionRole1);
            var role2 = guild.GetRole(Config.reactionRole2);
            var role3 = guild.GetRole(Config.reactionRole3);
            var role4 = guild.GetRole(Config.reactionRole4);
            var role5 = guild.GetRole(Config.reactionRole5);

            if (await Database.DbHandler.CheckIfUserHasRole(user.Id.ToString(), 1))
                await ((SocketGuildUser)user).AddRoleAsync(role1);

            if (await Database.DbHandler.CheckIfUserHasRole(user.Id.ToString(), 2))
                await ((SocketGuildUser)user).AddRoleAsync(role2);

            if (await Database.DbHandler.CheckIfUserHasRole(user.Id.ToString(), 3))
                await ((SocketGuildUser)user).AddRoleAsync(role3);

            if (await Database.DbHandler.CheckIfUserHasRole(user.Id.ToString(), 4))
                await ((SocketGuildUser)user).AddRoleAsync(role4);

            if (await Database.DbHandler.CheckIfUserHasRole(user.Id.ToString(), 5))
                await ((SocketGuildUser)user).AddRoleAsync(role5);
        }

        private async Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            var chnl = _client.GetChannel(Config.exceptionLogChannel) as IMessageChannel;
            await chnl.SendMessageAsync($"⚠️ `{arg}`");
        }

        public async Task RegisterCommandAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            if (message == null || message.Author.IsBot)
                return;

            int argPos = 0;

            if (message.HasStringPrefix("b1g", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message);

                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                    Console.WriteLine(result.ErrorReason);
            }
        }
    }
}

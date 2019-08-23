using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace b1g.Modules
{
    public class Music : ModuleBase<SocketCommandContext>
    {
        private Services.MusicService _musicService;

        public Music(Services.MusicService musicService)
        {
            _musicService = musicService;
        }

        [Command("musica")]
        public async Task Join()
        {
            if (Context.Channel.Id == (ulong)609542348287770624)
            {
                var user = Context.User as SocketGuildUser;
                if (user.VoiceChannel is null)
                {
                    await ReplyAsync("⚠️ Necesitas estar conectado a un canal de voz.");
                    return;
                }
                else
                {
                    await _musicService.ConnectAsync(user.VoiceChannel, Context.Channel as ITextChannel);
                    await ReplyAsync($"🔔 b1g se ha conectado a {user.VoiceChannel.Name}");
                }
            }
            else await ReplyAsync("⚠️ Dirígete a #comandos-bot para usar mis comandos.");
        }

        [Command("salir")]
        public async Task Leave()
        {
            if (Context.Channel.Id == (ulong)609542348287770624)
            {
                var user = Context.User as SocketGuildUser;
                if (user.VoiceChannel is null)
                {
                    await ReplyAsync("⚠️ Únete al canal donde está el bot para que salga del canal");
                }
                else
                {
                    await _musicService.LeaveAsync(user.VoiceChannel);
                    await ReplyAsync($"🔔 b1g ha dejado {user.VoiceChannel.Name}");
                }
            }
            else await ReplyAsync("⚠️ Dirígete a #comandos-bot para usar mis comandos.");
        }

        [Command("play")]
        public async Task Play([Remainder]string query)
        {
            if (Context.Channel.Id == (ulong)609542348287770624)
            {
                var result = await _musicService.PlayAsync(query, Context.Guild.Id);
                await ReplyAsync(result);
            }
            else await ReplyAsync("⚠️ Dirígete a #comandos-bot para usar mis comandos.");
        }

        [Command("parar")]
        public async Task Stop()
        {
            if (Context.Channel.Id == (ulong)609542348287770624)
            {
                await _musicService.StopAsync();
                await ReplyAsync("🔔");
            }
            else await ReplyAsync("⚠️ Dirígete a #comandos-bot para usar mis comandos.");
        }

        [Command("pasar")]
        public async Task Skip()
        {
            if (Context.Channel.Id == (ulong)609542348287770624)
            {
                var result = await _musicService.SkipAsync();
                await ReplyAsync(result);
            }
            else await ReplyAsync("⚠️ Dirígete a #comandos-bot para usar mis comandos.");
        }

        [Command("volumen")]
        public async Task Volume(int vol)
        {
            if (Context.Channel.Id == (ulong)609542348287770624)
            {
                await ReplyAsync(await _musicService.SetVolumeAsync(vol));
            }
            else await ReplyAsync("⚠️ Dirígete a #comandos-bot para usar mis comandos.");
        }

        [Command("pausar")]
        public async Task Pause()
        {
            if (Context.Channel.Id == (ulong)609542348287770624)
            {
                await ReplyAsync(await _musicService.PauseOrResumeAsync());
            }
            else await ReplyAsync("⚠️ Dirígete a #comandos-bot para usar mis comandos.");
        }

        [Command("despausar")]
        public async Task Resume()
        {
            if (Context.Channel.Id == (ulong)609542348287770624)
            {
                await ReplyAsync(await _musicService.ResumeAsync());
            }
            else await ReplyAsync("⚠️ Dirígete a #comandos-bot para usar mis comandos.");
        }
    }
}

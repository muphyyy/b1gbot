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

        [Command("salir")]
        public async Task Leave()
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

        [Command("play")]
        public async Task Play([Remainder]string query)
        {
            var result = await _musicService.PlayAsync(query, Context.Guild.Id);
            await ReplyAsync(result);
        }

        [Command("parar")]
        public async Task Stop()
        {
            await _musicService.StopAsync();
            await ReplyAsync("🔔");
        }

        [Command("pasar")]
        public async Task Skip()
        {
            var result = await _musicService.SkipAsync();
            await ReplyAsync(result);
        }

        [Command("volumen")]
        public async Task Volume(int vol)
            => await ReplyAsync(await _musicService.SetVolumeAsync(vol));

        [Command("pausar")]
        public async Task Pause()
            => await ReplyAsync(await _musicService.PauseOrResumeAsync());

        [Command("despausar")]
        public async Task Resume()
            => await ReplyAsync(await _musicService.ResumeAsync());
    }
}

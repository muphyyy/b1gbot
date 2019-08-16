using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Entities;

namespace b1g.Services
{
    public class MusicService
    {
        private LavaRestClient _lavaRestClient;
        private LavaSocketClient _lavaSocketClient;
        private DiscordSocketClient _client;
        private LavaPlayer _player;

        public MusicService(LavaRestClient lavaRestClient, DiscordSocketClient client, LavaSocketClient lavaSocketClient)
        {
            _client = client;
            _lavaRestClient = lavaRestClient;
            _lavaSocketClient = lavaSocketClient;
        }

        public Task InitializeAsync()
        {
            _client.Ready += ClientReadyAsync;
            _lavaSocketClient.Log += LogAsync;
            _lavaSocketClient.OnTrackFinished += TrackFinished;
            return Task.CompletedTask;
        }

        public async Task ConnectAsync(SocketVoiceChannel voiceChannel, ITextChannel textChannel)
            => await _lavaSocketClient.ConnectAsync(voiceChannel, textChannel);

        public async Task LeaveAsync(SocketVoiceChannel voiceChannel)
            => await _lavaSocketClient.DisconnectAsync(voiceChannel);

        public async Task<string> PlayAsync(string query, ulong guildId)
        {
            _player = _lavaSocketClient.GetPlayer(guildId);
            var results = await _lavaRestClient.SearchYouTubeAsync(query);
            if (results.LoadType == LoadType.NoMatches || results.LoadType == LoadType.LoadFailed)
            {
                return "⚠️ No se han encontrado resultados.";
            }

            var track = results.Tracks.FirstOrDefault();

            if (_player.IsPlaying)
            {
                _player.Queue.Enqueue(track);
                return $"🔔 {track.Title} se agregó a la cola.";
            }
            else
            {
                await _player.PlayAsync(track);
                return $"🔔 Ahora está sonando: {track.Title}";
            }
        }

        public async Task StopAsync()
        {
            if (_player is null)
                return;
            await _player.StopAsync();
        }

        public async Task<string> SkipAsync()
        {
            if (_player is null || _player.Queue.Items.Count() is 0)
                return "⚠️ No hay canciones en cola.";

            var oldTrack = _player.CurrentTrack;
            await _player.SkipAsync();
            return $"🔔 Se ha saltado: {oldTrack.Title} \nEstás escuchando: {_player.CurrentTrack.Title}";
        }

        public async Task<string> SetVolumeAsync(int vol)
        {
            if (_player is null)
                return "⚠️ No se está reproduciendo ninguna canción.";

            if (vol > 150 || vol <= 2)
            {
                return "⚠️ Usa un número entre el 2 y el 150.";
            }

            await _player.SetVolumeAsync(vol);
            return $"🔔 El volumen se ha ajustado a: {vol}/150";
        }

        public async Task<string> PauseOrResumeAsync()
        {
            if (_player is null)
                return "⚠️ No se estaba reproduciendo nada.";

            if (!_player.IsPaused)
            {
                await _player.PauseAsync();
                return "🔔 b1g se ha pausado.";
            }
            else
            {
                await _player.ResumeAsync();
                return "🔔 b1g vuelve a reproducir música.";
            }
        }

        public async Task<string> ResumeAsync()
        {
            if (_player is null)
                return "⚠️ No se estaba reproduciendo nada.";

            if (_player.IsPaused)
            {
                await _player.ResumeAsync();
                return "🔔 b1g vuelve a reproducir música.";
            }

            return "⚠️ b1g no está pausado.";
        }


        private async Task ClientReadyAsync()
        {
            await _lavaSocketClient.StartAsync(_client, new Configuration
            {
                LogSeverity = LogSeverity.Info
            });
        }

        private async Task TrackFinished(LavaPlayer player, LavaTrack track, TrackEndReason reason)
        {
            if (!reason.ShouldPlayNext())
                return;

            if (!player.Queue.TryDequeue(out var item) || !(item is LavaTrack nextTrack))
            {
                await player.TextChannel.SendMessageAsync("⚠️ Ya no hay mas canciones en cola.");
                return;
            }

            await player.PlayAsync(nextTrack);
        }

        private Task LogAsync(LogMessage logMessage)
        {
            Console.WriteLine(logMessage.Message);
            return Task.CompletedTask;
        }
    }
}

﻿using Digimezzo.Utilities.Log;
using Dopamine.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dopamine.Core.Api.Lyrics
{
    public class LyricsFactory
    {
        private List<ILyricsApi> lyricsApis;

        public LyricsFactory(int timeoutSeconds, string providers, ILocalizationInfo info)
        {
            lyricsApis = new List<ILyricsApi>();

            if (providers.ToLower().Contains("chartlyrics")) lyricsApis.Add(new ChartLyricsApi(timeoutSeconds));
            if (providers.ToLower().Contains("lololyrics")) lyricsApis.Add(new LololyricsApi(timeoutSeconds));
            if (providers.ToLower().Contains("lyricwikia")) lyricsApis.Add(new LyricWikiaApi(timeoutSeconds));
            if (providers.ToLower().Contains("metrolyrics")) lyricsApis.Add(new MetroLyricsApi(timeoutSeconds));
            if (providers.ToLower().Contains("xiamilyrics")) lyricsApis.Add(new XiamiLyricsApi(timeoutSeconds, info));
            if (providers.ToLower().Contains("neteaselyrics")) lyricsApis.Add(new NeteaseLyricsApi(timeoutSeconds, info));
        }

        public async Task<Lyrics> GetLyricsAsync(string artist, string title)
        {
            Lyrics lyrics = null;
            ILyricsApi api = this.GetRandomApi();

            while (api != null && (lyrics == null || !lyrics.HasText))
            {
                try
                {
                    lyrics = new Lyrics(await api.GetLyricsAsync(artist, title), api.SourceName);
                }
                catch (Exception ex)
                {
                    LogClient.Error("Error while getting lyrics from '{0}'. Exception: {1}", api.SourceName, ex.Message);
                }

                api = this.GetRandomApi();
            }

            return lyrics;
        }

        private ILyricsApi GetRandomApi()
        {
            ILyricsApi api = null;

            if (lyricsApis.Count > 0)
            {
                var rnd = new Random();
                int index = rnd.Next(lyricsApis.Count);
                api = lyricsApis[index];
                lyricsApis.RemoveAt(index);
            }

            return api;
        }
    }
}

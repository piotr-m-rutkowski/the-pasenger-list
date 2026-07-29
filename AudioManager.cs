using Raylib_cs;
using System.Collections.Generic;


    public class AudioManager
    {
        // Dictionaries to store loaded audio assets
        private Dictionary<string, Music> _ambientTracks = new Dictionary<string, Music>();
        private Dictionary<string, Sound> _sounds = new Dictionary<string, Sound>();

        private Dictionary<string, Sound> _narrationSounds = new Dictionary<string, Sound>();
        private Sound _currentNarrationSound;
        private bool _hasActiveNarration = false;
        // Tracks which ambient sound is currently playing
        private string _currentMusicKey = "";

        public AudioManager()
        {

        }

        /// <summary>
        /// Loads a streaming audio file (MP3, OGG, WAV) for long ambient loops.
        /// </summary>
        public void LoadAmbient(string key, string filePath)
        {
            Music m = Raylib.LoadMusicStream(filePath);
            m.Looping = true;
            _ambientTracks[key] = m;
        }

public void StopNarration()
{
    if (Raylib.IsSoundPlaying(_currentNarrationSound))
    {
        Raylib.StopSound(_currentNarrationSound);
    }
}

        /// <summary>
        /// Loads a short sound effect (WAV, MP3) into memory for quick playback.
        /// </summary>
        public void LoadSound(string key, string filePath)
        {
            _sounds[key] = Raylib.LoadSound(filePath);
        }

        /// <summary>
        /// Plays an ambient loop by its key. If a different track is playing, it stops it first.
        /// </summary>
        public void PlayAmbient(string key, float volume = 0.5f)
        {
            if (!_ambientTracks.ContainsKey(key)) return;

            // If a different ambient track is playing, stop it first
            if (!string.IsNullOrEmpty(_currentMusicKey) && _currentMusicKey != key)
            {
                Raylib.StopMusicStream(_ambientTracks[_currentMusicKey]);
            }

            _currentMusicKey = key;
            
            // Only start playing if it isn't already running
            if (!Raylib.IsMusicStreamPlaying(_ambientTracks[key]))
            {
                Raylib.PlayMusicStream(_ambientTracks[key]);
            }

            Raylib.SetMusicVolume(_ambientTracks[key], volume);
        }

        /// <summary>
        /// Stops the currently active ambient track.
        /// </summary>
        public void StopAmbient()
        {
            if (!string.IsNullOrEmpty(_currentMusicKey) && _ambientTracks.ContainsKey(_currentMusicKey))
            {
                Raylib.StopMusicStream(_ambientTracks[_currentMusicKey]);
                _currentMusicKey = "";
            }
        }

        /// <summary>
        /// Plays a one-shot sound effect.
        /// </summary>
        public void PlaySFX(string key, float volume = 1.0f)
        {
            if (_sounds.ContainsKey(key))
            {
                Raylib.SetSoundVolume(_sounds[key], volume);
                Raylib.PlaySound(_sounds[key]);
            }
        }

        /// <summary>
        /// Keeps the active ambient music stream fed with audio data.
        /// MUST be called once per frame in your main update loop!
        /// </summary>
        public void Update()
        {
            if (!string.IsNullOrEmpty(_currentMusicKey) && _ambientTracks.ContainsKey(_currentMusicKey))
            {
                Raylib.UpdateMusicStream(_ambientTracks[_currentMusicKey]);
            }
        }

        /// <summary>
        /// Unloads all loaded audio assets from RAM and closes the audio device.
        /// Call this when shutting down the game.
        /// </summary>
        public void Cleanup()
        {
            foreach (var m in _ambientTracks.Values) 
            {
                Raylib.UnloadMusicStream(m);
            }
            
            foreach (var s in _sounds.Values) 
            {
                Raylib.UnloadSound(s);
            }

            Raylib.CloseAudioDevice();
        }
    }

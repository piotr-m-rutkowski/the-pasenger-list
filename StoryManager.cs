using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Raylib_cs;
using Newtonsoft.Json;


public class StoryManager
{
    public Image CurrentHitMap;
    private bool _hasMap;
    public Dictionary<int, Scene> StoryMap;
    public Scene CurrentScene;
    public int CurrentStep;
    public float FadeAmount { get; private set; }
    private float _fadeSpeed;
    
    public bool WaitingForChoice { get; private set; }
    
    // Engine State
    public int Sanity = 5;
    public string DisplayedText = "";
    public Texture2D[] CurrentBGFrames;
    public float ShakeIntensity = 0;

    // Audio Tracking
    private Music _bgMusic;
    private string _currentMusicPath = "";
    private Sound _currentNarration;
    private bool _isNarrationPlaying = false;    
    public Dictionary<int, List<Texture2D>> InteractionTextures = new();

    public StoryManager(string jsonPath, int startID, bool playAudioOnStart = false)
    {
        string json = File.ReadAllText(jsonPath);
        StoryMap = JsonConvert.DeserializeObject<Dictionary<int, Scene>>(json);
        LoadScene(startID, playAudioOnStart);
    }

    public void LoadScene(int id, bool playAudio = true)
{
    if (!StoryMap.ContainsKey(id)) return;

    // --- 1. Memory Cleanup ---
    
    // Backgrounds
    if (CurrentBGFrames != null)
    {
        foreach (var tex in CurrentBGFrames) Raylib.UnloadTexture(tex);
    }

    // Audio
    if (_isNarrationPlaying) 
    {
        Raylib.StopSound(_currentNarration);
        Raylib.UnloadSound(_currentNarration);
        _isNarrationPlaying = false;
    }

    // HitMap Cleanup (The fix for your crash)
    if (_hasMap) 
    {
        Raylib.UnloadImage(CurrentHitMap);
        CurrentHitMap = new Image(); // Reset to blank state immediately
        _hasMap = false; 
    }

    // Interaction Highlights
    foreach (var textureList in InteractionTextures.Values) 
    {
        foreach (var tex in textureList)
        {
            Raylib.UnloadTexture(tex);
        }
    }
    InteractionTextures.Clear();

    // --- 2. Scene Initialization ---
    
    CurrentStep = id;
    CurrentScene = StoryMap[id];
    DisplayedText = ""; 
    FadeAmount = 0.0f;
    _fadeSpeed = CurrentScene.FadeTime > 0 ? 1.0f / CurrentScene.FadeTime : 1.0f;

    // Load Backgrounds
    CurrentBGFrames = new Texture2D[CurrentScene.Backgrounds.Count];
    for (int i = 0; i < CurrentScene.Backgrounds.Count; i++) {
        CurrentBGFrames[i] = Raylib.LoadTexture("assets/images/" + CurrentScene.Backgrounds[i]);
        Raylib.SetTextureFilter(CurrentBGFrames[i], TextureFilter.Bilinear);
    }

    // --- 3. Audio Handling ---
    if (playAudio)
            {
                if (!string.IsNullOrEmpty(CurrentScene.NarrationFile)) 
                {
                    // Check both possible folders (assets/narration or assets/audio)
                    string narrationPath = Path.Combine("assets", "narration", CurrentScene.NarrationFile);
                    if (!File.Exists(narrationPath))
                    {
                        narrationPath = Path.Combine("assets", "audio", CurrentScene.NarrationFile);
                    }

                    if (File.Exists(narrationPath))
                    {
                        _currentNarration = Raylib.LoadSound(narrationPath);
                        Raylib.PlaySound(_currentNarration);
                        _isNarrationPlaying = true;
                    }
                    else
                    {
                        Console.WriteLine($"[Audio Warning] Narration file missing: {CurrentScene.NarrationFile}");
                    }
                }

                HandleMusic(CurrentScene.MusicFile);
            }

    // --- 3. Actions & Effects ---
    
if (CurrentScene.OnEnter != null) {
            foreach (var action in CurrentScene.OnEnter) {
                if (action.Type == "Shake") ShakeIntensity = action.Value;
                if (action.Type == "Sanity") Sanity += (int)action.Value;
                if (action.Type == "Sound" && playAudio) 
                {
                    Raylib.PlaySound(Raylib.LoadSound("assets/audio/" + action.Param));
                }
            }
        }

    // --- 4. Loading the NEW HitMap ---
    
    if (!string.IsNullOrEmpty(CurrentScene.HitMapFile))
    {
        CurrentHitMap = Raylib.LoadImage("assets/maps/" + CurrentScene.HitMapFile);
        
        // Safety: If file is missing, Raylib returns an image with data = null
        if (CurrentHitMap.Width > 0) 
        {
            _hasMap = true;
        }
        else 
        {
            Console.WriteLine($"[Warning] Hitmap file not found: assets/maps/{CurrentScene.HitMapFile}");
            _hasMap = false;
        }
    }

    // --- 5. Loading Highlights ---
    
if (CurrentScene.Interactions != null)
        {
            foreach (var act in CurrentScene.Interactions)
            {
                List<Texture2D> frames = new List<Texture2D>();
                
                if (act.HighlightFiles != null && act.HighlightFiles.Count > 0) 
                {
                    foreach (var file in act.HighlightFiles)
                    {
                        frames.Add(Raylib.LoadTexture("assets/highlights/" + file));
                    }
                }
                else if (!string.IsNullOrEmpty(act.HighlightFile))
                {
                    frames.Add(Raylib.LoadTexture("assets/highlights/" + act.HighlightFile));
                }

                InteractionTextures.Add(act.TargetID, frames);
            }
        }
    }





   /* CurrentScene = StoryMap[id];

    // --- ONLY LOAD NARRATION IF playAudio IS TRUE ---
    if (playAudio && !string.IsNullOrEmpty(CurrentScene.NarrationFile))
    {
        string audioPath = Path.Combine("assets", "audio", CurrentScene.NarrationFile);
        
        if (File.Exists(audioPath))
        {
            _currentNarration = Raylib.LoadSound(audioPath);
            Raylib.PlaySound(_currentNarration);
            _isNarrationPlaying = true;
        }
    }}
}*/

    private void HandleMusic(string path) {
        if (!string.IsNullOrEmpty(path) && path != _currentMusicPath) {
            if (_currentMusicPath != "") Raylib.UnloadMusicStream(_bgMusic);
            _currentMusicPath = path;
            _bgMusic = Raylib.LoadMusicStream("assets/audio/" + path);
            Raylib.PlayMusicStream(_bgMusic);
        }
    }

public void StopNarration()
{
    if (_isNarrationPlaying)
    {
        Raylib.StopSound(_currentNarration);
        Raylib.UnloadSound(_currentNarration);
        _isNarrationPlaying = false;
    }
}

public void PauseNarration()
{
    if (_isNarrationPlaying && Raylib.IsSoundPlaying(_currentNarration))
    {
        Raylib.PauseSound(_currentNarration);
    }
}

public void ResumeNarration()
{
    if (_isNarrationPlaying)
    {
        Raylib.ResumeSound(_currentNarration);
    }
}

    public void Update(float dt) {
        // Fade & Shake logic
        if (FadeAmount < 1.0f) FadeAmount = Math.Min(1.0f, FadeAmount + _fadeSpeed * dt);
        if (ShakeIntensity > 0) ShakeIntensity -= dt * 10.0f;

        // Music update
        if (_currentMusicPath != "") Raylib.UpdateMusicStream(_bgMusic);
    }

    public void Cleanup() {
        if (CurrentBGFrames != null)
            foreach (var tex in CurrentBGFrames) Raylib.UnloadTexture(tex);
        if (_currentMusicPath != "") Raylib.UnloadMusicStream(_bgMusic);
        if (_isNarrationPlaying) Raylib.UnloadSound(_currentNarration);
    }

public struct Particle
{
    public System.Numerics.Vector2 Position;
    public System.Numerics.Vector2 Velocity;
    public Raylib_cs.Color Color; // Explicitly tell it to use Raylib's Color
    public float Radius;
    public float LifeTime;
    public bool Active;
}

// Inside StoryManager.cs
public List<string> History = new List<string>();
private const int MaxHistory = 20;

public void AddToHistory(string line)
{
    if (string.IsNullOrEmpty(line)) return;
    
    History.Add(line);
    
    // Keep only the last 20 lines
    if (History.Count > MaxHistory)
    {
        History.RemoveAt(0);
    }
}

/*        public void HandleChoice(string choiceKey)
        {
            if (CurrentScene.Branches != null && CurrentScene.Branches.ContainsKey(choiceKey))
            {
                // Apply Sanity Impact
                if (CurrentScene.HealthImpact != null && CurrentScene.HealthImpact.ContainsKey(choiceKey))
                {
                    Sanity -= CurrentScene.HealthImpact[choiceKey];
                }

                LoadScene(CurrentScene.Branches[choiceKey]);
            }
        }
*/
}
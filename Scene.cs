using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

public class ColorInteraction
{
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public int TargetID { get; set; }
    public List<string> HighlightFiles { get; set; } = new List<string>();
    public string Name { get; set; }
    // Keep this for backward compatibility with single-image scenes
    public string HighlightFile { get; set; }
}
public class GameAction
{
    public string Type { get; set; }  // "Shake", "Sanity", "Sound", "SetFlag"
    public float Value { get; set; }
    public string Param { get; set; } 
}

public class Choice
{
    public string Text { get; set; }
    public int TargetID { get; set; }
}

public class Scene
{
    public string Text { get; set; }
    
    public string MusicFile { get; set; } 
    
    // Ensure these also exist as they are used in StoryManager:
    public string NarrationFile { get; set; }
    public List<string> Backgrounds { get; set; } = new List<string>();
    public string HitMapFile { get; set; } // e.g., "room_map.png"
    public List<ColorInteraction> Interactions { get; set; } = new();
    public int NextStep { get; set; } = -1;
    public List<Choice> Choices { get; set; } = new List<Choice>();
    public List<GameAction> OnEnter { get; set; }
    public float FadeTime { get; set; } = 1.0f;
    public string Particles { get; set; } = "None";
}




/*public class Scene
{
    public string Text { get; set; }
    public int NextStep { get; set; }
    public bool IsChoice { get; set; }
    public string MusicFile { get; set; }
    public List<string> BackgroundFrames { get; set; }
    
    public List<string> ChoiceLabels { get; set; } 

    public Dictionary<string, int> Branches { get; set; }
    public Dictionary<string, int> HealthImpact { get; set; }
    public float FadeTime { get; set; } = 1.0f; // Default to 1 second if not specified

    public float ShakeOnEntry { get; set; } = 0f;
}
*/
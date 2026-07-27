using NarrativeEngine;
using Raylib_cs;
using System;
using System.Numerics;
using System.Collections.Generic;
using Newtonsoft.Json;

class Program
{
    enum GameState { Menu, Playing, Credits }
static GameState currentGameState = GameState.Menu;
    static    bool showLog = false;
    static void Main()
    {
        // Settings
        int vW = 1500; int vH = 1000;
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
       //Raylib.SetConfigFlags(ConfigFlags.FullscreenMode);
        Raylib.InitWindow(vW, vH, "Gloomy Warmth Engine");
        Raylib.SetTargetFPS(60);
        Raylib.InitAudioDevice();

        Sound typeSound = Raylib.LoadSound("assets/audio/vscode1-typewriter2.wav");
        Raylib.SetSoundVolume(typeSound, 0.05f);
        
        RenderTexture2D canvas = Raylib.LoadRenderTexture(vW, vH);
        StoryManager story = new StoryManager("story.json", 0);
        int frames = 0;
        ParticleSystem dustMotes = new ParticleSystem(800);
        Raylib.SetExitKey(KeyboardKey.Null);
        bool keepRunning = true;

while (keepRunning && !Raylib.WindowShouldClose())
{
    float dt = Raylib.GetFrameTime();
    float scale = Math.Min((float)Raylib.GetScreenWidth() / vW, (float)Raylib.GetScreenHeight() / vH);
    Vector2 mouse = Raylib.GetMousePosition();
    Vector2 vMouse = (mouse - new Vector2(Raylib.GetScreenWidth() - vW * scale, Raylib.GetScreenHeight() - vH * scale) * 0.5f) / scale;
    int hoveredTarget = -1;
    // --- 1. GLOBAL INPUT (Always works) ---
    if (Raylib.IsKeyPressed(KeyboardKey.L)) {
        showLog = !showLog;
    }
    
if (Raylib.IsKeyPressed(KeyboardKey.Escape))
{
    if (currentGameState == GameState.Playing)
    {
        currentGameState = GameState.Menu;
    }
}
    
    if (Raylib.IsKeyPressed(KeyboardKey.F11)) { Raylib.ToggleFullscreen(); }

    // --- 2. UPDATE LOGIC BASED ON STATE ---
    switch (currentGameState)
    {
case GameState.Menu:
    Rectangle startButton = new Rectangle(vW / 2 - 150, vH / 2, 300, 80);
    Rectangle quitButton = new Rectangle(vW / 2 - 150, vH / 2 + 100, 300, 80);

    // Default cursor, will be overridden if hovering
    Raylib.SetMouseCursor(MouseCursor.Default);

    // --- START BUTTON LOGIC ---
    if (Raylib.CheckCollisionPointRec(vMouse, startButton))
    {
        Raylib.SetMouseCursor(MouseCursor.PointingHand);
        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            currentGameState = GameState.Playing;
        }
    } 
    // --- QUIT BUTTON LOGIC ---
    // Notice this is now its own block, NOT inside the startButton block!
    else if (Raylib.CheckCollisionPointRec(vMouse, quitButton))
    {
        Raylib.SetMouseCursor(MouseCursor.PointingHand);
        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            keepRunning = false; 
        }
    }
    break;

        case GameState.Playing:
            if (!showLog) 
            {
                frames++;
                story.Update(dt);
                dustMotes.Update(dt);

                if (story.CurrentScene != null) 
                    dustMotes.UpdateSceneParticles(story.CurrentScene.Particles);

                // --- HITMAP DETECTION ---
                //int hoveredTarget = -1;
                bool textFinished = story.CurrentScene != null && story.DisplayedText.Length >= story.CurrentScene.Text.Length;

                if (textFinished && story.CurrentScene != null && story.CurrentHitMap.Width > 0)
                {
                    int mapX = (int)vMouse.X;
                    int mapY = (int)vMouse.Y;

                    if (mapX >= 0 && mapX < story.CurrentHitMap.Width && mapY >= 0 && mapY < story.CurrentHitMap.Height)
                    {
                        Color pixelColor = Raylib.GetImageColor(story.CurrentHitMap, mapX, mapY);
                        if (story.CurrentScene.Interactions != null)
                        {
                            foreach (var interaction in story.CurrentScene.Interactions)
                            {
                                if (pixelColor.R == interaction.R && pixelColor.G == interaction.G && pixelColor.B == interaction.B)
                                {
                                    hoveredTarget = interaction.TargetID;
                                    Raylib.SetMouseCursor(MouseCursor.PointingHand);

                                    if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                                    {
                                        story.AddToHistory(story.CurrentScene.Text); 
                                        story.AddToHistory($"Interacted with: {interaction.Name}");
                                        story.LoadScene(interaction.TargetID);
                                    }
                                    break; 
                                }
                            }
                        }
                    }
                }
                if (hoveredTarget == -1) Raylib.SetMouseCursor(MouseCursor.Default);

                // --- TYPEWRITER & PROGRESSION ---
                if (story.CurrentScene != null && story.FadeAmount > 0.9)
                {
                    if (frames % 6 == 0 && story.DisplayedText.Length < story.CurrentScene.Text.Length)
                    {
                        story.DisplayedText = story.CurrentScene.Text.Substring(0, story.DisplayedText.Length + 1);
                        Raylib.SetSoundPitch(typeSound, (float)Raylib.GetRandomValue(65, 80) / 100.0f);
                        Raylib.PlaySound(typeSound);
                    }
                }

                if (Raylib.IsKeyPressed(KeyboardKey.Space) && story.FadeAmount >= 1.0f)
                {
                    if (story.DisplayedText.Length < story.CurrentScene.Text.Length)
                        story.DisplayedText = story.CurrentScene.Text;
                    else 
                    {
                        bool hasChoices = story.CurrentScene.Choices != null && story.CurrentScene.Choices.Count > 0;
                        bool hasInteractions = story.CurrentScene.Interactions != null && story.CurrentScene.Interactions.Count > 0;
                        if (!hasChoices && !hasInteractions)
                        {
                            story.AddToHistory(story.CurrentScene.Text); 
                            story.LoadScene(story.CurrentScene.NextStep);
                        }
                    }
                }
            }
            break;
    }

    // --- 3. DRAWING ---
    Raylib.BeginTextureMode(canvas);
    Raylib.ClearBackground(Color.Black);

    if (currentGameState == GameState.Playing)
    {
        // ... [Your existing drawing code for Background, Motes, Highlights, UI Box, and Choices] ...
        // (Copy-paste the inside of your 'if (story.CurrentScene != null)' block here)

    // 1. Draw Animated Background
    if (story.CurrentBGFrames != null && story.CurrentBGFrames.Length > 0)
    {
        // Cycles through bg_1, bg_2, etc.
        int bgFrame = (frames / 10) % story.CurrentBGFrames.Length;
        Raylib.DrawTexture(story.CurrentBGFrames[bgFrame], 0, 0, Raylib.Fade(Color.White, story.FadeAmount));
    }


    // 2. Draw Animated/Static Highlight
    if (hoveredTarget != -1 && story.InteractionTextures.ContainsKey(hoveredTarget))
    {
        var highlightFrames = story.InteractionTextures[hoveredTarget];

        if (highlightFrames != null && highlightFrames.Count > 0)
        {
            // Pick the highlight frame (this solves the CS1503 error)
            int hFrame = (frames / 10) % highlightFrames.Count;

            // Pulse effect
            float pulse = (float)Math.Sin(Raylib.GetTime() * 2.0f) * 0.05f + 0.95f;
            Color highlightColor = Raylib.Fade(Color.White, pulse * story.FadeAmount);

            // Draw ONLY the specific texture at the index [hFrame]
            Raylib.DrawTexture(highlightFrames[hFrame], 0, 0, highlightColor);
        }
    }

    dustMotes.Draw();
                    // Draw UI Text Box
                    Raylib.DrawRectangle(20, 700, 1460, 250, Raylib.Fade(Color.Black, 0.6f));
                    Raylib.DrawText(story.DisplayedText, 50, 730, 24, Color.White);

                    // Draw Standard Choice Buttons
                    if (story.DisplayedText.Length >= story.CurrentScene.Text.Length && story.CurrentScene.Choices != null)
                    {
                        for (int i = 0; i < story.CurrentScene.Choices.Count; i++)
                        {
                            var choice = story.CurrentScene.Choices[i];
                            Rectangle btnRect = new Rectangle(400, 300 + (i * 80), 700, 60);
                            bool hovering = Raylib.CheckCollisionPointRec(vMouse, btnRect);
                            
                            Raylib.DrawRectangleRec(btnRect, hovering ? Color.Gray : Raylib.Fade(Color.Black, 0.8f));
                            Raylib.DrawRectangleLinesEx(btnRect, 2, Color.White);
                            Raylib.DrawText(choice.Text, (int)btnRect.X + 20, (int)btnRect.Y + 15, 20, Color.White);

                            if (hovering && Raylib.IsMouseButtonPressed(MouseButton.Left))
                            
                                {   story.AddToHistory(story.CurrentScene.Text); 
                                    story.AddToHistory($"Selected: {choice.Text}");
                                    story.LoadScene(choice.TargetID);
                                }
                        }
                    }
        }
    
else if (currentGameState == GameState.Menu)
{
    Raylib.ClearBackground(new Color(15, 15, 20, 255)); // Deep gloomy blue/black

    // 1. Draw Title
    Raylib.DrawText("Darkness of Gunia", vW / 2 - 220, 300, 50, Color.White);

    // 2. Draw Start Button
    Rectangle startButton = new Rectangle(vW / 2 - 150, vH / 2, 300, 80);

    
    // Change color if hovering (we check the mouse again here for visual feedback)
    bool hovering = Raylib.CheckCollisionPointRec(vMouse, startButton);
    Raylib.DrawRectangleRec(startButton, hovering ? Color.Gray : Color.DarkGray);
    Raylib.DrawRectangleLinesEx(startButton, 3, Color.White);

    Rectangle quitButton = new Rectangle(vW / 2 - 150, vH / 2 + 100, 300, 80);
    bool hoveringQuit = Raylib.CheckCollisionPointRec(vMouse, quitButton);
    Raylib.DrawRectangleRec(quitButton, hoveringQuit ? Color.Maroon : Color.Red);
    Raylib.DrawText("QUIT", (int)quitButton.X + 100, (int)quitButton.Y + 25, 30, Color.White);

    // 3. Draw Button Text
    Raylib.DrawText("START", (int)startButton.X + 85, (int)startButton.Y + 25, 30, Color.White);
}
    Raylib.EndTextureMode();

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);

    if (showLog)
    {
        UIHandler.DrawHistoryLog(story, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
    }
    else
    {
        // Draw the canvas with your shake logic
        Vector2 shake = new Vector2(Raylib.GetRandomValue(-10, 10) * story.ShakeIntensity * 0.1f, Raylib.GetRandomValue(-10, 10) * story.ShakeIntensity * 0.1f);
        Rectangle dest = new Rectangle((Raylib.GetScreenWidth() - vW * scale) * 0.5f + shake.X, (Raylib.GetScreenHeight() - vH * scale) * 0.5f + shake.Y, vW * scale, vH * scale);
        Raylib.DrawTexturePro(canvas.Texture, new Rectangle(0, 0, vW, -vH), dest, Vector2.Zero, 0, Color.White);
    }
    Raylib.EndDrawing();

/*
while (!Raylib.WindowShouldClose())
        {
        
            float dt = Raylib.GetFrameTime();
         if (Raylib.IsKeyPressed(KeyboardKey.L)) 
{
    if (showLog == false) {
        showLog = true;
    } else {
        showLog = false;
    }
    Console.WriteLine($"[INPUT] Manual Toggle. showLog is now: {showLog}");
}
            frames++;
            story.Update(dt);

            // Input & Scaling
            float scale = Math.Min((float)Raylib.GetScreenWidth() / vW, (float)Raylib.GetScreenHeight() / vH);
            Vector2 mouse = Raylib.GetMousePosition();
            Vector2 vMouse = (mouse - new Vector2(Raylib.GetScreenWidth() - vW * scale, Raylib.GetScreenHeight() - vH * scale) * 0.5f) / scale;

dustMotes.Update(dt); // Moves existing particles
    if (story.CurrentScene != null) 
    {
        // Spawns new particles based on scene type
        dustMotes.UpdateSceneParticles(story.CurrentScene.Particles);
    }

    // -------------------------------

// 2. Color ID Hit-Map Detection
int hoveredTarget = -1;

// NEW CONDITION: Only allow interaction if text is fully displayed
bool textFinished = story.CurrentScene != null && 
                    story.DisplayedText.Length >= story.CurrentScene.Text.Length;

if (textFinished && story.CurrentScene != null && story.CurrentHitMap.Width > 0)
{
    int mapX = (int)vMouse.X;
    int mapY = (int)vMouse.Y;

    if (mapX >= 0 && mapX < story.CurrentHitMap.Width && mapY >= 0 && mapY < story.CurrentHitMap.Height)
    {
        // This is the line that requires the 'unsafe' setting
        Color pixelColor = Raylib.GetImageColor(story.CurrentHitMap, mapX, mapY);

        if (story.CurrentScene.Interactions != null)
        {
            foreach (var interaction in story.CurrentScene.Interactions)
            {
                if (pixelColor.R == interaction.R && 
                    pixelColor.G == interaction.G && 
                    pixelColor.B == interaction.B)
                {
                    hoveredTarget = interaction.TargetID;
                    Raylib.SetMouseCursor(MouseCursor.PointingHand);

                    if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                    {  // story.AddToHistory(story.CurrentScene.Text); 
                        story.AddToHistory(story.CurrentScene.Text); 
                        story.AddToHistory($"Interacted with: {interaction.Name}");
                        story.LoadScene(interaction.TargetID);
                    }
                    break; 
                }
            }
        }
    }
}

// Ensure cursor resets if text isn't finished or nothing is hovered
if (hoveredTarget == -1) Raylib.SetMouseCursor(MouseCursor.Default);

if (!showLog) 
{
// Typewriter logic         // Only start typing once the scene has started fading in
 if (story.CurrentScene != null && story.FadeAmount > 0.9)
{
    // frames % 4 controls the speed (higher = slower)
    if (frames % 6 == 0 && story.DisplayedText.Length < story.CurrentScene.Text.Length)
    {
        story.DisplayedText = story.CurrentScene.Text.Substring(0, story.DisplayedText.Length + 1);// Trigger your typewriter sound here
        float pitch = (float)Raylib.GetRandomValue(65, 80) / 100.0f;
        Raylib.SetSoundPitch(typeSound, pitch);
        Raylib.PlaySound(typeSound);       // Console.WriteLine($"Current Length: {story.DisplayedText.Length}");
    }
}
// Keyboard Progression
if (Raylib.IsKeyPressed(KeyboardKey.Space) && story.FadeAmount >= 1.0f)
{
    if (story.DisplayedText.Length < story.CurrentScene.Text.Length)
    {
        story.DisplayedText = story.CurrentScene.Text;
    }
    else 
    {
        bool hasChoices = story.CurrentScene.Choices != null && story.CurrentScene.Choices.Count > 0;
        bool hasInteractions = story.CurrentScene.Interactions != null && story.CurrentScene.Interactions.Count > 0;

        if (!hasChoices && !hasInteractions)
        {
            // Linear scene progression
            story.AddToHistory(story.CurrentScene.Text); 
            story.LoadScene(story.CurrentScene.NextStep);
        }
    }
}
 /*   if (Raylib.IsKeyPressed(KeyboardKey.Space) && story.FadeAmount >= 1.0f)
    {
                if (story.DisplayedText.Length < story.CurrentScene.Text.Length)
                    story.DisplayedText = story.CurrentScene.Text;
                else if (story.CurrentScene.Choices == null || story.CurrentScene.Choices.Count == 0)
                    story.AddToHistory(story.CurrentScene.Text); // this updates the log
                    story.LoadScene(story.CurrentScene.NextStep);
    }
}

// DRAWING
    Raylib.BeginTextureMode(canvas);
    Raylib.ClearBackground(Color.Black);

if (story.CurrentScene != null)
{
    // 1. Draw Animated Background
    if (story.CurrentBGFrames != null && story.CurrentBGFrames.Length > 0)
    {
        // Cycles through bg_1, bg_2, etc.
        int bgFrame = (frames / 10) % story.CurrentBGFrames.Length;
        Raylib.DrawTexture(story.CurrentBGFrames[bgFrame], 0, 0, Raylib.Fade(Color.White, story.FadeAmount));
    }
    dustMotes.Draw();

    // 2. Draw Animated/Static Highlight
    if (hoveredTarget != -1 && story.InteractionTextures.ContainsKey(hoveredTarget))
    {
        var highlightFrames = story.InteractionTextures[hoveredTarget];

        if (highlightFrames != null && highlightFrames.Count > 0)
        {
            // Pick the highlight frame (this solves the CS1503 error)
            int hFrame = (frames / 10) % highlightFrames.Count;

            // Pulse effect
            float pulse = (float)Math.Sin(Raylib.GetTime() * 6.0f) * 0.95f + 0.75f;
            Color highlightColor = Raylib.Fade(Color.White, pulse * story.FadeAmount);

            // Draw ONLY the specific texture at the index [hFrame]
            Raylib.DrawTexture(highlightFrames[hFrame], 0, 0, highlightColor);
        }
    }


                    // Draw UI Text Box
                    Raylib.DrawRectangle(20, 700, 1460, 250, Raylib.Fade(Color.Black, 0.6f));
                    Raylib.DrawText(story.DisplayedText, 50, 730, 24, Color.White);

                    // Draw Standard Choice Buttons
                    if (story.DisplayedText.Length >= story.CurrentScene.Text.Length && story.CurrentScene.Choices != null)
                    {
                        for (int i = 0; i < story.CurrentScene.Choices.Count; i++)
                        {
                            var choice = story.CurrentScene.Choices[i];
                            Rectangle btnRect = new Rectangle(400, 300 + (i * 80), 700, 60);
                            bool hovering = Raylib.CheckCollisionPointRec(vMouse, btnRect);
                            
                            Raylib.DrawRectangleRec(btnRect, hovering ? Color.Gray : Raylib.Fade(Color.Black, 0.8f));
                            Raylib.DrawRectangleLinesEx(btnRect, 2, Color.White);
                            Raylib.DrawText(choice.Text, (int)btnRect.X + 20, (int)btnRect.Y + 15, 20, Color.White);

                            if (hovering && Raylib.IsMouseButtonPressed(MouseButton.Left))
                            
                                {   story.AddToHistory(story.CurrentScene.Text); 
                                    story.AddToHistory($"Selected: {choice.Text}");
                                    story.LoadScene(choice.TargetID);
                                }
                        }
                    }
                }
////////////

/// ////////////
                
            Raylib.EndTextureMode();
Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);

    if (showLog)
    {
        // 1. Draw the History Overlay from your UIHandler file
        // We pass the raw Screen Width/Height so it fills the whole window
        UIHandler.DrawHistoryLog(story, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
    }
    else
    {
        // 2. Draw the Game Canvas (Your existing code)
        
        // Calculate Screen Shake
        Vector2 shake = new Vector2(
            Raylib.GetRandomValue(-10, 10) * story.ShakeIntensity * 0.1f, 
            Raylib.GetRandomValue(-10, 10) * story.ShakeIntensity * 0.1f
        );

        // Calculate where the canvas fits on the screen
        Rectangle dest = new Rectangle(
            (Raylib.GetScreenWidth() - vW * scale) * 0.5f + shake.X, 
            (Raylib.GetScreenHeight() - vH * scale) * 0.5f + shake.Y, 
            vW * scale, 
            vH * scale
        );

        // Render the canvas texture to the window
        Raylib.DrawTexturePro(canvas.Texture, new Rectangle(0, 0, vW, -vH), dest, Vector2.Zero, 0, Color.White);
    }
Raylib.EndDrawing();
*/

// Fullscreen toggle remains outside the Drawing block
if (Raylib.IsKeyPressed(KeyboardKey.F11)) { Raylib.ToggleFullscreen(); }

        } // ============= END OF WHILE LOOP
        //Raylib.UnloadMusicStream(ambientRain);
        Raylib.CloseAudioDevice();
        Raylib.UnloadSound(typeSound);
        Raylib.UnloadRenderTexture(canvas);
        Raylib.CloseAudioDevice();
        Raylib.CloseWindow();
        
    }
}
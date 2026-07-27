using System;
using Raylib_cs;
using System.Numerics;

namespace NarrativeEngine
{
    public static class UIHandler
    {
        public static void DrawHistoryLog(StoryManager story, int vW, int vH)
        {
            // 1. Semi-transparent overlay (full screen)
            Raylib.DrawRectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight(), Raylib.Fade(Color.Black, 0.9f));
            
            // 2. Title
            Raylib.DrawText("DIALOGUE HISTORY", 50, 40, 30, Color.Gold);
            Raylib.DrawText("Press 'L' to return to game", 50, 80, 18, Color.Gray);
            Raylib.DrawLine(50, 110, vW - 50, 110, Color.DarkGray);

            // 3. Draw the lines
            // We draw them from newest (bottom) to oldest (top)
            int startY = 140;
            int spacing = 35;

            for (int i = 0; i < story.History.Count; i++)
            {
                Color textColor = Color.White;
                // Make older lines slightly more transparent/faded
                float alpha = (float)(i + 1) / story.History.Count;
                
                Raylib.DrawText($"> {story.History[i]}", 60, startY + (i * spacing), 20, Raylib.Fade(textColor, alpha));
            }
            
            if (story.History.Count == 0)
            {
                Raylib.DrawText("No recorded dialogue yet.", vW / 2 - 100, vH / 2, 20, Color.DarkGray);
            }
        }
    }
}
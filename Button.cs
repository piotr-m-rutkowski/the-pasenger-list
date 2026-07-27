using Raylib_cs;
using System.Numerics; // Sometimes Vector2 lives here depending on your setup

namespace NarrativeEngine 
{
    public struct Button
    {
        public Rectangle Bounds;
        public string Text;

        public Button(float x, float y, float width, float height, string text)
        {
            Bounds = new Rectangle(x, y, width, height);
            Text = text;
        }

        // Returns true only on the frame the mouse is clicked inside the box
        public bool IsClicked(Vector2 mousePoint)
        {
            return Raylib.CheckCollisionPointRec(mousePoint, Bounds) && 
                   Raylib.IsMouseButtonPressed(MouseButton.Left);
        }

        // Useful for changing the cursor or button color!
        public bool IsHovered(Vector2 mousePoint)
        {
            return Raylib.CheckCollisionPointRec(mousePoint, Bounds);
        }
    }
}
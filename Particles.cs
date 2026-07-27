using System;
using System.Numerics;
using Raylib_cs;

namespace NarrativeEngine 
{
    public struct Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Color Color;
        public float Radius;
        public float LifeTime;
        public bool Active;
        public bool IsRain; // New: To distinguish drawing style
    }

    public class ParticleSystem
    {
        private Particle[] _particles;
        private Random _rng = new Random();

        public ParticleSystem(int maxParticles)
        {
            _particles = new Particle[maxParticles];
        }

        public void UpdateSceneParticles(string type)
        {
            if (string.IsNullOrEmpty(type) || type == "None") return;

            switch (type)
            {
case "Rain":
    for (int i = 0; i < 6; i++)
    {
        Vector2 pos = new Vector2(_rng.Next(-500, 2000), -100);
        Vector2 vel = new Vector2(2.0f, 15.0f); // Fast speed

        // Randomize the "tone" (0 = more blue, 100 = more grey)
        int tone = _rng.Next(100, 180); 
        
        // R and G stay close to each other for greyness
        // Blue (B) stays slightly higher to keep that "rain" tint
        byte r = (byte)tone;
        byte g = (byte)(tone + _rng.Next(0, 10));
        byte b = (byte)(tone + _rng.Next(20, 40)); 
        byte a = (byte)_rng.Next(100, 180); // Randomize transparency too!

        Color rainColor = new Color(r, g, b, a);
        
        Emit(pos, rainColor, 1, vel, true);
    }
    break;
    //More Grey: byte b = (byte)(tone + 10);
    //More Blue: byte b = (byte)(tone + 50);

case "fRain":
    for (int i = 0; i < 6; i++)
    {
        Vector2 pos = new Vector2(_rng.Next(-500, 2000), -100);
        Vector2 vel = new Vector2(8.0f, 35.0f); // Fast speed

        // Randomize the "tone" (0 = more blue, 100 = more grey)
        int tone = _rng.Next(100, 180); 
        
        // R and G stay close to each other for greyness
        // Blue (B) stays slightly higher to keep that "rain" tint
        byte r = (byte)tone;
        byte g = (byte)(tone + _rng.Next(0, 10));
        byte b = (byte)(tone + _rng.Next(20, 40)); 
        byte a = (byte)_rng.Next(100, 180); // Randomize transparency too!

        Color rainColor = new Color(r, g, b, a);
        
        Emit(pos, rainColor, 1, vel, true);
    }
    break;

                case "Snow":
                    if (_rng.Next(0, 100) > 85) {
                        Emit(new Vector2(_rng.Next(0, 1920), -10), Color.White, 1, new Vector2(0, 1.5f));
                    }
                    break;

                case "Sparks":
                    if (_rng.Next(0, 100) > 90) {
                        Emit(new Vector2(1100, 850), Color.Gold, 2, new Vector2(_rng.Next(-20, 20) / 10f, -4.0f));
                    }
                    break;

                case "Dust":
                    if (_rng.Next(0, 100) > 98) {
                        Vector2 pos = new Vector2(_rng.Next(0, 1920), _rng.Next(0, 1080));
                        Vector2 drift = new Vector2(_rng.Next(-10, 10) / 20f, _rng.Next(-10, 10) / 20f);
                        Emit(pos, Raylib.Fade(Color.Gray, 0.4f), 1, drift);
                    }
                    break;
            }
        }

        // Updated Emit to include the isRain flag
        public void Emit(Vector2 position, Color color, int count, Vector2 velocity, bool isRain = false)
        {
            for (int i = 0; i < count; i++)
            {
                int index = FindAvailableIndex();
                if (index == -1) break;

                _particles[index] = new Particle {
                    Position = position,
                    // Add slight random variation to velocity
                    Velocity = velocity + new Vector2((float)_rng.NextDouble() * 0.5f, (float)_rng.NextDouble() * 2f),
                    Color = color,
                    Radius = isRain ? 1.0f : (float)_rng.NextDouble() * 2 + 1,
                    LifeTime = isRain ? 1.5f : (float)_rng.NextDouble() * 3 + 2,
                    Active = true,
                    IsRain = isRain
                };
            }
        }

        public void Update(float dt)
        {
            for (int i = 0; i < _particles.Length; i++)
            {
                if (!_particles[i].Active) continue;
                _particles[i].LifeTime -= dt;
                if (_particles[i].LifeTime <= 0) { _particles[i].Active = false; continue; }
                
                // For rain, we use a constant speed multiplication for that "fast" look
                _particles[i].Position += _particles[i].Velocity;
            }
        }

        public void Draw()
        {
            for (int i = 0; i < _particles.Length; i++)
            {
                if (!_particles[i].Active) continue;
                
                if (_particles[i].IsRain)
                {
                    // Draw a line streak for rain
                    // The end point is the position + velocity (creating the stretch)
                    Vector2 endLine = _particles[i].Position + (_particles[i].Velocity * 1.5f);
                    Raylib.DrawLineEx(_particles[i].Position, endLine, 1.5f, _particles[i].Color);
                }
                else
                {
                    // Draw a circle for dust/snow/sparks
                    Color c = Raylib.Fade(_particles[i].Color, _particles[i].LifeTime);
                    Raylib.DrawCircleV(_particles[i].Position, _particles[i].Radius, c);
                }
            }
        }

        private int FindAvailableIndex()
        {
            for (int i = 0; i < _particles.Length; i++)
                if (!_particles[i].Active) return i;
            return -1;
        }
    }
}
using System.Collections.Generic;
using LudumDare49.OpenGL;
using OpenTK.Mathematics;

namespace LudumDare49
{
    public static class Text
    {
        private static Texture _texture = new Texture("font.png");
        private static Sprite _sprite = new Sprite(_texture);

        private static readonly Vector2 TextSize = new Vector2(39, 63);
        private static readonly Dictionary<char, Vector4> CharToTexCoord = new();

        static Text()
        {
            string characters = "abcdefghijklmnopqrstuvwxyz0123456789";
            for (int i = 0; i < characters.Length; i++)
            {
                CharToTexCoord.Add(characters[i],
                    new Vector4(i * TextSize.X, i * TextSize.Y, (i + 1) * TextSize.X, (i + 1) * TextSize.Y));
            }
        }

        public static void Render(string text, Vector2 position, float size)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ')
                {
                    continue;
                }
                
                _sprite.Render(CharToTexCoord[text[i]], size * TextSize, position + Vector2.UnitX * TextSize * size * i);
            }
        }
    }
}
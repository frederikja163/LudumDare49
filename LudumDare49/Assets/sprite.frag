#version 330 core
in vec2 fTexCoord;

uniform sampler2D uTexture;

out vec4 Color;

void main()
{
//    Color = vec4(fTexCoord, 0, 1);
    Color = texture(uTexture, fTexCoord);
}
#version 330 core
in vec3 fNorm;
in vec2 fTexCoord;

out vec4 Color;

void main()
{
    Color = vec4(fNorm, 1);
}
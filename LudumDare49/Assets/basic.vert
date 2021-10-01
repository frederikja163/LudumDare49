#version 330 core
in vec2 vPos;
in vec2 vTexCoord;

out vec2 fTexCoord;

void main()
{
    gl_Position = vec4(vPos, 0, 1);
    
    fTexCoord = vec2(1, -1) * vTexCoord;
}
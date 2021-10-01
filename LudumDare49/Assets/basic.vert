#version 330 core
in vec2 vPos;

void main()
{
    gl_Position = vec4(vPos, 0, 1);
}
#version 330 core
layout(location = 0) in vec3 vPos;
layout(location = 1) in vec3 vNorm;
layout(location = 2) in vec2 vTexCoord;

out vec3 fNorm;
out vec2 fTexCoord;

void main()
{
    gl_Position = vec4(vPos, 1);
    
    fNorm = vNorm;
    fTexCoord = vTexCoord;
}
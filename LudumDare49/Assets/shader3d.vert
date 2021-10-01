#version 330 core
layout(location = 0) in vec3 vPos;
layout(location = 1) in vec3 vNorm;
layout(location = 2) in vec2 vTexCoord;

uniform mat4 uProjection;
uniform mat4 uView;

out vec3 fNorm;
out vec2 fTexCoord;

void main()
{
    gl_Position = uProjection * uView * vec4(vPos, 1);
    
    fNorm = vNorm;
    fTexCoord = vTexCoord;
}
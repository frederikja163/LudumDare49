#version 330 core
layout(location = 0) in vec3 vPos;
layout(location = 1) in vec3 vNorm;
layout(location = 2) in vec2 vTexCoord;
layout(location = 3) in float vMaterialIndex;

uniform mat4 uProjection;
uniform mat4 uView;
uniform mat4 uModel;

out vec3 fPos;
out vec3 fNorm;
out vec2 fTexCoord;
flat out int fMaterialIndex;

void main()
{
    gl_Position = uProjection * uView * uModel * vec4(vPos, 1);
    
    fPos = vec3(uModel * vec4(vPos, 1));
    fNorm = mat3(transpose(inverse(uModel))) * vNorm;
    fTexCoord = vTexCoord;
    fMaterialIndex = int(vMaterialIndex);
}
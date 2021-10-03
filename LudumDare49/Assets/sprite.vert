#version 330 core
uniform vec4 uTexCoord;
uniform vec2 uPosition;
uniform vec2 uSize;
uniform mat4 uView;
uniform mat4 uPerspective;

in vec2 vPos;

out vec2 fTexCoord;

void main()
{
    gl_Position = uPerspective * uView * vec4((vPos + vec2(0.5, -0.5)) * uSize + uPosition, 0, 1);
    
    fTexCoord = -vPos * (uTexCoord.zw - uTexCoord.xy) + uTexCoord.xy;
}
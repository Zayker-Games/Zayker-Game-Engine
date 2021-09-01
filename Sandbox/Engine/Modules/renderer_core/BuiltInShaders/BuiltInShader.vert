#version 330 core //Using version GLSL version 3.3
layout (location = 0) in vec4 vPos;

uniform mat4 MVP;

void main()
{
    gl_Position = MVP * vec4(vPos.x, vPos.y, vPos.z, 1.0);
}
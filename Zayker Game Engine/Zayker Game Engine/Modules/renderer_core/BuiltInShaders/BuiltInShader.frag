#version 330 core
in vec2 fUv;

out vec4 FragColor;

void main()
{
    FragColor = vec4(fUv.x, fUv.y, 1f, 1f);
}
#version 330 core
out vec4 FragColor;
void main()
{
    FragColor = vec4(gl_FragCoord.x/500, gl_FragCoord.y/500, 0.0f, 1.0f);
}
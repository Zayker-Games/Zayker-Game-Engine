#version 330 core
in vec2 fUv;

uniform sampler2D uTexture0;

out vec4 FragColor;

void main()
{
    FragColor = texture(uTexture0, fUv);
    // Alpha-Clipping below a == 0.1
    if(FragColor.w < 0.1)
        discard;
}
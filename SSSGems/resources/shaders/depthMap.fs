#version 330 core
out vec4 FragColor;

in float distance;

uniform sampler2D texture1;

void main()
{ 
    gl_FragColor = vec4(distance);
}
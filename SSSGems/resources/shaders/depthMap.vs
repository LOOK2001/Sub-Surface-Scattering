#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 normal;

uniform mat4 view;
uniform mat4 projection;
uniform mat4 model;

out float distance;

void main()
{
    gl_Position = projection * view * vec4(aPos + normal*0.003, 1.0);
	distance = length(view * vec4(aPos, 1.0));
}
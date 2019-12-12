#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

out vec2 TexCoords;
out vec3 Normal;
out vec3 lightDir;
out vec3 vertexCameraNormal;
out vec3 camDir;
out vec4 vertexLightCoord;
out float vertexLightDist;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

uniform mat4 light_projection;
uniform mat4 light_view;
uniform vec3 light_direction;
uniform vec3 light_position;

void main()
{
    TexCoords = aTexCoords;    
    gl_Position = projection * view * vec4(aPos, 1.0);
	camDir = -vec3(view * vec4(aPos, 1.0));
	Normal = aNormal;
	vertexCameraNormal = normalize(mat3(view)*aNormal);
	vertexLightCoord = light_projection * light_view * vec4(light_position, 1.0);
	vertexLightDist = length(light_view * vec4(aPos, 1.0));
	lightDir = -light_direction;
}
#version 330 core
out vec4 FragColor;

in vec2 TexCoords;
out vec3 vertexNormal;
in vec3 lightDir;
in vec3 vertexCameraNormal;
in vec3 camDir;
in vec4 vertexLightCoord;
in float vertexLightDist;

uniform sampler2D texture1;

vec3 lightColor = vec3(1.0f, 1.0f, 1.0f);

void main()
{    
	float bias = 0.0005;

	float hitShadowDist = texture(texture1, vertexLightCoord.xy).x;
	float scatDistance = vertexLightDist - hitShadowDist;
	float scatPower = exp(-scatDistance * 10);

	vec4 albedo = vec4(0.7);

	vec3 N = normalize(vertexNormal);
	vec3 L = normalize(lightDir);
	vec3 V = normalize(camDir);
	vec3 R = reflect(L, N);

	FragColor = vec4(scatPower);

    //FragColor = texture(texture1, TexCoords);
	//FragColor = vec4(1.0, 1.0, 1.0, 1.0);
}
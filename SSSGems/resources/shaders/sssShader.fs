#version 330 core
out vec4 FragColor;

in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
    vec4 FragPosLightSpace;
} fs_in;

uniform sampler2D depthMap;

uniform vec3 lightPos;
uniform vec3 viewPos;

float DepthCalculation(vec4 fragPosLightSpace)
{
    // 1. I need to do perspective divide for vertex position
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    // transform to [0,1] range because it is [0,1] range in depth map
    projCoords = projCoords * 0.5 + 0.5;
    // get closest depth value from light's perspective
    float closestDepth = texture(depthMap, projCoords.xy).r;
	// get depth of current fragment depth from light's perspective
    float currentDepth = projCoords.z;
	// calculate shadow bias to solve the shadow acne problem
    vec3 normal = normalize(fs_in.Normal);
    vec3 lightDir = normalize(lightPos - fs_in.FragPos);
    float bias = max(0.05 * (1.0 - dot(normal, lightDir)), 0.005);
	// use PCF method to sample more than once from depth map, which can produce softer shadows
    float thickness = 0.0;
    vec2 texelSize = 1.0 / textureSize(depthMap, 0);
    for(int x = -1; x <= 1; ++x)
    {
        for(int y = -1; y <= 1; ++y)
        {
            float pcfDepth = texture(depthMap, projCoords.xy + vec2(x, y) * texelSize).r; 
            thickness += currentDepth - bias > pcfDepth  ? 1.0 : 0.0;        
        }    
    }
    thickness /= 9.0;
    
    // keep the thickness at 0.0 when outside the far_plane region of the light's frustum.
    if(projCoords.z > 1.0)
        thickness = 0.0;

    thickness /= 9.0;
        
    return thickness;
}

float saturate(float val) {
    return clamp(val, 0.0, 1.0);
}

vec4 SubsurfaceScat(float thickness) {
	float distortion = 0.2f;
	float power = 10.0f;
	float scale = 5.0f;
	vec3 specularColor = vec3(0.2, 0.7, 0.2);
	vec3 viewDir = normalize(viewPos - fs_in.FragPos);
	vec3 lightDir  = normalize(lightPos - fs_in.FragPos);
	float ambient = 0.3;
    vec3 LTLight = normalize(lightDir + (fs_in.Normal * distortion));
    float LTDot = pow(saturate(dot(viewDir, -LTLight)), power) * scale;
	float lt = (LTDot + ambient) * thickness;
    return vec4(specularColor * lt, 1.0);
}

void main()
{           
    vec3 normal = normalize(fs_in.Normal);
    vec3 lightColor = vec3(0.3);
	vec3 diffuseColor = vec3(0.1, 0.8, 0.1);
	vec3 specularColor = vec3(0.1, 0.9, 0.1);
    // ambient
    vec3 ambient = 0.3 * lightColor;
    // diffuse
    vec3 lightDir = normalize(lightPos - fs_in.FragPos);
    float diff = max(dot(lightDir, normal), 0.0);
    vec3 diffuse = diff * diffuseColor;
    // specular
    vec3 viewDir = normalize(viewPos - fs_in.FragPos);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = 0.0;
    vec3 halfwayDir = normalize(lightDir + viewDir);  
    spec = pow(max(dot(normal, halfwayDir), 0.0), 64.0);
    vec3 specular = spec * specularColor;
	
	// Subsurface scattering
	// d_i, distance from light at entry point
	float d_i = DepthCalculation(fs_in.FragPosLightSpace);
	// d_o, distance from light to current fragment(exit point)
	float d_o = length(fs_in.FragPosLightSpace);
	// compute the depth
	float scatDistance = d_o - d_i;

	vec4 sss = SubsurfaceScat(scatDistance);

	vec3 result = (ambient + (diffuse + specular));
    
    FragColor = vec4(result, 1.0) + sss;
}
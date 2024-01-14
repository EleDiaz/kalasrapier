#version 450
in vec2 TexCoord;
in vec4 VertexNormal;
out vec4 OutFragColor;

uniform sampler2D uTexture;
uniform vec3 diffuse_color;
uniform vec3 light_direction;
uniform vec3 light_color;
uniform vec3 ambient;

void main()
{
    float cl = max(dot(light_direction,VertexNormal.xyz),0.0);

    vec4 tSample = min(texture(uTexture, TexCoord), vec4(diffuse_color.rgb, 1.0));
    // vec4 newcolor = vec4(ambient * diffuse_color * tSample + cl * light_color.rgb * diffuse_color.rgb, 1.0);
    vec4 newcolor = vec4(ambient,1.0)*tSample+cl*vec4(light_color.rgb,1.0)*tSample;
    OutFragColor = newcolor;
}


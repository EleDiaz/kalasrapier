#version 450 core 
in vec2 TexCoord;
in vec4 VertexNormal;
out vec4 OutFragColor;

uniform sampler2D uTexture;
uniform vec3 AmbientLight;
uniform vec3 DirLight0Diffuse;
uniform vec3 DirLight0Direction;


void main()
{
    float cl = max(dot(DirLight0Direction, VertexNormal.xyz), 0.0);

    vec4 tSample = texture(uTexture, TexCoord);
    vec4 newcolor = vec4(AmbientLight, 1.0)*tSample+cl*vec4(DirLight0Diffuse, 1.0)*tSample;

    OutFragColor = newcolor;
    //OutFragColor=vec4(tSample.rgb,1.0);
}


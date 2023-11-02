#version 450 core
layout(location=0) in vec3 aPosition;
layout(location=1) in float aWeight;

uniform mat4 view;
uniform mat4 model;
uniform mat4 projection;

uniform vec3 diffuse_color;

out float VertexWeight ;
out vec4 DiffuseColor;

void main(){
    vec4 position=vec4(aPosition,1.0f) * model * view * projection;
    VertexWeight=aWeight;
    DiffuseColor=vec4(diffuse_color,1.0);
    gl_Position=position;
}

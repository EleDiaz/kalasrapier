#version 450 core
layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;
layout(location = 2) in vec3 aNormal;

uniform mat4 view;
uniform mat4 model;
uniform mat4 projection;

uniform mat4 normal_transform_matrix;

out vec4 VertexNormal;
out vec2 TexCoord;

void main() {
    vec4 position = vec4(aPosition, 1.0) * model * view * projection;

    VertexNormal = vec4(aNormal, 1.0) * normal_transform_matrix;
    TexCoord = aTexCoord;
    gl_Position = position;
}

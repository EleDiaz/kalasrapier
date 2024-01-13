#version 450 core
layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 uv;
layout(location = 2) in vec3 normals;

uniform mat4 view;
uniform mat4 model;
uniform mat4 projection;

uniform vec3 diffuse_color;

out vec4 DiffuseColor;

void main() {
    vec4 position = vec4(aPosition, 1.0) * model * view * projection;
    DiffuseColor = vec4(diffuse_color, 1.0);
    gl_Position = position;
}

#version 460 core
layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec4 aColor;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

flat out vec4 VertexColor;

void main(void)
{
    vec4 position = vec4(aPosition, 1.0) * model * view * projection;
    VertexColor = aColor;
    gl_Position = position;
}

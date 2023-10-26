#version 460

flat in vec4 VertexColor;
out vec4 OutFragColor;

void main()
{
    OutFragColor = VertexColor;
}

#version 450

in float VertexWeight;
in vec4 DiffuseColor;
out vec4 OutFragColor;

void main()
{
    OutFragColor = DiffuseColor;
}


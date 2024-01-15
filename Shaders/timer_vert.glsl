#version 450


void main() {
    vec3 position = vec3(0.0);
    
    position.x = floor(gl_VertexID / 2.0) / 10 - 1;
    position.y = mod(gl_VertexID, 2.0) / 10 -1;
    
    
    gl_Position = vec4(position, 1.0);

}

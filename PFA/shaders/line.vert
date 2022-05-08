#version 330 core

out vec4 vertexColor;

layout (location = 0) in vec2 aPos;
uniform vec4 colour;

void main()
{
    gl_Position = vec4(aPos.x, aPos.y, 0.0, 1.0);
    vertexColor = colour;
}

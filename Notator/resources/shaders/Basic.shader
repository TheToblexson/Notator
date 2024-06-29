#shader vertex
#version 330 core
layout (location = 0) in vec3 aPosition;

uniform mat4 uMVP;

void main()
{
    gl_Position = uMVP * vec4(aPosition.x, aPosition.y, aPosition.z, 1.0);
}

#shader fragment
#version 330 core
layout (location = 0) out vec4 oColor;

uniform vec4 uColor;

void main()
{
    oColor = uColor;
}

#shader vertex
#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec4 aColor;
layout (location = 2) in vec2 aTexCoord;
layout (location = 3) in float aTexIndex;

uniform mat4 uMVP;

out vec4 vColor;
out vec2 vTexCoord;
out float vTexIndex;

void main()
{
    vColor = aColor;
    vTexCoord = aTexCoord;
    vTexIndex = aTexIndex;
    gl_Position = uMVP * vec4(aPosition.x, aPosition.y, aPosition.z, 1.0);
}

#shader fragment
#version 330 core
layout (location = 0) out vec4 oColor;

in vec4 vColor;
in vec2 vTexCoord;
in float vTexIndex;

uniform sampler2D uTextures[8];

void main()
{
    int index = int(vTexIndex);
    if (index == -1)
        oColor = vColor;
    else
        oColor = texture(uTextures[index], vTexCoord);
}


#shader vertex

#version 330 core
    layout (location = 0) in vec3 aPos;
    layout (location = 1) in vec4 aColour;
    layout (location = 2) in vec2 aTexCoords;
    layout (location = 3) in float aTexIndex;

    uniform mat4 uMVP; 

    out vec2 vTexCoords;
    out vec4 vColour;
    out float vTexIndex;

    void main()
    {
        gl_Position = uMVP * vec4(aPos, 1.0);
        vTexCoords = aTexCoords;
        vColour = aColour;
        vTexIndex = aTexIndex;
    };

#shader fragment
    #version 330 core

    in vec2 vTexCoords;
    in vec4 vColour;
    in float vTexIndex;

    uniform sampler2D uTextures[2];

    out vec4 oColour;

    void main()
    {
        int index = int(vTexIndex);
        oColour = texture(uTextures[index], vTexCoords) * vColour;
    }
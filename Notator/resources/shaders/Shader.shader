
#shader vertex

#version 330 core
    layout (location = 0) in vec3 aPos;
    layout (location = 1) in vec2 aTexCoords;

    out vec2 vTexCoords;

    void main()
    {
        gl_Position = vec4(aPos, 1.0);
        vTexCoords = aTexCoords;
    };

#shader fragment
    #version 330 core

    in vec2 vTexCoords;

    uniform sampler2D uTexture;

    out vec4 oColour;

    void main()
    {
        oColour = texture(uTexture, vTexCoords);
    }
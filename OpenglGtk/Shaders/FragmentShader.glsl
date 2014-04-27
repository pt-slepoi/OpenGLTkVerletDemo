uniform vec4 color;
uniform mat4 mvp;
varying float depth;
void main(void){

    gl_FragColor = color;
    //float z = (mvp * vec4(0,0,depth,1.0)).z/255.0;
    float z = depth/25.0;
    gl_FragColor = vec4(z,z,z,1.0);
    //gl_FragColor = vec4(color.x*depth,color.x*depth,color.x*depth,1.0);
}
uniform vec4 color;
varying float depth;
void main(void){

    gl_FragColor = color;
    //float z = depth/200.0;
    //gl_FragColor = vec4(z,z,z,1.0);
    //gl_FragColor = vec4(color.x*depth,color.x*depth,color.x*depth,1.0);
}
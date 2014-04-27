uniform vec4 color;
uniform int useColor;
uniform mat4 mvp;
varying float depth;
uniform float distance;
void main(void){

    gl_FragColor = color;
    float z = 1.0-depth/distance;
    if(useColor==1)
    	gl_FragColor = vec4(color.x*z,color.y*z,color.z*z,color.w*1.0);
    else
    	gl_FragColor = vec4(z,z,z,1.0);
  }
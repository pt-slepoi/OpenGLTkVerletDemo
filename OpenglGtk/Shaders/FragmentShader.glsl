uniform vec4 color;
uniform int useColor;
varying float zEye;

float f_start = 5.0;
float f_end = 50.0;

float fogAtZ(float z){
	return clamp( (z-f_start)/(f_end-f_start), 0.0, 1.0);
}

void main(void){
	float f = 1.0 - fogAtZ(zEye);
	if(useColor == 1){
		gl_FragColor  = color*f;
  		
	}else{
		gl_FragColor = vec4(1.0,1.0,1.0,1.0)*f;
	}
	gl_FragColor.w = 1.0;
	
	/*if(gl_FrontFacing)
		gl_FragColor = gl_FragColor * 0.50;
	  */ 
  }
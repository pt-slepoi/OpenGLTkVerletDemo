uniform mat4 mvp; 
varying float depth;
void main(void){
	
    gl_Position = mvp * gl_Vertex;
	depth = gl_Position.z; 

}
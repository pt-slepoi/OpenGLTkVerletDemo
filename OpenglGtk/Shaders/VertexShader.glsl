uniform mat4 mvp; 
uniform mat4 mv; 
varying float zEye;
void main(void){
	gl_Position = mvp * gl_Vertex;
    
    zEye =  -(mv * gl_Vertex).z;
    
    
	
    
    

}
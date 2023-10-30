import bpy
import os

dir_path=os.path.dirname(os.path.dirname(os.path.realpath(__file__)))
#new_path=os.path.dirname(os.path.join(dir_path,'..\..'))

new_path=os.path.join(dir_path,'mesh.json')

print(new_path)

F=open(new_path,"w")

sobjects=bpy.context.selected_objects;

object=sobjects[0];

object_data=object.data;

vertices=object_data.vertices;
F.write('[\n{\n');
F.write('"nvertex" : ');
F.write('%d,\n' % (len(vertices)));

F.write('"vertexdata" : [\n');
for vert in vertices[:-1]:
    coord=vert.co;
    F.write('%f,%f,%f,\n' % (coord[0],coord[2],-coord[1]))
coord=vertices[-1].co;
F.write('%f,%f,%f\n' % (coord[0],coord[2],-coord[1]))
F.write('],\n');

F.write('"colordata" : [\n');
for vert in vertices[:-1]:
    
    F.write('%f,%f,%f,%f,\n' % (1.0,0.0,0.0,1.0));
F.write('%f,%f,%f,%f\n' % (1.0,0.0,0.0,1.0));
F.write('],\n');


polygons=object_data.polygons;

nindices=len(polygons)*3;
F.write('"nindex" : ');
F.write('%d,\n' % nindices);

F.write('"indexdata" : [\n');
for polygon in polygons[:-1]:
    verts=polygon.vertices;
    verts=[verts[0],verts[1],verts[2]];
    for vert in verts:
        F.write('%d,\n' %(vert));
verts=polygons[-1].vertices
verts=[verts[0],verts[1],verts[2]];
for vert in verts[:-1]:
    F.write('%d,\n' %(vert));
F.write('%d\n' % (verts[-1]))
F.write(']\n');
F.write('}\n]\n');
F.close();
print("Finalizada la escritura de los datos");
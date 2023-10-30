import bpy
import os
import json

sobjects = bpy.context.selected_objects

object = sobjects[0]

object_data = object.data

vertices = object_data.vertices

mesh_data = {}

mesh_data["nvertex"] = len(vertices)
operated_vertices = []
for vert in vertices:
    coord = vert.co
    operated_vertices.append(coord[0])
    operated_vertices.append(coord[2])
    operated_vertices.append(-coord[1])

mesh_data["vertexdata"] = operated_vertices
mesh_data["colordata"] = [1.0, 0.0, 0.0, 1.0] * len(vertices)

polygons = object_data.polygons

nindices = len(polygons) * 3
mesh_data["nindex"] = nindices

operated_indices = []
for polygon in polygons:
    verts = polygon.vertices
    operated_indices.append(verts[0])
    operated_indices.append(verts[2])
    operated_indices.append(verts[1])

mesh_data["indexdata"] = operated_indices


dir_path = os.path.dirname(os.path.dirname(os.path.realpath(__file__)))
# new_path=os.path.dirname(os.path.join(dir_path,'..\..'))

new_path = os.path.join(dir_path, "mesh.json")

print(new_path)

with open(new_path, "w") as file:
    json.dump([mesh_data], file)

print("Finalizada la escritura de los datos")

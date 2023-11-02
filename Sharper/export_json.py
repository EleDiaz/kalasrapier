import bpy
import os
import json

def extract_mesh_data(obj):
    mesh_data = {}

    vertices = obj.data.vertices
    # mesh_data["nvertex"] = len(vertices)
    operated_vertices = []
    operated_normals = []
    for vert in vertices:
        coord = vert.co
        operated_vertices.append(coord[0])
        operated_vertices.append(coord[2])
        operated_vertices.append(-coord[1])
        normal = vert.normal
        operated_normals.append(normal[0])
        operated_normals.append(normal[2])
        operated_normals.append(-normal[1])

    mesh_data["vertexdata"] = operated_vertices
    mesh_data["normaldata"] = operated_normals

    mesh_data["colordata"] = [1.0, 0.0, 0.0, 1.0] * len(vertices)

    polygons = obj.data.polygons
    # nindices = len(polygons) * 3
    # mesh_data["nindex"] = nindices

    operated_indices = []
    for polygon in polygons:
        verts = polygon.vertices
        operated_indices.append(verts[0])
        operated_indices.append(verts[2])
        operated_indices.append(verts[1])

    mesh_data["indexdata"] = operated_indices

    operate_uvs = [0.0, 0.0] * len(vertices)
    ob_loops = obj.data.loops
    uv_layer = obj.data.uv_layers.active.data
    # loop are the polygons that conform the mesh. This imply some duplicated iterations
    # but was the only way to obtain the vertex_index
    for loop in ob_loops:
        vi = loop.vertex_index * 2
        uv = uv_layer[loop.index].uv
        operate_uvs[vi] = uv[0]
        operate_uvs[vi + 1] = uv[1]

    mesh_data["uvs"] = operate_uvs
    return mesh_data




sobjects = bpy.context.selected_objects

meshes = []
for obj in sobjects:
    meshes.append(extract_mesh_data(obj))

dir_path = os.path.dirname(os.path.dirname(os.path.realpath(__file__)))
# new_path=os.path.dirname(os.path.join(dir_path,'..\..'))

new_path = os.path.join(dir_path, "mesh.json")

print(new_path)

with open(new_path, "w") as file:
    json.dump(meshes, file)

print("Finalizada la escritura de los datos")

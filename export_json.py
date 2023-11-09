import bpy
import os
import json


def extract_mesh_data(obj):
    mesh_data = {}

    ### VERTICES & NORMALS
    vertices = obj.data.vertices
    operated_vertices = []
    operated_normals = []
    for vert in vertices:
        coord = vert.co
        operated_vertices += [coord[0], coord[2], -coord[1]]
        normal = vert.normal
        operated_normals += [normal[0], normal[2], -normal[1]]

    mesh_data["vertexdata"] = operated_vertices
    mesh_data["normaldata"] = operated_normals

    mesh_data["colordata"] = [1.0, 0.0, 0.0, 1.0] * len(vertices)
    mesh_data["weightdata"] = [1.0] * len(vertices)

    ### MATERIALS
    default_material = {"name": "default", "diffuse_color": [1.0, 1.0, 1.0, 1.0]}

    mesh_data["materials"] = [default_material]
    material_slots = obj.material_slots
    for ms in material_slots:
        # possible texture reference???
        mat = {
            "name": ms.material.name,
            "diffuse_color": [v for v in ms.material.diffuse_color],
        }
        mesh_data["materials"].append(mat)

    ### INDECES (and materials associated)
    mesh_data["indexdata"] = [[]] * (len(mesh_data["materials"]) - 1)

    polygons = obj.data.polygons
    for polygon in polygons:
        verts = polygon.vertices
        slot = 0
        if len(mesh_data["materials"]) == 1: # No materials at all
            slot = 0
        else:
            slot = polygon.material_index # Skip the default material
        # TODO: review if it's necessary to keep changing the vertices ccw?
        mesh_data["indexdata"][slot] += [verts[0], verts[2], verts[1]]

    ### UV (TODO: maybe we need to see how this works with several materials)
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

# meshes = []
# for obj in sobjects:
#     meshes.append(extract_mesh_data(obj))

dir_path = os.path.dirname(os.path.dirname(os.path.realpath(__file__)))
# new_path=os.path.dirname(os.path.join(dir_path,'..\..'))

new_path = os.path.join(dir_path, "mesh.json")

print(new_path)

with open(new_path, "w") as file:
    json.dump(extract_mesh_data(sobjects[0]), file, indent=4)

print("Finalizada la escritura de los datos")

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

    mesh_data["vertex_data"] = operated_vertices
    mesh_data["normal_data"] = operated_normals

    mesh_data["color_data"] = [1.0, 0.0, 0.0, 1.0] * len(vertices)
    mesh_data["weight_data"] = [1.0] * len(vertices)

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
    mesh_data["index_data"] = [[] for _ in range(0, len(mesh_data["materials"]) - 1)]

    polygons = obj.data.polygons
    for polygon in polygons:
        verts = polygon.vertices
        slot = polygon.material_index
        if len(mesh_data["materials"]) == 1:  # No materials at all
            slot = 0
        else:
            slot = polygon.material_index  # Skip the default material
        mesh_data["index_data"][slot] += [verts[0], verts[1], verts[2]]
    
    
    # The slots indicates the index range that the material is associate
    mesh_data["index_slots"] = [{"offset": len(slot), "start": 0} for slot in mesh_data["index_data"]]
    acc = 0
    for slot in mesh_data["index_slots"]:
        slot["start"] = acc
        acc += slot["offset"]

    # Flatten the index data
    mesh_data["index_data"] = [index for slots in mesh_data["index_data"] for index in slots]


    ### UV (TODO: maybe we need to see how this works with several materials)
    operate_uvs = []
    ob_loops = obj.data.loops
    uv_layer = obj.data.uv_layers.active.data

    for polygon in polygons:
        loops = polygon.loop_indices
        for loopindex in loops:
            loop = ob_loops[loopindex]
            uvloop = uv_layer[loopindex]
            operate_uvs += [uvloop.uv[0], uvloop.uv[1]]

    mesh_data["uv_data"] = operate_uvs
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

using System;
using System.Collections.Generic;
using System.Linq;
using Obi;
using Parabox.CSG;
using UnityEngine;

public class CompositeObject : MonoBehaviour
{
    public GameObject left;
    
    /// <summary>
    /// Use only right if you want to subtsruct single part
    /// </summary>
    public GameObject right;
    
    /// <summary>
    /// Use only rightMultipleObjects if you want to subtsruct multiple parts
    /// </summary>
    public GameObject[] rightMultipleObjects;

    public Action OnFinished;

    void Start()
    {
        Initialize();
    }
    
    public void Initialize()
    {
        if (left == null)
            return;
        
        if (right != null)
        {
            Substruct(left, right);
        }

        if (rightMultipleObjects.Length > 0)
        {
            Substruct(left, rightMultipleObjects);
        }

        OnFinished?.Invoke();
        return;
    }

    private GameObject Substruct(GameObject lg, GameObject rg)
    {
        var leftDuplicate = GameObject.Instantiate(lg);
        var rightDuplicate = GameObject.Instantiate(rg);
        
        // create new GameObject with location and rotation of left
        var go = new GameObject();
        
        var goNewPos = go.transform.InverseTransformPoint(lg.transform.position);
        var goNewRot = go.transform.InverseTransformVector(lg.transform.eulerAngles);
        leftDuplicate.transform.position = goNewPos;
        leftDuplicate.transform.rotation = Quaternion.Euler(goNewRot);
        rightDuplicate.transform.position = go.transform.InverseTransformPoint(rg.transform.position);
        rightDuplicate.transform.rotation = Quaternion.Euler(go.transform.InverseTransformVector(rg.transform.eulerAngles));
        
        var result = CSG.Subtract(leftDuplicate, rightDuplicate);
            
        go.transform.rotation = Quaternion.Euler(goNewRot);
        go.transform.position = goNewPos;
        
        // translate vertices, normals, tangents relatively to the new GameObject
        var mesh = new Mesh();
        mesh.vertices = TransformVertices(result.mesh.vertices, go.transform);
        mesh.triangles = result.mesh.triangles;
        mesh.normals = TransformVectors(result.mesh.normals, go.transform);
        mesh.colors = result.mesh.colors;
        mesh.uv = result.mesh.uv;
        mesh.subMeshCount = result.mesh.subMeshCount;
        mesh.tangents = TransformDirections(result.mesh.tangents,go.transform);

        lg.GetComponent<MeshFilter>().sharedMesh = result.mesh;
        var newMeshCollider = lg.AddComponent<MeshCollider>();
        newMeshCollider.sharedMesh = result.mesh;
        var obiCollider = lg.GetComponent<ObiCollider>();
        obiCollider.sourceCollider = newMeshCollider;
        lg.GetComponent<MeshRenderer>().sharedMaterials = result.materials.ToArray();

        Destroy(go);

        // cleaning
        if (!Application.isPlaying)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall+=()=>
            {
                DestroyImmediate(leftDuplicate);
                DestroyImmediate(rightDuplicate);
            };
            #endif
        }
        else
        {
            Destroy(leftDuplicate);
            Destroy(rightDuplicate);
        }
        
        rg.SetActive(false);

        return go;
    }
    
    /// <summary>
    /// Substruct all right gameobjects from left
    /// </summary>
    /// <param name="lg"></param>
    /// <param name="rg"></param>
    /// <returns></returns>
       private GameObject Substruct(GameObject lg, GameObject[] rg)
    {
        var leftDuplicate = GameObject.Instantiate(lg);
        GameObject[] rightDuplicates = rg.Select(x => Instantiate(x)).ToArray();

        // create new GameObject with location and rotation of left
        var go = new GameObject();
        
        var goNewPos = go.transform.InverseTransformPoint(lg.transform.position);
        var goNewRot = go.transform.InverseTransformVector(lg.transform.eulerAngles);
        leftDuplicate.transform.position = goNewPos;
        leftDuplicate.transform.rotation = Quaternion.Euler(goNewRot);

        for (int i = 0; i < rg.Length; i++)
        {
            rightDuplicates[i].transform.position = go.transform.InverseTransformPoint(rg[i].transform.position);
            rightDuplicates[i].transform.rotation = Quaternion.Euler(go.transform.InverseTransformVector(rg[i].transform.eulerAngles));
        }

        var result = CSG.Subtract(leftDuplicate, rightDuplicates);
            
        go.transform.rotation = Quaternion.Euler(goNewRot);
        go.transform.position = goNewPos;
        
        // translate vertices, normals, tangents relatively to the new GameObject
        var mesh = new Mesh();
        mesh.vertices = TransformVertices(result.mesh.vertices, go.transform);
        mesh.triangles = result.mesh.triangles;
        mesh.normals = TransformVectors(result.mesh.normals, go.transform);
        mesh.colors = result.mesh.colors;
        mesh.uv = result.mesh.uv;
        mesh.subMeshCount = result.mesh.subMeshCount;
        mesh.tangents = TransformDirections(result.mesh.tangents,go.transform);

        
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        go.AddComponent<MeshRenderer>().sharedMaterials = result.materials.ToArray();
        
        go.transform.parent = transform;
        go.tag = "composite";

        // cleaning
        if (!Application.isPlaying)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall+=()=>
            {
                DestroyImmediate(leftDuplicate);
                for (int i = 0; i < rg.Length; i++)
                {
                    DestroyImmediate(rightDuplicates[i]);
                }
              
            };
#endif
        }
        else
        {
            Destroy(leftDuplicate);
            for (int i = 0; i < rg.Length; i++)
            {
                DestroyImmediate(rightDuplicates[i]);
            }
        }
        
     
        lg.SetActive(false);
        for (int i = 0; i < rg.Length; i++)
        {
           rightDuplicates[i].SetActive(false);
        }

        return go;
    }
    
    private Vector3[] TransformVertices(Vector3[] vertices, Transform target)
    {
        var newVertices = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            newVertices[i] = target.InverseTransformPoint(vertices[i]);
        }
    
        return newVertices;
    }
    
    private Vector3[] TransformVectors(Vector3[] vertices, Transform target)
    {
        var newVertices = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            newVertices[i] = target.InverseTransformVector(vertices[i]);
        }
    
        return newVertices;
    }
    
    private Vector3[] TransformDirections(Vector3[] vertices, Transform target)
    {
        var newVertices = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            newVertices[i] = target.InverseTransformDirection(vertices[i]);
        }
    
        return newVertices;
    }
    
    private Vector4[] TransformDirections(Vector4[] vertices, Transform target)
    {
        var newVertices = new Vector4[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
           var vector3 = target.InverseTransformDirection(vertices[i]);
           newVertices[i] = new Vector4(vector3.x, vector3.y, vector3.z, vertices[i].w);
        }
    
        return newVertices;
    }
}
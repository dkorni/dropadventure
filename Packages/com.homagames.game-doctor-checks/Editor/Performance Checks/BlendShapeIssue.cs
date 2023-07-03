using System.Collections.Generic;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class BlendShapeIssue : StepBasedIssue
    {
        public BlendShapeIssue(ModelImporter importer)
        {
            Mesh m = AssetDatabase.LoadAssetAtPath<Mesh>(importer.assetPath);
            Name = $"{m.name} is importing blendshapes.";
            Description = $"You should consider removing blendshapes from the Mesh {m.name}";
            Priority = Priority.Low;
            AutomationType = AutomationType.Interactive;
            stepsList = new List<Step>()
            {
                new Step(() => !importer.importBlendShapes, $"Consider disabling blendshape for {m.name}",
                    (issue, step) =>
                    {
                        importer.importBlendShapes =
                            EditorGUILayout.Toggle("Import Blendshapes", importer.importBlendShapes);
                        EditorGUILayout.TextField(
                            "Or you can dismiss this issue if you actually need blenshapes on this asset.",
                            EditorStyles.wordWrappedLabel);
                    })
            };
        }
    }
}
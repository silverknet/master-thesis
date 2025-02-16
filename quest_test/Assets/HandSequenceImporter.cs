#if UNITY_EDITOR

using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor.AssetImporters;

[ScriptedImporter(1, new string[] { "hsec" })]
public class HandSequenceImporter : ScriptedImporter {
    

    public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx) {
        using (var file = new FileStream(ctx.assetPath, FileMode.Open, FileAccess.Read))
        using (var reader = new StreamReader(file)) {
            float fileLength = file.Length;

            HandSequence handSequence = ScriptableObject.CreateInstance<HandSequence>();

            while (true)
            {
                string line = reader.ReadLine(); // Read a new line
                if (line == null) break; // stop at end of file
                
                var element = line.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(float.Parse).ToArray();
                
                
                for (int i = 0; i < element.Length; i += 3)
                {
                    
                }

            }

        }
    }

}
#endif

using System;
using System.IO;

namespace StoryMode.Services;

public class ImageImporter
{
    public string ImportImage(string imagePath)
    {
        // todo: no hard-coded strings!
        if (ProjectManager.Instance.CurrentWorkspace is null) throw new InvalidOperationException("No project loaded!"); 
        
        var assetsDir = Path.Combine(ProjectManager.Instance.CurrentWorkspace, "assets");
        Directory.CreateDirectory(assetsDir);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imagePath)}";
        var destPath = Path.Combine(assetsDir, fileName);
        
        File.Copy(imagePath,  destPath);
        
        return Path.Combine("assets", fileName);
    }
}
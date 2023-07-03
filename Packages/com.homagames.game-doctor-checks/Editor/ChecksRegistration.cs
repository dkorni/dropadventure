using HomaGames.GameDoctor.Core;
using UnityEditor;

namespace HomaGames.GameDoctor.Checks
{
    /// <summary>
    /// Class in charge of registering all default checks
    /// </summary>
    public static class ChecksRegistration
    {
        [InitializeOnLoadMethod]
        private static void RegisterDefaultChecks()
        {
            AvailableChecks.RegisterCheck(new MissingScriptsCheck());
            AvailableChecks.RegisterCheck(new PlayerSettingsCheck());
            AvailableChecks.RegisterCheck(new BuildSettingsCheck());
            AvailableChecks.RegisterCheck(new GraphicsSettingsCheck());
            AvailableChecks.RegisterCheck(new AstcCompressionCheck());
            AvailableChecks.RegisterCheck(new TextureSizeCheck());
            AvailableChecks.RegisterCheck(new TextureReadableCheck());
            AvailableChecks.RegisterCheck(new MipmapsCheck());
            AvailableChecks.RegisterCheck(new SkinnedMeshCompressionCheck());
            AvailableChecks.RegisterCheck(new MeshReadWriteCheck());
            AvailableChecks.RegisterCheck(new MeshSizeCheck());
            AvailableChecks.RegisterCheck(new BlendShapeCheck());
        }
    }
}
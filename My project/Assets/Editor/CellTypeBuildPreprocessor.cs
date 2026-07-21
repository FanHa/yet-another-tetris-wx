using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Editor.Validation
{
    public sealed class CellTypeBuildPreprocessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!CellTypeRegistryStartupValidator.ValidateAllAndGetResult())
            {
                throw new BuildFailedException("Cell type validation failed. Fix validation errors before building.");
            }
        }
    }
}
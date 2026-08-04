using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Editor.Validation;

namespace Editor.Validation
{
    public sealed class CellTypeBuildPreprocessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!CellTypeRegistryValidator.ValidateAllAndGetResult())
            {
                throw new BuildFailedException("Cell type validation failed. Fix validation errors before building.");
            }
        }
    }
}
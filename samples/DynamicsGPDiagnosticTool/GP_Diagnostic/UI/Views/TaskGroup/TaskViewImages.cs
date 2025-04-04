namespace Microsoft.GP.MigrationDiagnostic.UI.Views.TaskGroup;

using System.Drawing;

internal static class TaskViewImages
{
    public static readonly Image completedImage, failedImage, notStartedImage;

    static TaskViewImages()
    {
        // Load image resources
        var resizedNotStartedImage = new Bitmap(Resources.greydash, new Size() { Height = 32, Width = 32 });
        notStartedImage = resizedNotStartedImage;

        var resizedCompletedImage = new Bitmap(Resources.Checkmark_grey_64x, new Size() { Height = 32, Width = 32 });
        completedImage = resizedCompletedImage;

        var resizedFailedImage = new Bitmap(Resources.X_red_64x, new Size() { Height = 32, Width = 32 });
        failedImage = resizedFailedImage;
    }
}

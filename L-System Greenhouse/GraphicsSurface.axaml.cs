using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace L_System_Greenhouse;

public partial class GraphicsSurface : UserControl
{
    private Bitmap? _bitmap;

    public GraphicsSurface()
    {
        InitializeComponent();
    }

    public sealed override void Render(DrawingContext context)
    {
        base.Render(context);

        if (_bitmap != null)
        {
            // Draw bitmap visibly
            context.DrawImage(_bitmap, new Rect(0, 0, _bitmap.PixelSize.Width, _bitmap.PixelSize.Height));
        }
    }

    public void Draw(Bitmap? bitmap)
    {
        _bitmap = bitmap;
        
        InvalidateVisual();
    }
}
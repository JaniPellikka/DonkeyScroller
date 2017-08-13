using System.Windows.Forms;

namespace JaniPellikka.Windows.Forms
{
    public class DonkeyScroller : Control
    {
        public DonkeyScroller()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint|
                ControlStyles.UserPaint|
                ControlStyles.ResizeRedraw|
                ControlStyles.OptimizedDoubleBuffer, 
                true
                );
            

        }
    }
}

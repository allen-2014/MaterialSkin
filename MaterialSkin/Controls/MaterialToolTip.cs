using MaterialSkin.Animations;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.Windows.Forms;

namespace MaterialSkin.Controls
{
    public class MaterialToolTip : ToolTip, IMaterialControl
    {
        //Properties for managing the material design properties
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }

        internal AnimationManager AnimationManager;

        //internal Point AnimationSource;


        private SizeF toopTipSize;

        private Padding margin;

        public MaterialToolTip()
        {
            AnimationManager = new AnimationManager(false)
            {
                Increment = 0.07,
                AnimationType = AnimationType.EaseInOut
            };
            //AnimationManager.OnAnimationProgress += sender => Invalidate();
            //AnimationManager.OnAnimationFinished += sender => OnItemClicked(_delayesArgs);

            BackColor = SkinManager.BackdropColor;
            ForeColor = SkinManager.TextHighEmphasisColor;

            this.ShowAlways = true;
            this.OwnerDraw = true;

            this.IsBalloon = false;
            this.UseFading = true;
            this.UseAnimation = true;

            margin = new Padding(4, 6, 4, 6);


            this.Draw += MaterialToolTip_Draw;
            this.Popup += MaterialToolTip_Popup;
        }

        public new void SetToolTip(Control control, string caption)
        {
            ComputeToolTipSize(control, caption);
            base.SetToolTip(control, caption);
        }

        //
        // Summary:
        //     Sets the ToolTip text associated with the specified control, and displays the
        //     ToolTip modally.
        //
        // Parameters:
        //   text:
        //     A System.String containing the new ToolTip text.
        //
        //   window:
        //     The System.Windows.Forms.Control to display the ToolTip for.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The window parameter is null.
        public new void Show(string text, IWin32Window window)
        {
            ComputeToolTipSize(window as Control, text);
            //base.Show(text, window);
            Control control = (window as Control);
    
          
            Rectangle rect = control.ClientRectangle;
            Point point = ComputeStartLocation(rect, MaterialToolTipPlacement.BottomCenter);
            

            base.Show(text, window, point.X, point.Y);
        }

        public void Show(string text, IWin32Window window, MaterialToolTipPlacement placement)
        {
            ComputeToolTipSize(window as Control, text);
            //base.Show(text, window);
            Control control = (window as Control);

            Rectangle rect = control.ClientRectangle;
            Point point = ComputeStartLocation(rect, placement);
  
            base.Show(text, window, point.X, point.Y);
        }
        //
        // Summary:
        //     Sets the ToolTip text associated with the specified control, and then displays
        //     the ToolTip for the specified duration.
        //
        // Parameters:
        //   text:
        //     A System.String containing the new ToolTip text.
        //
        //   window:
        //     The System.Windows.Forms.Control to display the ToolTip for.
        //
        //   duration:
        //     An System.Int32 containing the duration, in milliseconds, to display the ToolTip.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The window parameter is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     duration is less than or equal to 0.
        public new void Show(string text, IWin32Window window, int duration)
        {
            ComputeToolTipSize(window as Control, text);
            //base.Show(text, window, duration);
            Control control = (window as Control);
            Rectangle rect = control.ClientRectangle;
            Point point = ComputeStartLocation(rect, MaterialToolTipPlacement.BottomCenter);
            base.Show(text, window, point, duration);
        }

        public void Show(string text, IWin32Window window, int duration, MaterialToolTipPlacement placement)
        {
            ComputeToolTipSize(window as Control, text);
            //base.Show(text, window, duration);
            Control control = (window as Control);
            Rectangle rect = control.ClientRectangle;
            Point point = ComputeStartLocation(rect, placement);
            base.Show(text, window, point, duration);
        }
        //
        // Summary:
        //     Sets the ToolTip text associated with the specified control, and then displays
        //     the ToolTip for the specified duration at the specified relative position.
        //
        // Parameters:
        //   text:
        //     A System.String containing the new ToolTip text.
        //
        //   window:
        //     The System.Windows.Forms.Control to display the ToolTip for.
        //
        //   point:
        //     A System.Drawing.Point containing the offset, in pixels, relative to the upper-left
        //     corner of the associated control window, to display the ToolTip.
        //
        //   duration:
        //     An System.Int32 containing the duration, in milliseconds, to display the ToolTip.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The window parameter is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     duration is less than or equal to 0.
        public new void Show(string text, IWin32Window window, Point point, int duration)
        {
            ComputeToolTipSize(window as Control, text);
            base.Show(text, window, point, duration);
        }
        //
        // Summary:
        //     Sets the ToolTip text associated with the specified control, and then displays
        //     the ToolTip modally at the specified relative position.
        //
        // Parameters:
        //   text:
        //     A System.String containing the new ToolTip text.
        //
        //   window:
        //     The System.Windows.Forms.Control to display the ToolTip for.
        //
        //   x:
        //     The horizontal offset, in pixels, relative to the upper-left corner of the associated
        //     control window, to display the ToolTip.
        //
        //   y:
        //     The vertical offset, in pixels, relative to the upper-left corner of the associated
        //     control window, to display the ToolTip.
        public new void Show(string text, IWin32Window window, int x, int y)
        {
            ComputeToolTipSize(window as Control, text);
            base.Show(text, window, x, y);
        }
        //
        // Summary:
        //     Sets the ToolTip text associated with the specified control, and then displays
        //     the ToolTip for the specified duration at the specified relative position.
        //
        // Parameters:
        //   text:
        //     A System.String containing the new ToolTip text.
        //
        //   window:
        //     The System.Windows.Forms.Control to display the ToolTip for.
        //
        //   x:
        //     The horizontal offset, in pixels, relative to the upper-left corner of the associated
        //     control window, to display the ToolTip.
        //
        //   y:
        //     The vertical offset, in pixels, relative to the upper-left corner of the associated
        //     control window, to display the ToolTip.
        //
        //   duration:
        //     An System.Int32 containing the duration, in milliseconds, to display the ToolTip.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The window parameter is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     duration is less than or equal to 0.
        public new void Show(string text, IWin32Window window, int x, int y, int duration)
        {
            ComputeToolTipSize(window as Control, text);
            base.Show(text, window, x, y, duration);
        }
        //
        // Summary:
        //     Sets the ToolTip text associated with the specified control, and then displays
        //     the ToolTip modally at the specified relative position.
        //
        // Parameters:
        //   text:
        //     A System.String containing the new ToolTip text.
        //
        //   window:
        //     The System.Windows.Forms.Control to display the ToolTip for.
        //
        //   point:
        //     A System.Drawing.Point containing the offset, in pixels, relative to the upper-left
        //     corner of the associated control window, to display the ToolTip.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The window parameter is null.
        public new void Show(string text, IWin32Window window, Point point)
        {
            ComputeToolTipSize(window as Control, text);
            base.Show(text, window, point);
        }

        private void MaterialToolTip_Popup(object sender, PopupEventArgs e)
        {
            
            //
            e.ToolTipSize = Size.Ceiling(toopTipSize);
        }

        private void MaterialToolTip_Draw(object sender, DrawToolTipEventArgs e)
        {

            //RectangleF tooltipRectF = new RectangleF(0, 0, toopTipSize.Width, toopTipSize.Height);
            //GraphicsPath tooltipPath = DrawHelper.CreateRoundRect(tooltipRectF, 4);
            //e.Graphics.FillPath(Brushes.Gray, tooltipPath);

            e.Graphics.Clear(SkinManager.BackgroundColor);
            Rectangle rec = new Rectangle(0, 0, Size.Ceiling(toopTipSize).Width, Size.Ceiling(toopTipSize).Height);
            RectangleF recF = new RectangleF(rec.Location, rec.Size);
            recF.X -= 0.5f;
            recF.Y -= 0.5f;

            //Rectangle rec = new Rectangle(0, 0, Size.Ceiling(toopTipSize).Width, Size.Ceiling(toopTipSize).Height);
            DrawHelper.DrawSquareShadow(e.Graphics, rec);

            e.Graphics.FillRectangle(SkinManager.BackdropBrush, recF);

            
            //e.DrawBackground();
            string caption = e.ToolTipText;
            
            Font font = SkinManager.getFontByType(MaterialSkinManager.fontType.Caption);
            StringFormat sf = StringFormat.GenericTypographic;
            sf.Alignment = StringAlignment.Center ;
            sf.LineAlignment = StringAlignment.Center;
          
            
            e.Graphics.DrawString(caption, font, SkinManager.TextHighEmphasisBrush, rec, sf);
        }

        private void ComputeToolTipSize(Control control, string text)
        {
            if(control == null)
            {
                toopTipSize.Width = 0;
                toopTipSize.Height = 0;
                return;
            }

            using (Graphics g =  control.CreateGraphics())
            {
                if (!string.IsNullOrEmpty(text))
                {
                    StringFormat sf = StringFormat.GenericTypographic;
                    sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

                    toopTipSize = g.MeasureString(text.ToUpper(), SkinManager.getFontByType(MaterialSkinManager.fontType.Caption), PointF.Empty, sf);
                    toopTipSize.Width += margin.Left;
                    toopTipSize.Height += margin.Vertical;
                }
                else
                {
                    toopTipSize.Width = 0;
                    toopTipSize.Height = 0;
                }
            }
        }

        private Point ComputeStartLocation(RectangleF controlRectangle, MaterialToolTipPlacement placement)
        {
            Point point = default;
            Size tlTipSize = Size.Ceiling(this.toopTipSize);
            switch(placement)
            {
                case MaterialToolTipPlacement.BottomCenter:
                    point = Point.Ceiling(new PointF(controlRectangle.X + (controlRectangle.Width - tlTipSize.Width) / 2, controlRectangle.Bottom + margin.Top));
                    break;
                case MaterialToolTipPlacement.BottomLeft:
                    point = Point.Ceiling(new PointF(controlRectangle.X + 0, controlRectangle.Bottom + margin.Top));
                    break;
                case MaterialToolTipPlacement.BottomRight:
                    point = Point.Ceiling(new PointF(controlRectangle.X + controlRectangle.Width - tlTipSize.Width, controlRectangle.Bottom + margin.Top));
                    break;
                case MaterialToolTipPlacement.TopCenter:
                    point = Point.Ceiling(new PointF(controlRectangle.X + (controlRectangle.Width - tlTipSize.Width) / 2, controlRectangle.Y -(tlTipSize.Height + margin.Top)));
                    break;
                case MaterialToolTipPlacement.TopLeft:
                    point = Point.Ceiling(new PointF(controlRectangle.X + 0, controlRectangle.Y - (tlTipSize.Height + margin.Top)));
                    break;
                case MaterialToolTipPlacement.TopRight:
                    point = Point.Ceiling(new PointF(controlRectangle.X + controlRectangle.Width - tlTipSize.Width, controlRectangle.Y - (tlTipSize.Height + margin.Top)));
                    break;

                case MaterialToolTipPlacement.LeftTop:
                    point = Point.Ceiling(new PointF(controlRectangle.X - (tlTipSize.Width + margin.Left), controlRectangle.Y + 0));
                    break;
                case MaterialToolTipPlacement.LeftCenter:
                    point = Point.Ceiling(new PointF(controlRectangle.X - (tlTipSize.Width + margin.Left), controlRectangle.Y + (controlRectangle.Height - tlTipSize.Height) / 2));
                    break;
                case MaterialToolTipPlacement.LeftBottom:
                    point = Point.Ceiling(new PointF(controlRectangle.X - (tlTipSize.Width + margin.Left), controlRectangle.Y + controlRectangle.Height- tlTipSize.Height));
                    break;
                case MaterialToolTipPlacement.RightTop:
                    point = Point.Ceiling(new PointF(controlRectangle.Right + margin.Right, controlRectangle.Y + 0));
                    break;
                case MaterialToolTipPlacement.RightCenter:
                    point = Point.Ceiling(new PointF(controlRectangle.Right + margin.Right, controlRectangle.Y + (controlRectangle.Height - tlTipSize.Height) / 2));
                    break;
                case MaterialToolTipPlacement.RightBottom:
                    point = Point.Ceiling(new PointF(controlRectangle.Right + margin.Right, controlRectangle.Y + controlRectangle.Height - tlTipSize.Height));
                    break;

            }
            
            return point;
        }

    }

    public enum MaterialToolTipPlacement
    {
        LeftTop,
        LeftCenter,
        LeftBottom,
        RightTop,
        RightCenter,
        RightBottom,
        TopLeft,
        TopCenter,
        TopRight,
        BottomLeft,
        BottomCenter,
        BottomRight,

    }
}

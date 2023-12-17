namespace MaterialSkin.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    public sealed class MaterialDivider2 : Control, IMaterialControl
    {
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }

        private int minHeight = 5;
        private SizeF titleSize = default;
        private Rectangle titleRect = new Rectangle();

        private Padding margin;


        private TitlePosition _titlePosition = TitlePosition.Center;
        [Category("Material")]
        public TitlePosition TitlePosition
        {
            get
            {
                return _titlePosition;
            }
            set
            {
                TitlePosition oldTitlePosition = _titlePosition;
                _titlePosition = value;
                allocatePositonSize(oldTitlePosition, titleSize);
                Invalidate();
            }
        }


        private string _title = string.Empty;
        [Category("Material")]
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value.TrimStart().TrimEnd();
     
                SizeF oldTitleSize = this.titleSize;
                if (!string.IsNullOrWhiteSpace(_title))
                {
                    computeTitleize(_title);
                }
                allocatePositonSize(_titlePosition, oldTitleSize);
                Invalidate();
            }
        }

        private Rectangle leftDivider = default;
        private Rectangle rightDivider = default;
        public MaterialDivider2()
        {
            margin = new Padding(10, 4, 10, 4);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            Height = 10;
            BackColor = SkinManager.BackgroundColor;//SkinManager.DividersColor;
            Font = SkinManager.getFontByType(MaterialSkinManager.fontType.Caption);

            //if true, the text is clear
            DoubleBuffered = true;
            //
            leftDivider = new Rectangle(0, this.Height / 2, this.Width, 1);
            rightDivider = leftDivider;
        }

        private void computeTitleize(string text)
        {

            using (Graphics g = this.CreateGraphics())
            {
                if (!string.IsNullOrEmpty(text))
                {
                    StringFormat sf = StringFormat.GenericTypographic;
                    sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

                    titleSize = g.MeasureString(text.ToUpper(), SkinManager.getFontByType(MaterialSkinManager.fontType.Caption), PointF.Empty, sf);
                }
                else
                {
                    titleSize.Width = 0;
                    titleSize.Height = 0;
                }
            }
        }

        private void allocatePositonSize(TitlePosition oldTitlePosition, SizeF oldTitleSize)
        {
            if (string.IsNullOrWhiteSpace(_title))
            {
                leftDivider.Width = this.Width;
                rightDivider.Width = 0;
                leftDivider.Location = new Point(0, this.Height / 2);
                rightDivider.Location = leftDivider.Location;
                titleRect.Width = titleRect.Height = 0 ;
            }
            else
            {
                //if(titlePosition != oldTitlePosition)
                //{
                //    leftDivider.Width = this.Width;
                //    rightDivider.Width = 0;
                //}
//                SizeF oldTitleSize = this.titleSize;
//                computeTitleize(title);

                if (this.Height < titleSize.Height)
                    this.Height = Convert.ToInt32(Math.Ceiling(titleSize.Height));

                leftDivider.Width = this.Width;
                //if(rightDivider.Width == 0)
                //{
                    if (titleSize.Width >= leftDivider.Width)
                    {
                        return;
                    }
                    //totle width
                    int width = leftDivider.Width;
                    //left and right margin ,and title width
                    float space = margin.Horizontal + titleSize.Width;
                    if (_titlePosition == TitlePosition.Center)
                    {
                        leftDivider.Width = width / 2;
                    }
                    else if(_titlePosition == TitlePosition.Left)
                    {
                        leftDivider.Width = width / 4;
                    }
                    else if(_titlePosition == TitlePosition.Right)
                    {
                        leftDivider.Width = width * 3 / 4;
                    }
                    rightDivider.Width = width - leftDivider.Width;
                    leftDivider.Width -= Convert.ToInt32(space / 2);
                    rightDivider.Width -= Convert.ToInt32(space / 2);
                    leftDivider.Location = new Point(0, this.Height / 2);
                rightDivider.Location = new Point(width - rightDivider.Width, this.Height / 2);

                //}
                ////rightDivider.Width > 0
                //else
                //{
                //    if (titleSize.Width >= rightDivider.Location.X + rightDivider.Width)
                //    {
                //        return;
                //    }
                //    float bias = (oldTitleSize.Width - this.titleSize.Width) / 2;
                //    float oldSpace = margin.Horizontal + oldTitleSize.Width;
                //    leftDivider.Width += Convert.ToInt32(bias);
                //    rightDivider.Width += Convert.ToInt32(bias);

                //    rightDivider.Location = new Point(rightDivider.Location.X - Convert.ToInt32(bias), rightDivider.Location.Y);
                    
                //}
                //
                titleRect.Width = Convert.ToInt32(titleSize.Width + margin.Horizontal);
                titleRect.Height = Convert.ToInt32(this.Height);
                titleRect.X = leftDivider.Width;
                titleRect.Y = 0;

            }
            
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            allocatePositonSize(_titlePosition, titleSize);

            Invalidate();
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Height < minHeight)
                Height = minHeight;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            e.Graphics.Clear(BackColor);

            using (var dividerPen = new Pen(SkinManager.DividersColor, 1))
            {
                // left divider of title
                g.DrawLine(
                        dividerPen,
                        leftDivider.X,
                        leftDivider.Y,
                        leftDivider.X + leftDivider.Width,
                        leftDivider.Y
                   );
                // right divider of title
                g.DrawLine(
                        dividerPen,
                        rightDivider.X,
                        rightDivider.Y,
                        rightDivider.X + rightDivider.Width,
                        rightDivider.Y);
            }

            e.Graphics.FillRectangle(SkinManager.BackgroundBrush, titleRect);
            //Draw title
            using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
            {
                // Draw header text
                NativeText.DrawTransparentText(
                    _title,
                    SkinManager.getLogFontByType(MaterialSkinManager.fontType.Caption),
                    SkinManager.TextHighEmphasisColor,
                    titleRect.Location,
                    titleRect.Size,
                    NativeTextRenderer.TextAlignFlags.Center | NativeTextRenderer.TextAlignFlags.Middle);
            }
        }

    }

    public enum TitlePosition
    {
        Left,
        Center,
        Right
    }
}
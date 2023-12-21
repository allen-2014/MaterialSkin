namespace MaterialSkin.Controls
{
    using MaterialSkin.Animations;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    public class MaterialRadioButton : RadioButton, IMaterialControl
    {
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }

        [Browsable(false)]
        public Point MouseLocation { get; set; }

        private bool ripple;

        [Category("Material")]
        public bool Ripple
        {
            get { return ripple; }
            set
            {
                ripple = value;
                AutoSize = AutoSize; //Make AutoSize directly set the bounds.

                if (value)
                {
                    if (AutoSize || Height < HEIGHT_RIPPLE)
                        Height = HEIGHT_RIPPLE;
                }
                else
                {
                    Margin = new Padding(0);
                    if (AutoSize || Height < _radioButtonBounds.Height)
                    {
                        Height = _radioButtonBounds.Height;
                    }
                        
                }

                Invalidate();
            }
        }

        private MaterialSizeType sizeType = MaterialSizeType.Default;
        [Category("Material")]
        public MaterialSizeType SizeType
        {
            get
            {
                return this.sizeType;
            }
            set
            {
                this.sizeType = value;
                if(this.sizeType == MaterialSizeType.Small)
                {
                    _boxOffset = Height / 2 - (int)(RADIOBUTTON_DENSITY_SIZE / 2) - 1;
                    _radioButtonBounds = new Rectangle(_boxOffset, _boxOffset, RADIOBUTTON_DENSITY_SIZE, RADIOBUTTON_DENSITY_SIZE);
                    _radioSizeHalf = RADIOBUTTON_DENSITY_SIZE_HALF;
                    //_radioButtonCenter = _boxOffset + RADIOBUTTON_DENSITY_SIZE_HALF;
                    _radioButtonSize = RADIOBUTTON_DENSITY_SIZE;
                    _textOffset = DENSITY_TEXT_OFFSET;
                }
                else
                {
                    _boxOffset = Height / 2 - (int)(RADIOBUTTON_SIZE / 2) - 1;
                    _radioButtonBounds = new Rectangle(_boxOffset, _boxOffset, RADIOBUTTON_SIZE, RADIOBUTTON_SIZE);
                    _radioSizeHalf = RADIOBUTTON_SIZE_HALF;
                    //_radioButtonCenter = _boxOffset + RADIOBUTTON_SIZE_HALF;
                    _radioButtonSize = RADIOBUTTON_SIZE;
                    _textOffset = TEXT_OFFSET;
                }
                if(AutoSize)
                {
                    GetPreferredSize(Size.Empty);
                }

                Invalidate();
            }
        }

        // animation managers
        private readonly AnimationManager _checkAM;

        private readonly AnimationManager _rippleAM;
        private readonly AnimationManager _hoverAM;

        // size related variables which should be recalculated onsizechanged
        private Rectangle _radioButtonBounds;

        private int _radioButtonCenter;
        private int _radioButtonSize;
        private int _radioSizeHalf;
        private int _textOffset;

        private int _boxOffset;

        // size constants
        private const int HEIGHT_RIPPLE = 37;

        private const int HEIGHT_NO_RIPPLE = 20;
        private const int RADIOBUTTON_SIZE = 18;
        private const int RADIOBUTTON_DENSITY_SIZE = 14;
        private const int RADIOBUTTON_SIZE_HALF = RADIOBUTTON_SIZE / 2;
        private const int RADIOBUTTON_DENSITY_SIZE_HALF = RADIOBUTTON_DENSITY_SIZE / 2;
        private const int RADIOBUTTON_OUTER_CIRCLE_WIDTH = 2;
        private const int RADIOBUTTON_INNER_CIRCLE_SIZE = RADIOBUTTON_SIZE - (2 * RADIOBUTTON_OUTER_CIRCLE_WIDTH);
        private const int TEXT_OFFSET = 26;
        private const int DENSITY_TEXT_OFFSET = 22;


        
        public MaterialRadioButton()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);

            _checkAM = new AnimationManager
            {
                AnimationType = AnimationType.EaseInOut,
                Increment = 0.06
            };
            _hoverAM = new AnimationManager(true)
            {
                AnimationType = AnimationType.Linear,
                Increment = 0.10
            };
            _rippleAM = new AnimationManager(false)
            {
                AnimationType = AnimationType.Linear,
                Increment = 0.10,
                SecondaryIncrement = 0.08
            };

            _checkAM.OnAnimationProgress += sender => Invalidate();
            _hoverAM.OnAnimationProgress += sender => Invalidate();
            _rippleAM.OnAnimationProgress += sender => Invalidate();

            TabStopChanged += (sender, e) => TabStop = true;

            CheckedChanged += (sender, args) =>
            {
                if (Ripple)
                    _checkAM.StartNewAnimation(Checked ? AnimationDirection.In : AnimationDirection.Out);
            };

            SizeChanged += OnSizeChanged;

            Ripple = true;
            SizeType = MaterialSizeType.Default;
            MouseLocation = new Point(-1, -1);
        }

        private void OnSizeChanged(object sender, EventArgs eventArgs)
        {
            if (Ripple && Height < HEIGHT_RIPPLE)
            {
                Height = HEIGHT_RIPPLE;
            }
            else if (!Ripple && Height < HEIGHT_NO_RIPPLE)
            {
                Height = HEIGHT_NO_RIPPLE;
            }
            //minus 1 for radio button and text can have the same horizon
            _boxOffset = Height / 2 - (int)(_radioButtonSize / 2) - 1;
            _radioSizeHalf = _radioButtonSize / 2;
            //if(density == MaterialDensity.Default)
            //{
            //    _radioButtonBounds = new Rectangle(_boxOffset, _boxOffset, RADIOBUTTON_SIZE, RADIOBUTTON_SIZE);
            //}
            //else if(density == MaterialDensity.Dense)
            //{
            //    _radioButtonBounds = new Rectangle(_boxOffset, _boxOffset, RADIOBUTTON_DENSITY_SIZE, RADIOBUTTON_DENSITY_SIZE);
            //}
            _radioButtonBounds = new Rectangle(_boxOffset, _boxOffset, _radioButtonSize, _radioButtonSize);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size strSize = default;

            using (NativeTextRenderer NativeText = new NativeTextRenderer(CreateGraphics()))
            {

                if (sizeType == MaterialSizeType.Default)
                {
                    strSize = NativeText.MeasureLogString(Text, SkinManager.getLogFontByType(MaterialSkinManager.fontType.Body1));
                }
                else if (sizeType == MaterialSizeType.Small)
                {
                    strSize = NativeText.MeasureLogString(Text, SkinManager.getLogFontByType(MaterialSkinManager.fontType.Body2));
                }
            }

            int w = _boxOffset + _textOffset + strSize.Width;
            Width = w;
            return Ripple ? new Size(w, HEIGHT_RIPPLE) : new Size(w, HEIGHT_NO_RIPPLE);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // clear the control
            g.Clear(Parent.BackColor);

            int RADIOBUTTON_CENTER = _boxOffset + _radioSizeHalf;//_boxOffset + (density == MaterialDensity.Default ? RADIOBUTTON_SIZE_HALF:(density == MaterialDensity.Dense? RADIOBUTTON_DENSITY_SIZE_HALF: RADIOBUTTON_SIZE_HALF));
            Point animationSource = new Point(RADIOBUTTON_CENTER, RADIOBUTTON_CENTER);

            double animationProgress = _checkAM.GetProgress();

            int colorAlpha = Enabled ? (Checked && !Ripple ? 255: (int)(animationProgress * 255.0)) : SkinManager.CheckBoxOffDisabledColor.A;
            int backgroundAlpha = Enabled ? (int)(SkinManager.CheckboxOffColor.A * (1.0 - animationProgress)) : SkinManager.CheckBoxOffDisabledColor.A;
            float animationSize = (float)(animationProgress * 9f);
            float animationSizeHalf = animationSize / 2;
            int rippleHeight = (HEIGHT_RIPPLE % 2 == 0) ? HEIGHT_RIPPLE - 3 : HEIGHT_RIPPLE - 2;

            Color RadioColor = Color.FromArgb(colorAlpha, Enabled ? SkinManager.ColorScheme.AccentColor : SkinManager.CheckBoxOffDisabledColor);

            // draw hover animation
            if (Ripple)
            {
                double animationValue = _hoverAM.GetProgress();
                int rippleSize = (int)(rippleHeight * (0.7 + (0.3 * animationValue)));

                using (SolidBrush rippleBrush = new SolidBrush(Color.FromArgb((int)(40 * animationValue),
                    !Checked ? (SkinManager.Theme == MaterialSkinManager.Themes.LIGHT ? Color.Black : Color.White) : RadioColor)))
                {
                    g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize - 1, rippleSize - 1));
                }
            }

            // draw ripple animation
            if (Ripple && _rippleAM.IsAnimating())
            {
                for (int i = 0; i < _rippleAM.GetAnimationCount(); i++)
                {
                    double animationValue = _rippleAM.GetProgress(i);
                    int rippleSize = (_rippleAM.GetDirection(i) == AnimationDirection.InOutIn) ? (int)(rippleHeight * (0.7 + (0.3 * animationValue))) : rippleHeight;

                    using (SolidBrush rippleBrush = new SolidBrush(Color.FromArgb((int)((animationValue * 40)), !Checked ? (SkinManager.Theme == MaterialSkinManager.Themes.LIGHT ? Color.Black : Color.White) : RadioColor)))
                    {
                        g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, animationSource.Y - rippleSize / 2, rippleSize - 1, rippleSize - 1));
                    }
                }
            }

            // draw radiobutton circle
            using (Pen pen = new Pen(DrawHelper.BlendColor(Parent.BackColor, Enabled ? SkinManager.CheckboxOffColor : SkinManager.CheckBoxOffDisabledColor, backgroundAlpha), 2))
            {
                //int radio_size = (density == MaterialDensity.Default ? RADIOBUTTON_SIZE : (density == MaterialDensity.Dense? RADIOBUTTON_DENSITY_SIZE: RADIOBUTTON_SIZE));
                g.DrawEllipse(pen, new Rectangle(_boxOffset, _boxOffset, _radioButtonSize, _radioButtonSize));
            }

            if (Enabled)
            {
                using (Pen pen = new Pen(RadioColor, 2))
                {
                    g.DrawEllipse(pen, new Rectangle(_boxOffset, _boxOffset, _radioButtonSize, _radioButtonSize));
                }
            }

            if (Checked)
            {
                float ellipseWidth = animationSize;
                float ellipseWidthHalf = animationSizeHalf;
                if (!Ripple)
                {
                    ellipseWidth = _radioButtonSize - 6;
                    ellipseWidthHalf = _radioSizeHalf - 3;

                }

                using (SolidBrush brush = new SolidBrush(RadioColor))
                {
                    //g.FillEllipse(brush, new RectangleF(RADIOBUTTON_CENTER - animationSizeHalf, RADIOBUTTON_CENTER - animationSizeHalf, animationSize, animationSize));
                    g.FillEllipse(brush, new RectangleF(RADIOBUTTON_CENTER - ellipseWidthHalf, RADIOBUTTON_CENTER - ellipseWidthHalf, ellipseWidth, ellipseWidth));
                }
            }

            // Text
            using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
            {
                Rectangle textLocation = new Rectangle(_boxOffset + _textOffset, 0, Width, Height);
                NativeText.DrawTransparentText(Text, SkinManager.getLogFontByType(sizeType == MaterialSizeType.Default?MaterialSkinManager.fontType.Body1:(sizeType == MaterialSizeType.Small? MaterialSkinManager.fontType.Body2: MaterialSkinManager.fontType.Body1)),
                    Enabled ? SkinManager.TextHighEmphasisColor : SkinManager.TextDisabledOrHintColor,
                    textLocation.Location,
                    textLocation.Size,
                    NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
            }
        }

        private bool IsMouseInCheckArea()
        {
            return ClientRectangle.Contains(MouseLocation);
        }

        private bool hovered = false;

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (DesignMode) return;

            MouseState = MouseState.OUT;

            GotFocus += (sender, AddingNewEventArgs) =>
            {
                if (Ripple && !hovered)
                {
                    _hoverAM.StartNewAnimation(AnimationDirection.In, new object[] { Checked });
                    hovered = true;
                }
            };

            LostFocus += (sender, args) =>
            {
                if (Ripple && hovered)
                {
                    _hoverAM.StartNewAnimation(AnimationDirection.Out, new object[] { Checked });
                    hovered = false;
                }
            };

            MouseEnter += (sender, args) =>
            {
                MouseState = MouseState.HOVER;
                //if (Ripple && !hovered)
                //{
                //    _hoverAM.StartNewAnimation(AnimationDirection.In, new object[] { Checked });
                //    hovered = true;
                //}
            };

            MouseLeave += (sender, args) =>
            {
                MouseLocation = new Point(-1, -1);
                MouseState = MouseState.OUT;
                //if (Ripple && hovered)
                //{
                //    _hoverAM.StartNewAnimation(AnimationDirection.Out, new object[] { Checked });
                //    hovered = false;
                //}
            };

            MouseDown += (sender, args) =>
            {
                MouseState = MouseState.DOWN;
                if (Ripple)
                {
                    _rippleAM.SecondaryIncrement = 0;
                    _rippleAM.StartNewAnimation(AnimationDirection.InOutIn, new object[] { Checked });
            
                }
           
            };

            KeyDown += (sender, args) =>
            {
                if (Ripple && (args.KeyCode == Keys.Space) && _rippleAM.GetAnimationCount() == 0)
                {
                    _rippleAM.SecondaryIncrement = 0;
                    _rippleAM.StartNewAnimation(AnimationDirection.InOutIn, new object[] { Checked });
                }
            };

            MouseUp += (sender, args) =>
            {
                if (Ripple)
                {
                    MouseState = MouseState.HOVER;
                    _rippleAM.SecondaryIncrement = 0.08;
                    _hoverAM.StartNewAnimation(AnimationDirection.Out, new object[] { Checked });
                    hovered = false;
                }
            };

            KeyUp += (sender, args) =>
            {
                if (Ripple && (args.KeyCode == Keys.Space))
                {
                    MouseState = MouseState.HOVER;
                    _rippleAM.SecondaryIncrement = 0.08;
                }
            };

            MouseMove += (sender, args) =>
            {
                MouseLocation = args.Location;
                Cursor = IsMouseInCheckArea() ? Cursors.Hand : Cursors.Default;
            };
        }
    }

}
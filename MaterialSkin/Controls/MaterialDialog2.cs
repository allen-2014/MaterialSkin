using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using MaterialSkin.Animations;

namespace MaterialSkin.Controls
{
    public class MaterialDialog2 : MaterialForm
    {
        private const int LEFT_RIGHT_PADDING = 24;
        private const int BUTTON_PADDING = 8;
        private const int BUTTON_HEIGHT = 36;
        private const int TEXT_TOP_PADDING = 17;
        private const int TEXT_BOTTOM_PADDING = 28;
        private const int  MIN_HEADER_HEIGHT = 24;
        private int _header_Height = 40;

        private MaterialButton _validationButton = default;//new MaterialButton();
        private MaterialButton _cancelButton = default;//new MaterialButton();
        private AnimationManager _AnimationManager;
        private bool CloseAnimation = false;
        private bool OpenAnimation = false;
        private Form _formOverlay;
        private string _text;
        private string _title = string.Empty;

        

        private ButtonState _buttonState = ButtonState.None;
        private Rectangle _xButtonBounds => new Rectangle(Width - (_header_Height + MIN_HEADER_HEIGHT) /2, (_header_Height - MIN_HEADER_HEIGHT) /2, MIN_HEADER_HEIGHT, MIN_HEADER_HEIGHT);


        public new Region Region
        {
            get 
            {
                base.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 6, 6));
                return base.Region;
            }

        }
        public new int Height
        {
            get { return base.Height; }
            set
            {
                int maxMinHeight = _header_Height > MIN_HEADER_HEIGHT ? _header_Height : MIN_HEADER_HEIGHT;
                if (value < maxMinHeight)
                {
                    base.Height = maxMinHeight;
                }
                else
                    base.Height = value;
                
                Invalidate();
            }
        }

        private bool _ShowTitleDivideLine = false;
        [Category("MaterialDialog2")]
        public bool ShowTitleDivideLine
        {
            get
            {
                return _ShowTitleDivideLine;
            }
            set
            {
                _ShowTitleDivideLine = value;
                
                Invalidate();
            }
        }

        [Category("MaterialDialog2"), Localizable(true)]
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                if (_title.Length == 0)
                    _header_Height = MIN_HEADER_HEIGHT;
                else
                    _header_Height = 40;

                Invalidate();
            }
        }

        private NativeTextRenderer.TextAlignFlags nativeTextAligh = 
            NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Bottom;
        private TitleAlign _TitleAlign = TitleAlign.LeftBottom;
        [Category("MaterialDialog2")]
        public TitleAlign TitleAlign
        {
            get
            {
                return _TitleAlign;
            }
            set
            {
                _TitleAlign = value;
                switch(_TitleAlign)
                {
                    case TitleAlign.CenterBottom:
                        nativeTextAligh = NativeTextRenderer.TextAlignFlags.Center | NativeTextRenderer.TextAlignFlags.Bottom;
                        break;
                    case TitleAlign.CenterMiddle:
                        nativeTextAligh = NativeTextRenderer.TextAlignFlags.Center | NativeTextRenderer.TextAlignFlags.Middle;
                        break;
                    case TitleAlign.CenterTop:
                        nativeTextAligh = NativeTextRenderer.TextAlignFlags.Center | NativeTextRenderer.TextAlignFlags.Top;
                        break;
                    case TitleAlign.LeftBottom:
                        nativeTextAligh = NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Bottom;
                        break;
                    case TitleAlign.LeftMiddle:
                        nativeTextAligh = NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle;
                        break;
                    case TitleAlign.LeftTop:
                        nativeTextAligh = NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Top;
                        break;
                    default:
                        nativeTextAligh = NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Bottom;
                        break;
                }
                Invalidate();
            }
        }



        [Category("MaterialDialog2")]
        public MaterialButton MValidationButton
        {
            get { return _validationButton; }
            set
            {
                _validationButton = value;
                this.AcceptButton = _validationButton;
                if(_validationButton != null)
                {
                    _validationButton.DialogResult = DialogResult.OK;
                    _validationButton.AutoSize = false;
                    _validationButton.DrawShadows = false;
                    _validationButton.Type = MaterialButton.MaterialButtonType.Text;
                    _validationButton.UseAccentColor = buttonUseAccentColor;
        
                    int _buttonWidth = ((TextRenderer.MeasureText(_validationButton.Text, SkinManager.getFontByType(MaterialSkinManager.fontType.Button))).Width + 32);
                    _validationButton.Width = _buttonWidth;
                    _validationButton.Height = BUTTON_HEIGHT;
                    _validationButton.TextChanged += _validationButton_TextChanged;
                }
                Invalidate();
            }
        }



        [Category("MaterialDialog2")]
        public MaterialButton MCancelButton
        {
            get { return _cancelButton; }
            set
            {
                _cancelButton = value;
                this.CancelButton = _cancelButton;
                if(_cancelButton != null)
                {
                    _cancelButton.DialogResult = DialogResult.Cancel;
                    _cancelButton.AutoSize = false;
                    _cancelButton.DrawShadows = false;
                    _cancelButton.Type = MaterialButton.MaterialButtonType.Text;
                    _cancelButton.UseAccentColor = buttonUseAccentColor;

                    int _buttonWidth = ((TextRenderer.MeasureText(_cancelButton.Text, SkinManager.getFontByType(MaterialSkinManager.fontType.Button))).Width + 32);
                    _cancelButton.Width = _buttonWidth;
                    _cancelButton.Height = BUTTON_HEIGHT;
                    _cancelButton.TextChanged += _cancelButton_TextChanged;
                }
                Invalidate();
            }
        }


        private bool buttonUseAccentColor = false;

        [Category("MaterialDialog2")]
        public bool ButtonUseAccentColor
        {
            get => buttonUseAccentColor;
            set
            {
                buttonUseAccentColor = value;
                if (_validationButton != null)
                {
                    _validationButton.UseAccentColor = value;
                }
                if(_cancelButton != null)
                {
                    _cancelButton.UseAccentColor = value;
                }

                Invalidate();
            }
        }

        /// <summary>
        /// The Collection for the Buttons
        /// </summary>
        //public ObservableCollection<MaterialButton> Buttons { get; set; }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );


        public MaterialDialog2(Form parentForm)
        {
            _formOverlay = new Form
            {
                BackColor = Color.Black,
                Opacity = 0.5,
                MinimizeBox = false,
                MaximizeBox = false,
                Text = "",
                ShowIcon = false,
                ControlBox = false,
                FormBorderStyle = FormBorderStyle.None,
                //Size = new Size(ParentForm.Width, ParentForm.Height),
                ShowInTaskbar = false,
                //Owner = ParentForm,
                Visible = true,
                //Location = new Point(ParentForm.Location.X, ParentForm.Location.Y),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom,
            };
            //when designed mode, parentForm is null
            if (parentForm != null)
            {
                _formOverlay.Size = new Size(parentForm.Width, parentForm.Height);
                _formOverlay.Owner = parentForm;
                _formOverlay.Location = new Point(parentForm.Location.X, parentForm.Location.Y);
            }
            //show the x button
            this.ControlBox = true;

            if (Title.Length == 0)
                _header_Height = MIN_HEADER_HEIGHT;
            else
                _header_Height = 40;

            ShowInTaskbar = false;
            Sizable = false;
            if(!DesignMode) Opacity = 0;

            BackColor = SkinManager.BackgroundColor;
            FormStyle = FormStyles.StatusAndActionBar_None;

            _AnimationManager = new AnimationManager();
            _AnimationManager.AnimationType = AnimationType.EaseOut;
            _AnimationManager.Increment = 0.03;
            _AnimationManager.OnAnimationProgress += _AnimationManager_OnAnimationProgress;


            //Width = this.Width;
            //int TextWidth = TextRenderer.MeasureText(_text, SkinManager.getFontByType(MaterialSkinManager.fontType.Body1)).Width;
            //int RectWidth = Width - (2 * LEFT_RIGHT_PADDING) - BUTTON_PADDING;
            //int RectHeight = ((TextWidth / RectWidth) + 1) * 19;
            //Rectangle textRect = new Rectangle(
            //    LEFT_RIGHT_PADDING,
            //    _header_Height + TEXT_TOP_PADDING,
            //    RectWidth,
            //    RectHeight + 9);

            //MIN_HEADER_HEIGHT = _header_Height; //560;
            //Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 6, 6));


            //_xButtonBounds = new Rectangle(Width - LEFT_RIGHT_PADDING, 0, LEFT_RIGHT_PADDING, _header_Height);
        }



        public MaterialDialog2():this(null)
        {
            //_formOverlay = new Form
            //{
            //    BackColor = Color.Black,
            //    Opacity = 0.5,
            //    MinimizeBox = false,
            //    MaximizeBox = false,
            //    Text = "",
            //    ShowIcon = false,
            //    ControlBox = false,
            //    FormBorderStyle = FormBorderStyle.None,
            //    //Size = new Size(ParentForm.Width, ParentForm.Height),
            //    ShowInTaskbar = false,
            //    //Owner = ParentForm,
            //    Visible = true,
            //    //Location = new Point(ParentForm.Location.X, ParentForm.Location.Y),
            //    Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom,
            //};

            //this.ControlBox = true;
            //string Title = string.Empty;
            //_title = "Title";
            //if (Title.Length == 0)
            //    _header_Height = 20;
            //else
            //    _header_Height = 40;

            //_text = Text;
            //ShowInTaskbar = false;
            //Sizable = false;

            //BackColor = SkinManager.BackgroundColor;
            //FormStyle = FormStyles.StatusAndActionBar_None;

            //_AnimationManager = new AnimationManager();
            //_AnimationManager.AnimationType = AnimationType.EaseOut;
            //_AnimationManager.Increment = 0.03;
            //_AnimationManager.OnAnimationProgress += _AnimationManager_OnAnimationProgress;


            ////Width = this.Width;
            ////int TextWidth = TextRenderer.MeasureText(_text, SkinManager.getFontByType(MaterialSkinManager.fontType.Body1)).Width;
            ////int RectWidth = Width - (2 * LEFT_RIGHT_PADDING) - BUTTON_PADDING;
            ////int RectHeight = ((TextWidth / RectWidth) + 1) * 19;
            ////Rectangle textRect = new Rectangle(
            ////    LEFT_RIGHT_PADDING,
            ////    _header_Height + TEXT_TOP_PADDING,
            ////    RectWidth,
            ////    RectHeight + 9);

            //minHeight = _header_Height; //560;
            //Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 6, 6));

            //_xButtonBounds = new Rectangle(Width - LEFT_RIGHT_PADDING, 0, Width, _header_Height);

        }


        /// <summary>
        /// Sets up the Starting Location and starts the Animation
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            if (DesignMode)
            {
                _formOverlay.Visible = false;
                return;
            }

            //
            if (this.Owner != null && _formOverlay.Owner == null)
            {
                _formOverlay.Size = new Size(Owner.Width, Owner.Height);
                _formOverlay.Owner = Owner;
                _formOverlay.Location = new Point(Owner.Location.X, Owner.Location.Y);
            }
            else if (this.Owner == null)
                _formOverlay.Visible = false;

            Location = new Point(Convert.ToInt32(Owner.Location.X + (Owner.Width / 2) - (Width / 2)), Convert.ToInt32(Owner.Location.Y + (Owner.Height / 2) - (Height / 2)));
            //animation value = 0 -> 1
            _AnimationManager.StartNewAnimation(AnimationDirection.In);
            OpenAnimation = true;

        }

        private void _validationButton_TextChanged(object sender, EventArgs e)
        {
            if(DesignMode)
            {
                int _buttonWidth = ((TextRenderer.MeasureText(_validationButton.Text, SkinManager.getFontByType(MaterialSkinManager.fontType.Button))).Width + 32);
                _validationButton.Width = _buttonWidth;
                _validationButton.Height = BUTTON_HEIGHT;
            }
        }

        private void _cancelButton_TextChanged(object sender, EventArgs e)
        {
            if(DesignMode)
            {
                int _buttonWidth = ((TextRenderer.MeasureText(_cancelButton.Text, SkinManager.getFontByType(MaterialSkinManager.fontType.Button))).Width + 32);
                _cancelButton.Width = _buttonWidth;
                _cancelButton.Height = BUTTON_HEIGHT;
            }
        }


        /// <summary>
        /// Animates the Form slides
        /// </summary>
        void _AnimationManager_OnAnimationProgress(object sender)
        {
            if (OpenAnimation || CloseAnimation)
            {
                Opacity = _AnimationManager.GetProgress();
            }
        }

        /// <summary>
        /// Ovverides the Paint to create the solid colored backcolor
        /// </summary>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            //Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 6, 6));

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            e.Graphics.Clear(BackColor);


            // Calc title Rect
            Rectangle titleRect = new Rectangle(
                LEFT_RIGHT_PADDING,
                0,
                Width - (2 * LEFT_RIGHT_PADDING),
                _header_Height);

            //Draw title
            using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
            {

                // Draw header text
                NativeText.DrawTransparentText(
                    _title,
                    SkinManager.getLogFontByType(MaterialSkinManager.fontType.H6),
                    SkinManager.TextHighEmphasisColor,
                    titleRect.Location,
                    titleRect.Size,
                    nativeTextAligh);
            }

            //// Calc text Rect

            //int TextWidth = TextRenderer.MeasureText(_text, SkinManager.getFontByType(MaterialSkinManager.fontType.Body1)).Width;
            //int RectWidth = Width - (2 * LEFT_RIGHT_PADDING) - BUTTON_PADDING;
            //int RectHeight = ((TextWidth / RectWidth) + 1) * 19;

            //Rectangle textRect = new Rectangle(
            //    LEFT_RIGHT_PADDING,
            //    _header_Height + 17,
            //    RectWidth,
            //    RectHeight + 19);

            ////Draw  Text
            //using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
            //{
            //    // Draw header text
            //    NativeText.DrawMultilineTransparentText(
            //        _text,
            //        SkinManager.getLogFontByType(MaterialSkinManager.fontType.Body1),
            //        SkinManager.TextHighEmphasisColor,
            //        textRect.Location,
            //        textRect.Size,
            //        NativeTextRenderer.TextAlignFlags.Left | NativeTextRenderer.TextAlignFlags.Middle);
            //}



            // Determine whether or not we even should be drawing the buttons.
            if(ControlBox)
            {
                if (_buttonState == ButtonState.XOver && ControlBox)
                    g.FillRectangle(SkinManager.BackgroundHoverRedBrush, _xButtonBounds);

                if (_buttonState == ButtonState.XDown && ControlBox)
                    g.FillRectangle(SkinManager.BackgroundDownRedBrush, _xButtonBounds);
            }
            using (var formButtonsPen = new Pen(SkinManager.TextHighEmphasisColor, 2))
            {
                // Close button
                if (ControlBox)
                {
                    g.DrawLine(
                        formButtonsPen,
                        _xButtonBounds.X + (int)(_xButtonBounds.Width * 0.33),
                        _xButtonBounds.Y + (int)(_xButtonBounds.Height * 0.33),
                        _xButtonBounds.X + (int)(_xButtonBounds.Width * 0.66),
                        _xButtonBounds.Y + (int)(_xButtonBounds.Height * 0.66)
                   );

                    g.DrawLine(
                        formButtonsPen,
                        _xButtonBounds.X + (int)(_xButtonBounds.Width * 0.66),
                        _xButtonBounds.Y + (int)(_xButtonBounds.Height * 0.33),
                        _xButtonBounds.X + (int)(_xButtonBounds.Width * 0.33),
                        _xButtonBounds.Y + (int)(_xButtonBounds.Height * 0.66));
                }
            }
            if (_ShowTitleDivideLine)
            {
                using (var formButtonsPen = new Pen(SkinManager.DividersColor, 1))
                {
                    //title divide line
                    g.DrawLine(
                            formButtonsPen,
                            0,
                            _header_Height,
                            Width,
                            _header_Height
                       );
                }

            }

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (DesignMode)
                return;
            UpdateButtons(e.Button, e.Location);

            //if (e.Button == MouseButtons.Left && !Maximized && _resizeCursors.Contains(Cursor))
            //    ResizeForm(_resizeDir);
            base.OnMouseDown(e);

        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (DesignMode)
                return;
            UpdateButtons(e.Button, e.Location, true);

            base.OnMouseUp(e);
            //ReleaseCapture();

        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (DesignMode) return;

            var coords = e.Location;

            UpdateButtons(e.Button, coords);

        }


        private void UpdateButtons(MouseButtons button, Point location, bool up = false)
        {
            if (DesignMode) return;

            var oldState = _buttonState;
            bool showMin = MinimizeBox && ControlBox;
            bool showMax = MaximizeBox && ControlBox;

            if (button == MouseButtons.Left && !up)
            {
                if (ControlBox && _xButtonBounds.Contains(location))
                    _buttonState = ButtonState.XDown;
                else
                    _buttonState = ButtonState.None;
                //if (showMin && !showMax && _maxButtonBounds.Contains(location))
                //    _buttonState = ButtonState.MinDown;
                //else if (showMin && showMax && _minButtonBounds.Contains(location))
                //    _buttonState = ButtonState.MinDown;
                //else if (showMax && _maxButtonBounds.Contains(location))
                //    _buttonState = ButtonState.MaxDown;
                //else if (ControlBox && _xButtonBounds.Contains(location))
                //    _buttonState = ButtonState.XDown;
                //else if (_drawerButtonBounds.Contains(location))
                //    _buttonState = ButtonState.DrawerDown;
                //else
                //    _buttonState = ButtonState.None;
            }
            else
            {
                if (ControlBox && _xButtonBounds.Contains(location))
                {
                    _buttonState = ButtonState.XOver;

                    if (oldState == ButtonState.XDown && up)
                        Close();
                }
                else
                    _buttonState = ButtonState.None;

                //if (showMin && !showMax && _maxButtonBounds.Contains(location))
                //{
                //    _buttonState = ButtonState.MinOver;

                //    if (oldState == ButtonState.MinDown && up)
                //        WindowState = FormWindowState.Minimized;
                //}
                //else if (showMin && showMax && _minButtonBounds.Contains(location))
                //{
                //    _buttonState = ButtonState.MinOver;

                //    if (oldState == ButtonState.MinDown && up)
                //        WindowState = FormWindowState.Minimized;
                //}
                //else if (showMax && _maxButtonBounds.Contains(location))
                //{
                //    _buttonState = ButtonState.MaxOver;

                //    if (oldState == ButtonState.MaxDown && up)
                //        Maximized = !Maximized;
                //}
                //else if (ControlBox && _xButtonBounds.Contains(location))
                //{
                //    _buttonState = ButtonState.XOver;

                //    if (oldState == ButtonState.XDown && up)
                //        Close();
                //}
                //else if (_drawerButtonBounds.Contains(location))
                //{
                //    _buttonState = ButtonState.DrawerOver;
                //}
                //else
                //{
                //    _buttonState = ButtonState.None;
                //}
            }

            if (oldState != _buttonState)
                Invalidate();
        }
        /// <summary>
        /// Overrides the Closing Event to Animate the Slide Out
        /// </summary>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _formOverlay.Visible = false;
            _formOverlay.Close();
            _formOverlay.Dispose();

            DialogResult res = this.DialogResult;

            base.OnClosing(e);
        }

        /// <summary>
        /// Closes the Form after the pull out animation
        /// </summary>
        void _AnimationManager_OnAnimationFinished(object sender)
        {
            Close();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Prevents the Form from beeing dragged
        /// </summary>
        protected override void WndProc(ref Message message)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;

            switch (message.Msg)
            {
                case WM_SYSCOMMAND:
                    int command = message.WParam.ToInt32() & 0xfff0;
                    if (command == SC_MOVE)
                        return;
                    break;
            }

            base.WndProc(ref message);
        }

    }
    public enum TitleAlign
    {
        LeftTop,
        LeftMiddle,
        LeftBottom,
        //RightTop,
        //RightCenter,
        //RightBottom,
        CenterTop,
        CenterMiddle,
        CenterBottom,
    }
}

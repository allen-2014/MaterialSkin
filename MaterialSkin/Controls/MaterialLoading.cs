using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MaterialSkin.Animations;

namespace MaterialSkin.Controls
{
    public class MaterialLoading:Form,IMaterialControl
    {
        //Properties for managing the material design properties
        [Browsable(false)]
        public int Depth { get; set; }

        [Browsable(false)]
        public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

        [Browsable(false)]
        public MouseState MouseState { get; set; }

        private int loadingIconSize = 64;
        private Rectangle textRect = default;

        private readonly AnimationManager _AnimationManager;
        private bool CloseAnimation = false;
        private bool OpenAnimation = false;

        //private Form _parentForm = default;
        private MaterialPictureBox _picBox = default;


        private bool _loading = false;
        [Browsable(false)]
        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                Visible = _loading;
                if(!Visible)
                {
                    Opacity = 0;
                }
            }
        }

        private string _text = string.Empty;
        [Browsable(false)]
        public new string Text
        {
            get { return _text; }
            set
            {
                _text = value;
            }
        }


        public MaterialLoading(Form parentForm)
        {
            //draw text to be clear
            DoubleBuffered = true;
            //
            BackColor = Color.Black;
            if (DesignMode)
                Opacity = 0.7;
            else
                Opacity = 0;
            MinimizeBox = false;
            MaximizeBox = false;
            Text = "";
            ShowIcon = false;
            ControlBox = false;
            FormBorderStyle = FormBorderStyle.None;
            //Size = new Size(ParentForm.Width, ParentForm.Height),
            ShowInTaskbar = false;
            //Owner = ParentForm,
            //Visible = true;
            //Location = new Point(ParentForm.Location.X, ParentForm.Location.Y),
            Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

            //when designed mode, parentForm is null
            Size = new Size(parentForm.Width, parentForm.Height);
            Owner = parentForm;
            Location = new Point(parentForm.Location.X, parentForm.Location.Y);
            //_parentForm = parentForm;
            //
            loadingIconSize = Size.Width > Size.Height ? Size.Height / 10 : Size.Width / 10;
            textRect = new Rectangle(0, 0, Size.Width / 3, Size.Height);

            //
            _picBox = new MaterialPictureBox();
            _picBox.SizeMode = PictureBoxSizeMode.StretchImage;
            _picBox.Image = Properties.Resources.loading;
            _picBox.Size = new Size(loadingIconSize, loadingIconSize);
            this.Controls.Add(_picBox);
            //
            _AnimationManager = new AnimationManager();
            _AnimationManager.AnimationType = AnimationType.EaseOut;
            _AnimationManager.Increment = 0.03;
            _AnimationManager.OnAnimationProgress += _AnimationManager_OnAnimationProgress;
        }



        //public MaterialLoading():this(null)
        //{

        //}


        /// <summary>
        /// Sets up the Starting Location and starts the Animation
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {

            base.OnLoad(e);

            Location = new Point(Convert.ToInt32(Owner.Location.X), Convert.ToInt32(Owner.Location.Y ));
            //son control ordinate
            //10 for moving abouv a little
            _picBox.Location = new Point(_picBox.Parent.Width / 2 - _picBox.Width / 2, _picBox.Parent.Height / 2 - _picBox.Height / 2 - 10);
            _picBox.BackColor = Color.Transparent;
            //
            int txtWidth = Width / 3;
            //4 for space text and picbox
            textRect = new Rectangle(Width / 2 - txtWidth / 2, _picBox.Location.Y + _picBox.Height + 4, txtWidth, Height - _picBox.Location.Y - _picBox.Height);
        }

        private void _AnimationManager_OnAnimationProgress(object sender)
        {
            if (OpenAnimation || CloseAnimation)
            {
                if (Opacity > 0.7)
                {
                    OpenAnimation = false;
                    return;
                }
                Opacity = _AnimationManager.GetProgress();
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if(Visible)
            {
                //animation value =0 -> 1
                _AnimationManager.Increment = 0.03;
                _AnimationManager.StartNewAnimation(AnimationDirection.In);
                OpenAnimation = true;
            }
            
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (NativeTextRenderer NativeText = new NativeTextRenderer(g))
            {
                NativeText.DrawMultilineTransparentText(
                    _text,
                    //CharacterCasing == CharacterCasingEnum.Upper ? base.Text.ToUpper() : CharacterCasing == CharacterCasingEnum.Lower ? base.Text.ToLower() :
                    //    CharacterCasing == CharacterCasingEnum.Title ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(base.Text.ToLower()) : base.Text,
                    SkinManager.getLogFontByType(MaterialSkinManager.fontType.Button),
                    SkinManager.ColorScheme.TextColor,
                    textRect.Location,
                    textRect.Size,
                    NativeTextRenderer.TextAlignFlags.Center | NativeTextRenderer.TextAlignFlags.Top);
            }

        }

    }
}

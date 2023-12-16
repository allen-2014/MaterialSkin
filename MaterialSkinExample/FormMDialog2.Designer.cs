
namespace MaterialSkinExample
{
    partial class FormMDialog2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mBtnOK = new MaterialSkin.Controls.MaterialButton();
            this.mBtnCancel = new MaterialSkin.Controls.MaterialButton();
            this.SuspendLayout();
            // 
            // mBtnOK
            // 
            this.mBtnOK.AutoSize = false;
            this.mBtnOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mBtnOK.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.mBtnOK.Depth = 0;
            this.mBtnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mBtnOK.DrawShadows = false;
            this.mBtnOK.HighEmphasis = true;
            this.mBtnOK.Icon = null;
            this.mBtnOK.Location = new System.Drawing.Point(527, 163);
            this.mBtnOK.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.mBtnOK.MaterialToolTip = null;
            this.mBtnOK.MouseState = MaterialSkin.MouseState.HOVER;
            this.mBtnOK.Name = "mBtnOK";
            this.mBtnOK.NoAccentTextColor = System.Drawing.Color.Empty;
            this.mBtnOK.Size = new System.Drawing.Size(59, 36);
            this.mBtnOK.TabIndex = 0;
            this.mBtnOK.Text = "OK";
            this.mBtnOK.ToolTipCaption = "";
            this.mBtnOK.ToolTipDuration = 0;
            this.mBtnOK.ToolTipPlacement = MaterialSkin.Controls.MaterialToolTipPlacement.BottomCenter;
            this.mBtnOK.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Text;
            this.mBtnOK.UseAccentColor = true;
            this.mBtnOK.UseVisualStyleBackColor = true;
            // 
            // mBtnCancel
            // 
            this.mBtnCancel.AutoSize = false;
            this.mBtnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mBtnCancel.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.mBtnCancel.Depth = 0;
            this.mBtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mBtnCancel.DrawShadows = false;
            this.mBtnCancel.HighEmphasis = true;
            this.mBtnCancel.Icon = null;
            this.mBtnCancel.Location = new System.Drawing.Point(389, 163);
            this.mBtnCancel.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.mBtnCancel.MaterialToolTip = null;
            this.mBtnCancel.MouseState = MaterialSkin.MouseState.HOVER;
            this.mBtnCancel.Name = "mBtnCancel";
            this.mBtnCancel.NoAccentTextColor = System.Drawing.Color.Empty;
            this.mBtnCancel.Size = new System.Drawing.Size(118, 36);
            this.mBtnCancel.TabIndex = 1;
            this.mBtnCancel.Text = "mBtnCancel";
            this.mBtnCancel.ToolTipCaption = "";
            this.mBtnCancel.ToolTipDuration = 0;
            this.mBtnCancel.ToolTipPlacement = MaterialSkin.Controls.MaterialToolTipPlacement.BottomCenter;
            this.mBtnCancel.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Text;
            this.mBtnCancel.UseAccentColor = true;
            this.mBtnCancel.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ButtonUseAccentColor = true;
            this.ClientSize = new System.Drawing.Size(593, 208);
            this.Controls.Add(this.mBtnCancel);
            this.Controls.Add(this.mBtnOK);
            this.KeyPreview = true;
            this.MCancelButton = this.mBtnCancel;
            this.MValidationButton = this.mBtnOK;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Title = "Title";
            this.ResumeLayout(false);

        }

        #endregion

        private MaterialSkin.Controls.MaterialButton mBtnOK;
        private MaterialSkin.Controls.MaterialButton mBtnCancel;
    }
}